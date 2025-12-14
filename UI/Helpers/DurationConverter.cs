using System;
using System.Globalization;
using System.Windows.Data;

namespace UI.Helpers
{
    public class DurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float durationInSeconds)
            {
                var timeSpan = TimeSpan.FromSeconds(durationInSeconds);
                
                // Format as mm:ss if less than 1 hour, otherwise as h:mm:ss
                if (timeSpan.TotalHours < 1)
                {
                    return $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
                }
                else
                {
                    return $"{(int)timeSpan.TotalHours}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
                }
            }
            
            if (value is double durationInDouble)
            {
                var timeSpan = TimeSpan.FromSeconds(durationInDouble);
                
                // Format as mm:ss if less than 1 hour, otherwise as h:mm:ss
                if (timeSpan.TotalHours < 1)
                {
                    return $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
                }
                else
                {
                    return $"{(int)timeSpan.TotalHours}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
                }
            }
            
            return "0:00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}