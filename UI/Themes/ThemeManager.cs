using System;
using System.Windows;
using System.Windows.Media;

namespace UI.Themes
{
    public static class ThemeManager
    {
        private static bool _isDarkTheme = false;

        public static void ApplyTheme(bool isDarkTheme)
        {
            if (_isDarkTheme == isDarkTheme) return;

            var application = Application.Current;
            if (application == null) return;

            // Находим ResourceDictionary с текущей темой и заменяем его
            var resourceDictionaries = application.Resources.MergedDictionaries;

            // Удаляем предыдущий файл темы
            ResourceDictionary oldTheme = null;
            for (int i = resourceDictionaries.Count - 1; i >= 0; i--)
            {
                var dict = resourceDictionaries[i];
                if (dict.Source != null &&
                    (dict.Source.OriginalString.Contains("LightThemeResources.xaml") ||
                     dict.Source.OriginalString.Contains("DarkThemeResources.xaml")))
                {
                    oldTheme = dict;
                    resourceDictionaries.RemoveAt(i);
                    break;
                }
            }

            // Загружаем новую тему
            ResourceDictionary newTheme;
            if (isDarkTheme)
            {
                newTheme = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/UI;component/Themes/DarkThemeResources.xaml", UriKind.Absolute)
                };
            }
            else
            {
                newTheme = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/UI;component/Themes/LightThemeResources.xaml", UriKind.Absolute)
                };
            }

            // Добавляем новую тему
            resourceDictionaries.Add(newTheme);

            _isDarkTheme = isDarkTheme;

            // Принудительно обновляем все окна приложения
            foreach (Window window in application.Windows)
            {
                // Обновление ресурсов окна для применения новых цветов
                window.InvalidateMeasure();
                window.InvalidateArrange();
                window.UpdateLayout();
            }
        }

        public static bool IsDarkTheme()
        {
            var application = Application.Current;
            if (application == null) return false;

            // Проверяем, является ли фон темным
            if (application.Resources.Contains("CurrentBackgroundColor"))
            {
                var color = (Color)application.Resources["CurrentBackgroundColor"];
                // Простой способ определения: если яркость цвета меньше 0.5, то это темная тема
                double brightness = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
                return brightness < 0.5;
            }

            // По умолчанию предполагаем светлую тему
            return false;
        }
    }
}