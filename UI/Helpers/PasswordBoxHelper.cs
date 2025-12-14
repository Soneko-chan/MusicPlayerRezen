using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace UI.Helpers
{
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached(
                "Password",
                typeof(string),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(string.Empty, OnPasswordPropertyChanged));

        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached(
                "Attach",
                typeof(bool),
                typeof(PasswordBoxHelper), 
                new PropertyMetadata(false, OnAttachPropertyChanged));

        private static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached(
                "IsUpdating", 
                typeof(bool), 
                typeof(PasswordBoxHelper));

        public static void SetAttach(PasswordBox box, bool value)
        {
            box.SetValue(AttachProperty, value);
        }

        public static bool GetAttach(PasswordBox box)
        {
            return (bool)box.GetValue(AttachProperty);
        }

        public static string GetPassword(PasswordBox box)
        {
            return (string)box.GetValue(PasswordProperty);
        }

        public static void SetPassword(PasswordBox box, string value)
        {
            box.SetValue(PasswordProperty, value);
        }

        private static void OnAttachPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox == null) return;

            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordChanged;
            }
            else
            {
                passwordBox.PasswordChanged -= PasswordChanged;
            }
        }

        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox == null) return;

            var newPassword = (string)e.NewValue;

            if (!GetIsUpdating(passwordBox))
            {
                passwordBox.Password = newPassword;
            }
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox == null) return;

            SetIsUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);
            SetIsUpdating(passwordBox, false);
        }

        private static bool GetIsUpdating(PasswordBox box)
        {
            return (bool)box.GetValue(IsUpdatingProperty);
        }

        private static void SetIsUpdating(PasswordBox box, bool value)
        {
            box.SetValue(IsUpdatingProperty, value);
        }
    }
}