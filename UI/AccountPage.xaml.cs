using System;
using System.Windows;
using System.Windows.Controls;
using Domain;
using Services;

namespace UI
{
    public partial class AccountPage : Page
    {
        private readonly UserService _userService;
        private User? _currentUser;

        public AccountPage(UserService userService)
        {
            InitializeComponent();

            _userService = userService;

            // Получаем текущего пользователя из свойств приложения
            if (App.Current.Properties.Contains("CurrentUser"))
            {
                _currentUser = App.Current.Properties["CurrentUser"] as User;
                LoadUserData();
            }
        }

        private void LoadUserData()
        {
            if (_currentUser != null)
            {
                LoginValue.Text = _currentUser.Login;
                UsernameValue.Text = _currentUser.Username;
                EmailValue.Text = _currentUser.Email ?? "Не указан";
                DateCreatedValue.Text = _currentUser.DateCreated.ToString("dd.MM.yyyy");

                // Обновляем статус подписки
                if (_currentUser.SubscriptionExpiry.HasValue)
                {
                    if (_currentUser.SubscriptionExpiry > DateTime.Now)
                    {
                        SubscriptionStatus.Text = "Активна";
                        SubscriptionStatus.Foreground = (System.Windows.Media.Brush)FindResource("SuccessColorBrush");
                        SubscriptionExpiryValue.Text = _currentUser.SubscriptionExpiry.Value.ToString("dd.MM.yyyy");
                    }
                    else
                    {
                        SubscriptionStatus.Text = "Истекла";
                        SubscriptionStatus.Foreground = (System.Windows.Media.Brush)FindResource("DangerColorBrush");
                        SubscriptionExpiryValue.Text = _currentUser.SubscriptionExpiry.Value.ToString("dd.MM.yyyy");
                    }
                }
                else
                {
                    SubscriptionStatus.Text = "Не активна";
                    SubscriptionStatus.Foreground = (System.Windows.Media.Brush)FindResource("WarningColorBrush");
                    SubscriptionExpiryValue.Text = "Не указана";
                }
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Удаляем текущего пользователя из свойств приложения
            App.Current.Properties.Remove("CurrentUser");

            // Возвращаемся на главную страницу
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                // Меняем текст кнопки "Аккаунт" обратно на "Вход"
                mainWindow.UpdateLoginButtonAfterLogout();

                mainWindow.MainFrame.Navigate(new HomePage(
                    mainWindow._trackService,
                    mainWindow._playlistService,
                    mainWindow._userService));
            }
        }

        
    }
}