using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Domain;
using Services;

namespace UI
{
    public partial class AddToPlaylistPage : Page
    {
        private readonly Playlist _playlist;
        private readonly PlaylistService _playlistService;
        private readonly TrackService _trackService;
        private readonly UserService _userService;
        private readonly PlaylistPage _parentPage;

        public AddToPlaylistPage(Playlist playlist, PlaylistService playlistService, TrackService trackService, UserService userService, PlaylistPage parentPage)
        {
            InitializeComponent();

            _playlist = playlist;
            _playlistService = playlistService;
            _trackService = trackService;
            _userService = userService;
            _parentPage = parentPage;

            PlaylistNameText.Text = $"Добавить трек в '{_playlist.PlaylistName}'";

            LoadAddToPlaylistPageContent();
        }

        private void LoadAddToPlaylistPageContent()
        {
            // Получаем все треки
            var allTracks = _trackService.GetAllTracks();
            
            // Получаем ID треков, уже находящихся в плейлисте
            var playlistTrackIds = _playlist.PlaylistTracks.Select(pt => pt.TrackId).ToHashSet();
            
            // Фильтруем треки, которые еще не в плейлисте
            var availableTracks = allTracks.Where(t => !playlistTrackIds.Contains(t.TrackId)).ToList();

            // Привязываем данные к ItemsControl
            TracksList.ItemsSource = availableTracks;
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "Поиск треков...")
                SearchBox.Text = "";
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
                SearchBox.Text = "Поиск треков...";
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var searchTerm = SearchBox.Text;
            if (searchTerm == "Поиск треков...")
                searchTerm = "";

            // Получаем все треки
            var allTracks = _trackService.GetAllTracks();
            
            // Получаем ID треков, уже находящихся в плейлисте
            var playlistTrackIds = _playlist.PlaylistTracks.Select(pt => pt.TrackId).ToHashSet();
            
            // Фильтруем треки, которые еще не в плейлисте и соответствуют поисковому запросу
            var availableTracks = allTracks
                .Where(t => !playlistTrackIds.Contains(t.TrackId))
                .Where(t => t.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            (t.Artist?.ArtistName ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Обновляем список треков
            TracksList.ItemsSource = availableTracks;
        }

        // Метод для добавления выделенных треков в плейлист
        private void AddSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем выбранные треки из ListBox
                var selectedTracks = TracksList.SelectedItems.Cast<Track>().ToList();

                if (selectedTracks.Count == 0)
                {
                    MessageBox.Show("Пожалуйста, выберите хотя бы один трек для добавления", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                int order = _playlist.PlaylistTracks.Count; // Начальный порядковый номер

                foreach (var track in selectedTracks)
                {
                    _playlistService.AddTrackToPlaylist(_playlist.PlaylistId, track.TrackId, order++);
                }

                // Обновляем объект плейлиста, чтобы отразить новые треки
                var updatedPlaylist = _playlistService.GetPlaylistById(_playlist.PlaylistId);
                if (updatedPlaylist != null)
                {
                    _playlist.PlaylistTracks = updatedPlaylist.PlaylistTracks;
                }

                // Обновляем родительскую страницу
                _parentPage.RefreshPlaylist();

                // Возвращаемся на страницу плейлиста
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.MainFrame.Navigate(_parentPage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении треков: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}