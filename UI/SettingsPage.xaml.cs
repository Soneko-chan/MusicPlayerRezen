using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Domain;
using Services;
using Ookii.Dialogs.Wpf;

namespace UI
{
    public partial class SettingsPage : Page
    {
        private TrackService? _trackService;

        public SettingsPage()
        {
            InitializeComponent();

            // Получаем сервисы из свойств приложения
            if (Application.Current.Properties.Contains("TrackService"))
            {
                _trackService = Application.Current.Properties["TrackService"] as TrackService;
            }

            LoadSettingsPageContent();
        }

        private void LoadSettingsPageContent()
        {
            // Отключаем обработчики событий, чтобы избежать ложного срабатывания при установке значений
            if (DarkThemeRadio != null) DarkThemeRadio.Checked -= ThemeRadio_Checked;
            if (LightThemeRadio != null) LightThemeRadio.Checked -= ThemeRadio_Checked;
            if (DarkThemeRadio != null) DarkThemeRadio.Unchecked -= ThemeRadio_Checked;
            if (LightThemeRadio != null) LightThemeRadio.Unchecked -= ThemeRadio_Checked;

            // Проверяем сохраненную тему в настройках и устанавливаем соответствующий переключатель
            string savedTheme = UI.Properties.Settings.Default.SelectedTheme;
            bool isDarkTheme = !string.IsNullOrEmpty(savedTheme) && savedTheme.Contains("DarkTheme.xaml");

            if (DarkThemeRadio != null) DarkThemeRadio.IsChecked = isDarkTheme;
            if (LightThemeRadio != null) LightThemeRadio.IsChecked = !isDarkTheme;

            // Включаем обработчики событий обратно
            if (DarkThemeRadio != null) DarkThemeRadio.Checked += ThemeRadio_Checked;
            if (LightThemeRadio != null) LightThemeRadio.Checked += ThemeRadio_Checked;
            if (DarkThemeRadio != null) DarkThemeRadio.Unchecked += ThemeRadio_Checked;
            if (LightThemeRadio != null) LightThemeRadio.Unchecked += ThemeRadio_Checked;

            // Показываем текущую директорию импорта
            CurrentImportPathText.Text = UI.Properties.Settings.Default.ImportMusicPath != null
                ? $"Текущая директория импорта: {UI.Properties.Settings.Default.ImportMusicPath}"
                : "Текущая директория импорта: не выбрана";
        }

        private void ThemeRadio_Checked(object sender, RoutedEventArgs e)
        {
            var isDarkTheme = DarkThemeRadio != null && DarkThemeRadio.IsChecked == true;

            // Применяем новую тему
            UI.Themes.ThemeManager.ApplyTheme(isDarkTheme);

            // Сохраняем выбор темы
            UI.Properties.Settings.Default.SelectedTheme = isDarkTheme ? "Themes/DarkTheme.xaml" : "Themes/LightTheme.xaml";
            UI.Properties.Settings.Default.Save();
        }

        private void ImportMusicButton_Click(object sender, RoutedEventArgs e)
        {
            // Используем Ookii.Dialogs для выбора папки
            var dialog = new VistaFolderBrowserDialog()
            {
                Description = "Выберите папку с музыкальными файлами",
                UseDescriptionForTitle = true
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                var directoryPath = dialog.SelectedPath;
                if (!Directory.Exists(directoryPath))
                {
                    MessageBox.Show("Указанная директория не существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (_trackService == null)
                {
                    MessageBox.Show("Сервис треков не инициализирован", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                try
                {
                    ImportTracksFromDirectory(directoryPath);
                    MessageBox.Show($"Импорт треков завершен. Найдено и добавлено {GetFileCount(directoryPath)} аудиофайлов.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Обновляем отображение текущей директории
                    CurrentImportPathText.Text = $"Текущая директория импорта: {directoryPath}";

                    // Сохраняем путь в настройках
                    UI.Properties.Settings.Default.ImportMusicPath = directoryPath;
                    UI.Properties.Settings.Default.Save();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при импорте треков: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private int GetFileCount(string directoryPath)
        {
            var supportedExtensions = new[] { ".mp3", ".wav", ".flac", ".m4a", ".wma", ".aac", ".ogg" };
            var files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories)
                .Where(file => supportedExtensions.Contains(Path.GetExtension(file).ToLower()));
            return files.Count();
        }

        private void ImportTracksFromDirectory(string directoryPath)
        {
            var supportedExtensions = new[] { ".mp3", ".wav", ".flac", ".m4a", ".wma", ".aac", ".ogg" };
            var files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories)
                .Where(file => supportedExtensions.Contains(Path.GetExtension(file).ToLower()));

            foreach (var filePath in files)
            {
                try
                {
                    var track = CreateTrackFromAudioFile(filePath);
                    if (track != null)
                    {
                        // Проверяем, не существует ли уже трек с таким же путем
                        var existingTrack = _trackService?.GetAllTracks()
                            .FirstOrDefault(t => t.TrackFilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase));

                        if (existingTrack == null)
                        {
                            _trackService.AddTrack(track);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Логируем ошибку для конкретного файла, но продолжаем обработку других файлов
                    System.Diagnostics.Debug.WriteLine($"Ошибка при обработке файла {filePath}: {ex.Message}");
                }
            }
        }

        private Track CreateTrackFromAudioFile(string filePath)
        {
            try
            {
                var file = TagLib.File.Create(filePath);

                var track = new Track
                {
                    Title = file.Tag.Title ?? Path.GetFileNameWithoutExtension(filePath),
                    TrackFilePath = filePath,
                    TrackDuration = (float)file.Properties.Duration.TotalSeconds,
                    DateCreated = File.GetCreationTime(filePath)
                };

                // Пытаемся найти или установить обложку
                if (file.Tag.Pictures.Length > 0)
                {
                    var picture = file.Tag.Pictures[0];
                    var directory = Path.GetDirectoryName(filePath);
                    if (!string.IsNullOrEmpty(directory))
                    {
                        var coverPath = Path.Combine(directory,
                                                     Path.GetFileNameWithoutExtension(filePath) + "_cover.jpg");
                        File.WriteAllBytes(coverPath, picture.Data.Data);
                        track.CoverPath = coverPath;
                    }
                }

                file.Dispose();
                return track;
            }
            catch
            {
                // Если не удалось обработать файл через TagLib, создаем базовую запись
                return new Track
                {
                    Title = Path.GetFileNameWithoutExtension(filePath),
                    TrackFilePath = filePath,
                    DateCreated = File.GetCreationTime(filePath)
                };
            }
        }

    }
}