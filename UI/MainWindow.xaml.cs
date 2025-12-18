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
    public partial class MainWindow : Window
    {
        public readonly TrackService _trackService;
        public readonly PlaylistService _playlistService;
        public readonly UserService _userService;
        public readonly PaymentService _paymentService;

        private Track? _currentTrack;
        private List<Track> _currentTrackList = new List<Track>();
        private int _currentTrackIndex = -1;
        private bool _isPlaying = false;

        private MediaPlayer? _mediaPlayer;
        private double _currentVolume = 0.1;
        private bool _isUserChangingProgress = false;

        public MainWindow(TrackService trackService, PlaylistService playlistService, UserService userService, PaymentService paymentService)
        {
            InitializeComponent();

            _trackService = trackService;
            _playlistService = playlistService;
            _userService = userService;
            _paymentService = paymentService;

            
            if (VolumeSlider != null)
            {
                VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;
                VolumeSlider.Value = 10; 
                _currentVolume = 0.1; 
            }

            
            MainFrame.Navigate(new HomePage(_trackService, _playlistService, _userService));
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new HomePage(_trackService, _playlistService, _userService));
        }

        private void PlaylistsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PlaylistsPage(_playlistService, _trackService, _userService));
        }

        private void LibraryButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LibraryPage(_trackService, _playlistService, _userService));
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SettingsPage());
        }

        private void SubscriptionButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SubscriptionPage(_userService, _paymentService));
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (App.Current.Properties.Contains("CurrentUser"))
            {
                
                MainFrame.Navigate(new AccountPage(_userService));
            }
            else
            {
                
                MainFrame.Navigate(new LoginPage(_userService));
            }
        }

        public void UpdateLoginButtonAfterLogin()
        {
            if (LoginButton != null)
            {
                LoginButton.Content = "Аккаунт";
            }
        }

        public void UpdateLoginButtonAfterLogout()
        {
            if (LoginButton != null)
            {
                LoginButton.Content = "Вход";
            }
        }

        
        private void PreviousTrackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentTrackList.Count > 0 && _currentTrackIndex > 0)
            {
                _currentTrackIndex--;
                PlayTrack(_currentTrackList[_currentTrackIndex], _currentTrackList);
            }
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mediaPlayer != null)
            {
                if (_isPlaying)
                {
                    _mediaPlayer.Pause();
                    _isPlaying = false;
                    PlayPauseButton.Content = "▶";
                }
                else
                {
                    _mediaPlayer.Play();
                    _isPlaying = true;
                    PlayPauseButton.Content = "⏸";
                }
            }
        }

        private void NextTrackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentTrackList.Count > 0 && _currentTrackIndex < _currentTrackList.Count - 1)
            {
                _currentTrackIndex++;
                PlayTrack(_currentTrackList[_currentTrackIndex], _currentTrackList);
            }
        }

        public void PlayTrack(Track track, List<Track> trackList)
        {
            _currentTrack = track;
            _currentTrackList = trackList;
            _currentTrackIndex = trackList.IndexOf(track);

            CurrentTrackTitle.Text = track.Title;
            CurrentTrackArtist.Text = track.Artist?.ArtistName ?? "Неизвестный исполнитель";

            if (!string.IsNullOrEmpty(track.CoverPath) && System.IO.File.Exists(track.CoverPath))
            {
                try
                {
                    var bitmap = new System.Windows.Media.Imaging.BitmapImage(new Uri(track.CoverPath));
                    CurrentTrackCover.Source = bitmap;
                }
                catch
                {
                    
                    CurrentTrackCover.Source = null;
                }
            }
            else
            {
                
                CurrentTrackCover.Source = null;
            }

            
            if (_mediaPlayer != null)
            {
                _mediaPlayer.MediaEnded -= MediaPlayer_MediaEnded;
                _mediaPlayer.MediaOpened -= MediaPlayer_MediaOpened;
                _mediaPlayer.Stop();
                _mediaPlayer.Close();
            }

            
            _mediaPlayer = new MediaPlayer();

            
            _mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            _mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;

            
            _mediaPlayer.Volume = _currentVolume;

            
            if (System.IO.File.Exists(track.TrackFilePath))
            {
                try
                {
                    _mediaPlayer.Open(new Uri(track.TrackFilePath));
                    _mediaPlayer.Play();
                    _isPlaying = true;
                    PlayPauseButton.Content = "⏸";
                }
                catch
                {
                    
                    _isPlaying = false;
                    PlayPauseButton.Content = "▶";
                }
            }

            
            UpdatePlaybackButtons();
        }

        private void UpdatePlaybackButtons()
        {
            
            if (_currentTrackList.Count == 0)
            {
                PreviousTrackButton.IsEnabled = false;
                NextTrackButton.IsEnabled = false;
            }
            else
            {
                PreviousTrackButton.IsEnabled = _currentTrackIndex > 0;
                NextTrackButton.IsEnabled = _currentTrackIndex < _currentTrackList.Count - 1;
            }
        }

        private void MediaPlayer_MediaOpened(object? sender, EventArgs e)
        {
            
            if (_mediaPlayer != null)
            {
                var duration = _mediaPlayer.NaturalDuration;
                if (duration.HasTimeSpan)
                {
                    var totalSeconds = (int)duration.TimeSpan.TotalSeconds;
                    TotalTimeText.Text = TimeSpanToString(TimeSpan.FromSeconds(totalSeconds));

                   
                    StartProgressTimer();
                }
            }
        }

        private void StartProgressTimer()
        {
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (sender, e) =>
            {
                if (_mediaPlayer != null && !_isUserChangingProgress)
                {
                    var position = _mediaPlayer.Position;
                    var duration = _mediaPlayer.NaturalDuration;

                    if (duration.HasTimeSpan)
                    {
                        var totalSeconds = (int)duration.TimeSpan.TotalSeconds;
                        var currentSeconds = (int)position.TotalSeconds;

                        if (totalSeconds > 0)
                        {
                            var progress = (double)currentSeconds / totalSeconds * 100;
                            ProgressSlider.Value = progress;
                            CurrentTimeText.Text = TimeSpanToString(position);
                        }
                    }
                }
            };
            timer.Start();
        }

        private string TimeSpanToString(TimeSpan timeSpan)
        {
            if (timeSpan.Hours > 0)
            {
                return timeSpan.ToString(@"h\:mm\:ss");
            }
            else
            {
                return timeSpan.ToString(@"m\:ss");
            }
        }

        private void ProgressSlider_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isUserChangingProgress = true;
        }

        private void ProgressSlider_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isUserChangingProgress = false;

            if (_mediaPlayer != null && _currentTrack != null)
            {
                var newPosition = TimeSpan.FromSeconds(ProgressSlider.Value / 100.0 *
                    _mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds);
                _mediaPlayer.Position = newPosition;
                CurrentTimeText.Text = TimeSpanToString(newPosition);
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _currentVolume = e.NewValue / 100.0; 

            if (_mediaPlayer != null)
            {
                _mediaPlayer.Volume = _currentVolume; 
            }
        }

        private void MediaPlayer_MediaEnded(object? sender, EventArgs e)
        {
            
            if (_currentTrackIndex < _currentTrackList.Count - 1)
            {
                _currentTrackIndex++;
                PlayTrack(_currentTrackList[_currentTrackIndex], _currentTrackList);
            }
        }

        
        public void PlayTrackFromOtherPage(Track track, List<Track> trackList)
        {
            PlayTrack(track, trackList);
        }
    }
}