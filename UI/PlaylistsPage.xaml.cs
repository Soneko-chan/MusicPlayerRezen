using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Domain;
using Services;

namespace UI
{
    public partial class PlaylistsPage : Page
    {
        private readonly PlaylistService _playlistService;
        private readonly TrackService _trackService;
        private readonly UserService _userService;

        public PlaylistsPage(PlaylistService playlistService, TrackService trackService, UserService userService)
        {
            InitializeComponent();

            _playlistService = playlistService;
            _trackService = trackService;
            _userService = userService;

            LoadPlaylistsPageContent();
        }

        private void LoadPlaylistsPageContent()
        {
            
            var playlists = _playlistService.GetAllPlaylists();
            
            
            if (App.Current.Properties.Contains("CurrentUser"))
            {
                var currentUser = App.Current.Properties["CurrentUser"] as User;
                if (currentUser != null)
                {
                    playlists = playlists.Where(p => p.UserId == currentUser.UserId).ToList();
                }
            }
            
            
            PlaylistsList.ItemsSource = playlists;
        }

        private void CreatePlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            
            var newPlaylist = new Playlist
            {
                PlaylistName = "Новый плейлист",
                DateCreated = DateTime.Now
            };

            
            if (App.Current.Properties.Contains("CurrentUser"))
            {
                var currentUser = App.Current.Properties["CurrentUser"] as User;
                if (currentUser != null)
                {
                    newPlaylist.UserId = currentUser.UserId;
                }
            }

            _playlistService.AddPlaylist(newPlaylist);
            LoadPlaylistsPageContent();
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