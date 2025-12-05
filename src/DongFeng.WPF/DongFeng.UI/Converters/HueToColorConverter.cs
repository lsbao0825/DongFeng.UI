using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using DongFeng.UI.Helpers;

namespace DongFeng.UI.Converters
{
    public class HueToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double h)
            {
                return new SolidColorBrush(ColorHelper.HsvToColor(h, 1.0, 1.0));
            }
            return Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

