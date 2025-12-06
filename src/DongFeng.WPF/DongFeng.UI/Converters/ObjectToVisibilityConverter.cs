using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DongFeng.UI.Converters
{
    public class ObjectToVisibilityConverter : IValueConverter
    {
        public bool IsInverted { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = value != null;
            if (value is string s && string.IsNullOrEmpty(s)) isVisible = false;
            
            if (IsInverted) isVisible = !isVisible;

            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
