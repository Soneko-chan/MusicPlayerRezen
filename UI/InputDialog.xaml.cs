using System;
using System.Windows;
using System.Windows.Controls;

namespace UI
{
    public partial class InputDialog : Window
    {
        public string? Answer { get; private set; }

        public InputDialog(string title, string prompt, string defaultValue = "")
        {
            Title = title;
            Width = 400;
            Height = 180;
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            var promptTextBlock = new TextBlock()
            {
                Text = prompt,
                Margin = new Thickness(10)
            };
            Grid.SetRow(promptTextBlock, 0);

            var textBox = new TextBox()
            {
                Text = defaultValue,
                Margin = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Center
            };
            textBox.KeyDown += (s, e) => {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    Answer = textBox.Text;
                    DialogResult = true;
                    Close();
                }
                else if (e.Key == System.Windows.Input.Key.Escape)
                {
                    DialogResult = false;
                    Close();
                }
            };
            Grid.SetRow(textBox, 1);

            var buttonPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10)
            };

            var okButton = new Button()
            {
                Content = "OK",
                Margin = new Thickness(5),
                Width = 80,
                Height = 30
            };
            okButton.Click += (s, e) => {
                Answer = textBox.Text;
                DialogResult = true;
                Close();
            };

            var cancelButton = new Button()
            {
                Content = "Отмена",
                Margin = new Thickness(5),
                Width = 80,
                Height = 30
            };
            cancelButton.Click += (s, e) => {
                DialogResult = false;
                Close();
            };

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);

            Grid.SetRow(buttonPanel, 2);

            grid.Children.Add(promptTextBlock);
            grid.Children.Add(textBox);
            grid.Children.Add(buttonPanel);

            Content = grid;

            // Фокус на текстбокс
            Loaded += (s, e) => textBox.Focus();
        }
    }
}