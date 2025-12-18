using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Domain;
using Services;

namespace UI
{
    public partial class LibraryPage : Page
    {
        private readonly TrackService _trackService;
        private readonly PlaylistService _playlistService;
        private readonly UserService _userService;

        public LibraryPage(TrackService trackService, PlaylistService playlistService, UserService userService)
        {
            InitializeComponent();

            _trackService = trackService;
            _playlistService = playlistService;
            _userService = userService;

            LoadLibraryPageContent();
        }

        private void LoadLibraryPageContent()
        {
            
            var tracks = _trackService.GetAllTracks();
            UpdateTracksDisplay(tracks);
        }

        private void UpdateTracksDisplay(List<Track> tracks)
        {
           
            ContentList.ItemsSource = tracks;
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "Поиск музыки, исполнителей, альбомов...")
                SearchBox.Text = "";
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
                SearchBox.Text = "Поиск музыки, исполнителей, альбомов...";
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var searchTerm = SearchBox.Text;
            if (searchTerm == "Поиск музыки, исполнителей, альбомов...")
                searchTerm = "";

            var tracks = _trackService.GetAllTracks()
                .Where(t => t.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            (t.Artist?.ArtistName ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            (t.Album?.AlbumName ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            UpdateTracksDisplay(tracks);
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
                    mainWindow.PlayTrack(track, nonNullTracks);
                }
            }
        }
    }

}