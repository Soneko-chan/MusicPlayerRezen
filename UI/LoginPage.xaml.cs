using System;
using System.Windows;
using System.Windows.Controls;
using Services;

namespace UI
{
    public partial class LoginPage : Page
    {
        private readonly UserService? _userService;

        public LoginPage(UserService userService)
        {
            InitializeComponent();

            _userService = userService;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginTextBox.Text;
            string password = UI.Helpers.PasswordBoxHelper.GetPassword(PasswordBox);

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var isValid = _userService?.ValidateUser(login, password) ?? false;
                if (isValid)
                {
                    var user = _userService?.GetUserByLogin(login);
                    if (user != null)
                    {
                        
                        App.Current.Properties["CurrentUser"] = user;

                        MessageBox.Show($"Добро пожаловать, {user.Username}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                        
                        var mainWindow = Window.GetWindow(this) as MainWindow;
                        if (mainWindow != null)
                        {
                            
                            mainWindow.UpdateLoginButtonAfterLogin();

                            mainWindow.MainFrame.Navigate(new HomePage(
                                mainWindow._trackService,
                                mainWindow._playlistService,
                                mainWindow._userService));
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при входе: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterClick(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null && _userService != null)
            {
                mainWindow.MainFrame.Navigate(new RegisterPage(_userService));
            }
        }

    }
}