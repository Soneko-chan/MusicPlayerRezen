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
                
                var recentlyAddedTracks = _trackService.GetAllTracks()
                    .OrderByDescending(t => t.DateCreated)
                    .Take(10)
                    .ToList();

                
                RecentlyAddedList.ItemsSource = recentlyAddedTracks;

                var popularTracks = GetPopularTracks(10);
                PopularTracksList.ItemsSource = popularTracks;

               
                
            }
            catch (Exception ex)
            {
                
                System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке контента главной страницы: {ex.Message}");
            }
        }

        private List<Track> GetPopularTracks(int count)
        {
            
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
                    var nonNullTracks = allTracks.Where(t => t != null).ToList();
                    mainWindow.PlayTrackFromOtherPage(track, nonNullTracks);
                }
            }
        }

        
    }
}