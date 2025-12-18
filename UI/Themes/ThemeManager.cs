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

            
            var resourceDictionaries = application.Resources.MergedDictionaries;

            
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

            
            resourceDictionaries.Add(newTheme);

            _isDarkTheme = isDarkTheme;

            
            foreach (Window window in application.Windows)
            {
                
                window.InvalidateMeasure();
                window.InvalidateArrange();
                window.UpdateLayout();
            }
        }

        public static bool IsDarkTheme()
        {
            var application = Application.Current;
            if (application == null) return false;

           
            if (application.Resources.Contains("CurrentBackgroundColor"))
            {
                var color = (Color)application.Resources["CurrentBackgroundColor"];
                
                double brightness = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
                return brightness < 0.5;
            }

            
            return false;
        }
    }
}