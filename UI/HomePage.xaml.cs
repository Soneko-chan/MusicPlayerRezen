using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Domain;
using Services;

namespace UI
{
    public partial class HomePage : Page
    {
        private readonly TrackService _trackService;
        private readonly PlaylistService _playlistService;
        private readonly UserService _userService;

        public HomePage(TrackService trackService, PlaylistService playlistService, UserService userService)
        {
            InitializeComponent();

            _trackService = trackService;
            _playlistService = playlistService;
            _userService = userService;

            LoadHomePageContent();
        }

        private void LoadHomePageContent()
        {
            try
            {
                // Загружаем недавно добавленные треки
                var recentlyAddedTracks = _trackService.GetAllTracks()
                    .OrderByDescending(t => t.DateCreated)
                    .Take(10)
                    .ToList();

                // Привязываем данные к ItemsControl
                RecentlyAddedList.ItemsSource = recentlyAddedTracks;

                // Загружаем популярные треки (например, треки, добавленные в плейлисты чаще всего)
                var popularTracks = GetPopularTracks(10);
                PopularTracksList.ItemsSource = popularTracks;

                // Загружаем последние плейлисты пользователя
                
            }
            catch (Exception ex)
            {
                // Обработка ошибок при загрузке данных
                System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке контента главной страницы: {ex.Message}");
            }
        }

        private List<Track> GetPopularTracks(int count)
        {
            // Простая логика: считаем треки, которые чаще всего встречаются в плейлистах
            var allTracks = _trackService.GetAllTracks();
            var playlistTracks = _playlistService.GetAllPlaylists()
                .SelectMany(p => p.PlaylistTracks)
                .GroupBy(pt => pt.TrackId)
                .OrderByDescending(g => g.Count())
                .Take(count)
                .Select(g => g.First().TrackId)
                .ToList();

            return allTracks
                .Where(t => playlistTracks.Contains(t.TrackId))
                .Take(count)
                .ToList();
        }

        private void TrackItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Border border && border.DataContext is Track track)
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    var allTracks = _trackService.GetAllTracks();
                    // Удаляем потенциальные null значения из списка перед передачей
                    var nonNullTracks = allTracks.Where(t => t != null).ToList();
                    mainWindow.PlayTrackFromOtherPage(track, nonNullTracks);
                }
            }
        }

        private void PlaylistItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Border border && border.DataContext is Playlist playlist)
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.MainFrame.Navigate(new PlaylistPage(playlist, _playlistService, _trackService, _userService));
                }
            }
        }
    }
}