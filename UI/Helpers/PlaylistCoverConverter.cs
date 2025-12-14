using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Windows.Media.PixelFormats;

namespace UI.Helpers
{
    public class PlaylistCoverConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? coverPath = value as string;

            // If there's a valid cover path and the file exists, return it
            if (!string.IsNullOrEmpty(coverPath) && File.Exists(coverPath))
            {
                try
                {
                    return new BitmapImage(new Uri(coverPath));
                }
                catch
                {
                    // If the image fails to load, fall back to default
                }
            }

            // Return a default placeholder image
            return CreateDefaultPlaylistImage();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Convert back is not needed for this converter
            throw new NotImplementedException();
        }

        private BitmapImage CreateDefaultPlaylistImage()
        {
            // Create a simple geometric placeholder for missing playlist covers
            var renderTarget = new RenderTargetBitmap(120, 120, 96, 96, Pbgra32);
            var drawingVisual = new DrawingVisual();

            using (var drawingContext = drawingVisual.RenderOpen())
            {
                // Draw a colored background
                var brush = new SolidColorBrush(Colors.MediumPurple);
                drawingContext.DrawRectangle(brush, null, new Rect(0, 0, 120, 120));

                // Draw some simple shapes to represent music
                var iconBrush = new SolidColorBrush(Colors.White);
                var pen = new Pen(iconBrush, 2);

                // Draw a simple musical note shape
                drawingContext.DrawEllipse(iconBrush, pen, new Point(60, 60), 20, 20);
                
                var geometry = new StreamGeometry();
                using (var context = geometry.Open())
                {
                    context.BeginFigure(new Point(75, 40), true, true);
                    context.LineTo(new Point(75, 80), true, true);
                    context.LineTo(new Point(90, 80), true, true);
                    context.LineTo(new Point(90, 30), true, true);
                }
                drawingContext.DrawGeometry(iconBrush, pen, geometry);
            }

            renderTarget.Render(drawingVisual);
            
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            var stream = new MemoryStream();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTarget));
            encoder.Save(stream);
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            bitmap.Freeze(); // Important for cross-thread access
            
            return bitmap;
        }
    }
}