using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Domain;
using Services;

namespace UI
{
    public partial class PlaylistPage : Page
    {
        private Playlist _playlist;
        private readonly PlaylistService _playlistService;
        private readonly TrackService _trackService;
        private readonly UserService _userService;

        public PlaylistPage(Playlist playlist, PlaylistService playlistService, TrackService trackService, UserService userService)
        {
            InitializeComponent();

            _playlist = playlist;
            _playlistService = playlistService;
            _trackService = trackService;
            _userService = userService;

            // Устанавливаем контекст данных для привязки
            this.DataContext = _playlist;

            LoadPlaylistPageContent();
        }

        private void LoadPlaylistPageContent()
        {
            // Привязываем треки плейлиста к ItemsControl
            var orderedTracks = _playlist.PlaylistTracks
                .OrderBy(pt => pt.TrackOrder)
                .Select(pt => pt.Track)
                .Where(t => t != null)
                .Cast<Track>()
                .ToList();

            TracksList.ItemsSource = orderedTracks;
        }

        private void TrackItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Border border && border.DataContext is Track track)
            {
                // Create a list of tracks in this playlist for the player to navigate
                var playlistTracks = _playlist.PlaylistTracks
                    .OrderBy(pt => pt.TrackOrder)
                    .Select(pt => pt.Track)
                    .Where(t => t != null)
                    .Cast<Track>()
                    .ToList();

                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.PlayTrack(track, playlistTracks);
                }
            }
        }

        private void AddTrackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to a page where user can select tracks to add
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Navigate(new AddToPlaylistPage(_playlist, _playlistService, _trackService, _userService, this));
            }
        }

        private void EditPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            // Создаем диалог для редактирования плейлиста с возможностью выбора обложки
            var dialog = new EditPlaylistDialog(_playlist.PlaylistName, _playlist.PlaylistCoverPath);
            if (dialog.ShowDialog() == true)
            {
                var newName = dialog.PlaylistName;
                var newCoverPath = dialog.CoverImagePath;

                if (!string.IsNullOrWhiteSpace(newName))
                {
                    // Создаем новую сущность для обновления
                    var updatedPlaylist = new Playlist
                    {
                        PlaylistId = _playlist.PlaylistId,
                        PlaylistName = newName,
                        UserId = _playlist.UserId,
                        PlaylistCoverPath = newCoverPath,
                        DateCreated = _playlist.DateCreated,
                        // Копируем существующие связи
                        PlaylistTracks = new List<PlaylistTrack>(_playlist.PlaylistTracks)
                    };

                    // Сохраняем изменения в БД
                    _playlistService.UpdatePlaylist(updatedPlaylist);

                    // После сохранения в БД, перезагружаем плейлист из БД для гарантии актуальности
                    var refreshedPlaylist = _playlistService.GetPlaylistById(_playlist.PlaylistId);
                    if (refreshedPlaylist != null)
                    {
                        _playlist = refreshedPlaylist;
                        this.DataContext = _playlist;
                        LoadPlaylistPageContent();
                    }
                }
            }
        }

        private void DeletePlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            // Show confirmation dialog
            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить плейлист \"{_playlist.PlaylistName}\"?\n\nЭто действие нельзя отменить.",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Delete the playlist using the service
                    _playlistService.DeletePlaylist(_playlist.PlaylistId);

                    // Navigate back to the playlists page
                    var mainWindow = Window.GetWindow(this) as MainWindow;
                    if (mainWindow != null)
                    {
                        // Navigate back to playlists page and ensure it refreshes content
                        var playlistsPage = new PlaylistsPage(_playlistService, _trackService, _userService);
                        mainWindow.MainFrame.Navigate(playlistsPage);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении плейлиста: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Method to refresh the playlist content
        public void RefreshPlaylist()
        {
            // Получаем обновленный плейлист из сервиса
            var updatedPlaylist = _playlistService.GetPlaylistById(_playlist.PlaylistId);
            if (updatedPlaylist != null)
            {
                _playlist = updatedPlaylist;
                this.DataContext = _playlist;
                LoadPlaylistPageContent();
            }
        }
    }
}