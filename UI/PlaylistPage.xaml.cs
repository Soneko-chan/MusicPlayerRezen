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

            
            this.DataContext = _playlist;

            LoadPlaylistPageContent();
        }

        private void LoadPlaylistPageContent()
        {
            
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
            
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Navigate(new AddToPlaylistPage(_playlist, _playlistService, _trackService, _userService, this));
            }
        }

        private void EditPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            
            var dialog = new EditPlaylistDialog(_playlist.PlaylistName, _playlist.PlaylistCoverPath);
            if (dialog.ShowDialog() == true)
            {
                var newName = dialog.PlaylistName;
                var newCoverPath = dialog.CoverImagePath;

                if (!string.IsNullOrWhiteSpace(newName))
                {
                    
                    var updatedPlaylist = new Playlist
                    {
                        PlaylistId = _playlist.PlaylistId,
                        PlaylistName = newName,
                        UserId = _playlist.UserId,
                        PlaylistCoverPath = newCoverPath,
                        DateCreated = _playlist.DateCreated,
                        
                        PlaylistTracks = new List<PlaylistTrack>(_playlist.PlaylistTracks)
                    };

                    
                    _playlistService.UpdatePlaylist(updatedPlaylist);

                    
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
                    
                    _playlistService.DeletePlaylist(_playlist.PlaylistId);

                    
                    var mainWindow = Window.GetWindow(this) as MainWindow;
                    if (mainWindow != null)
                    {
                        
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

       
        public void RefreshPlaylist()
        {
            
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