using System;
using System.Windows;
using System.Windows.Controls;
using Services;

namespace UI
{
    public partial class SubscriptionPage : Page
    {
        private readonly UserService _userService;
        private readonly PaymentService _paymentService;
        private bool _isMonthlySelected = true;

        private TextBox? _cardNumberTextBox;
        private TextBox? _expiryTextBox;
        private TextBox? _cvvTextBox;


        public SubscriptionPage(UserService userService, PaymentService paymentService)
        {
            InitializeComponent();

            _userService = userService;
            _paymentService = paymentService;

            LoadSubscriptionPageContent();
        }

        private void LoadSubscriptionPageContent()
        {
            // Обновляем информацию о текущем пользователе
            if (App.Current.Properties.Contains("CurrentUser"))
            {
                var user = App.Current.Properties["CurrentUser"] as Domain.User;
                if (user != null)
                {
                    // Обновляем статус подписки
                    if (user.SubscriptionExpiry.HasValue)
                    {
                        if (user.SubscriptionExpiry > DateTime.Now)
                        {
                            var daysLeft = (user.SubscriptionExpiry.Value - DateTime.Now).Days;
                            SubscriptionStatusText.Text = $"Статус подписки: Активна до {user.SubscriptionExpiry:dd.MM.yyyy}";
                            DaysLeftText.Text = $"Осталось {daysLeft} дней";
                        }
                        else
                        {
                            SubscriptionStatusText.Text = "Статус подписки: Подписка истекла";
                            DaysLeftText.Text = "Подписка не активна";
                        }
                    }
                    else
                    {
                        SubscriptionStatusText.Text = "Статус подписки: Подписка отсутствует";
                        DaysLeftText.Text = "Подписка не активна";
                    }
                }
                else
                {
                    SubscriptionStatusText.Text = "Статус подписки: Не авторизован";
                    DaysLeftText.Text = "Войдите в аккаунт для просмотра статуса подписки";
                }
            }
            else
            {
                SubscriptionStatusText.Text = "Статус подписки: Не авторизован";
                DaysLeftText.Text = "Войдите в аккаунт для просмотра статуса подписки";
            }

            // Инициализируем поля формы оплаты
            _cardNumberTextBox = CardNumberTextBox;
            _expiryTextBox = ExpiryTextBox;
            _cvvTextBox = CvvTextBox;

           
        }


       

        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            var cardNumber = _cardNumberTextBox?.Text;
            var expiry = _expiryTextBox?.Text;
            var cvv = _cvvTextBox?.Text;

            // Validate input
            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length != 16 || !long.TryParse(cardNumber, out _))
            {
                MessageBox.Show("Пожалуйста, введите корректный номер карты (16 цифр)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(expiry) || expiry.Length != 5 || !expiry.Contains("/"))
            {
                MessageBox.Show("Пожалуйста, введите корректный срок действия (MM/YY)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(cvv) || (cvv.Length != 3 && cvv.Length != 4) || !int.TryParse(cvv, out _))
            {
                MessageBox.Show("Пожалуйста, введите корректный CVV код (3-4 цифры)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Calculate amount based on selected option
            decimal amount = _isMonthlySelected ? 149.00m : 1490.00m;

            try
            {
                var user = App.Current.Properties["CurrentUser"] as Domain.User;
                if (user != null)
                {
                    _paymentService.ProcessPayment(user.UserId, cardNumber, expiry, cvv, amount);
                    MessageBox.Show("Подписка успешно оплачена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Refresh the page to show updated status
                    LoadSubscriptionPageContent();
                }
                else
                {
                    MessageBox.Show("Пожалуйста, войдите в систему для оплаты подписки", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оплате: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}