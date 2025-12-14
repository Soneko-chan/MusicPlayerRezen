using System;
using System.Windows;
using System.Windows.Controls;
using Services;

namespace UI
{
    public partial class RegisterPage : Page
    {
        private readonly UserService? _userService;

        public RegisterPage(UserService userService)
        {
            InitializeComponent();

            _userService = userService;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginTextBox.Text;
            var username = UsernameTextBox.Text;
            var email = EmailTextBox.Text;

            string password = UI.Helpers.PasswordBoxHelper.GetPassword(PasswordBox);
            string confirmPassword = UI.Helpers.PasswordBoxHelper.GetPassword(ConfirmPasswordBox);

            // Validate input
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать не менее 6 символов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                _userService?.RegisterUser(login, username, email, password);
                MessageBox.Show("Регистрация прошла успешно! Теперь вы можете войти в систему.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Navigate back to login page
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null && _userService != null)
                {
                    mainWindow.MainFrame.Navigate(new LoginPage(_userService));
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null && _userService != null)
            {
                mainWindow.MainFrame.Navigate(new LoginPage(_userService));
            }
        }
    }
}