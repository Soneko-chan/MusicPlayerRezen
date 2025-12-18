using Data.Interfaces;
using Data.SqlServer;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using Services;

namespace UI
{
    public partial class App : Application
    {
        private ITrackRepository? _trackRepository;
        private IArtistRepository? _artistRepository;
        private IAlbumRepository? _albumRepository;
        private IPlaylistRepository? _playlistRepository;
        private IUserRepository? _userRepository;
        private IPaymentRepository? _paymentRepository;
        private MusicPlayerDbContext? _dbContext;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.database.json")
                    .Build();

                var factory = new MusicPlayerDbContextFactory();
                _dbContext = factory.CreateDbContext(configuration);

                _dbContext.Database.Migrate();


                _trackRepository = new TrackRepository(_dbContext);
                _artistRepository = new ArtistRepository(_dbContext);
                _albumRepository = new AlbumRepository(_dbContext);
                _playlistRepository = new PlaylistRepository(_dbContext);
                _userRepository = new UserRepository(_dbContext);
                _paymentRepository = new PaymentRepository(_dbContext);


                var trackService = new TrackService(_trackRepository, _artistRepository, _albumRepository);
                var playlistService = new PlaylistService(_playlistRepository, _trackRepository, _userRepository);
                var userService = new UserService(_userRepository);
                var paymentService = new PaymentService(_paymentRepository, _userRepository);


                SeedInitData();


                Application.Current.Properties["TrackService"] = trackService;
                Application.Current.Properties["PlaylistService"] = playlistService;
                Application.Current.Properties["UserService"] = userService;
                Application.Current.Properties["PaymentService"] = paymentService;


                ApplySavedTheme();

                var mainWindow = new MainWindow(trackService, playlistService, userService, paymentService);
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при запуске приложения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        private void ApplySavedTheme()
        {
            var app = Application.Current;
            if (app != null)
            {
                
                string savedTheme = UI.Properties.Settings.Default.SelectedTheme;
                if (!string.IsNullOrEmpty(savedTheme))
                {
                    
                    bool isDarkTheme = savedTheme.Contains("DarkTheme.xaml");
                    UI.Themes.ThemeManager.ApplyTheme(isDarkTheme);
                }
                else
                {
                    
                    UI.Themes.ThemeManager.ApplyTheme(false);
                }
            }
        }

        private void SeedInitData()
        {
            
            if (_userRepository != null && _userRepository.GetAll().Any())
            {
                return;
            }

            
            var userService = new UserService(_userRepository!);

            try
            {
                
                userService.RegisterUser("admin", "Admin User", "admin@example.com", "admin123");
                userService.RegisterUser("user1", "Regular User 1", "user1@example.com", "password1");
                userService.RegisterUser("user2", "Regular User 2", "user2@example.com", "password2");

                
                var user = _userRepository?.GetByLogin("user1");
                if (user != null)
                {
                    user.SubscriptionExpiry = DateTime.Now.AddDays(30); 
                    _userRepository?.Update(user);
                }

                
                user = _userRepository?.GetByLogin("user2");
                if (user != null)
                {
                    user.SubscriptionExpiry = DateTime.Now.AddDays(-10); 
                    _userRepository?.Update(user);
                }

                
                _dbContext?.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при инициализации тестовых данных: {ex.Message}");
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            
            UI.Properties.Settings.Default.Save();

            try
            {
                
                _dbContext?.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при сохранении изменений перед выходом: {ex.Message}");
            }

            _dbContext?.Dispose();
            base.OnExit(e);
        }
    }
}