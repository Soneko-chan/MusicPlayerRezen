using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Ookii.Dialogs.Wpf;

namespace UI
{
    public partial class EditPlaylistDialog : Window
    {
        public string PlaylistName { get; private set; }
        public string? CoverImagePath { get; private set; }

        public EditPlaylistDialog(string currentName, string? currentCoverPath)
        {
            InitializeComponent();

            PlaylistNameTextBox.Text = currentName ?? string.Empty;
            LoadCoverImage(currentCoverPath);

            
            CoverImagePath = currentCoverPath;
        }

        private void LoadCoverImage(string? coverPath)
        {
            if (!string.IsNullOrEmpty(coverPath) && File.Exists(coverPath))
            {
                try
                {
                    var bitmap = new BitmapImage(new Uri(coverPath));
                    CoverImage.Source = bitmap;
                    CoverImage.Visibility = Visibility.Visible;
                    PlaceholderText.Visibility = Visibility.Collapsed;
                }
                catch
                {
                    
                    CoverImage.Visibility = Visibility.Collapsed;
                    PlaceholderText.Visibility = Visibility.Visible;
                }
            }
            else
            {
               
                CoverImage.Visibility = Visibility.Collapsed;
                PlaceholderText.Visibility = Visibility.Visible;
            }
        }

        private void SelectCoverButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaOpenFileDialog()
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Все файлы|*.*",
                Title = "Выберите обложку для плейлиста"
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                var selectedPath = dialog.FileName;
                if (File.Exists(selectedPath))
                {
                    
                    var appDataPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "MusicPlayer", "PlaylistCovers"
                    );
                    
                    Directory.CreateDirectory(appDataPath);
                    
                    var fileName = Path.GetFileName(selectedPath);
                    var fileExtension = Path.GetExtension(selectedPath);
                    var newFileName = $"playlist_{DateTime.Now:yyyyMMdd_HHmmss}_{fileName}";
                    var newFilePath = Path.Combine(appDataPath, newFileName);
                    
                    
                    File.Copy(selectedPath, newFilePath, true);
                    
                    CoverImagePath = newFilePath;
                    LoadCoverImage(newFilePath);
                }
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            PlaylistName = PlaylistNameTextBox.Text?.Trim();
            
            if (string.IsNullOrWhiteSpace(PlaylistName))
            {
                MessageBox.Show("Введите название плейлиста", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}