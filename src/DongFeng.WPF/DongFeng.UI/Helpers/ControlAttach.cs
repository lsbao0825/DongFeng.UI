using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DongFeng.UI.Helpers
{
    public class ControlAttach
    {
        #region CornerRadius
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ControlAttach), new PropertyMetadata(new CornerRadius(3)));

        public static void SetCornerRadius(DependencyObject element, CornerRadius value)
        {
            element.SetValue(CornerRadiusProperty, value);
        }

        public static CornerRadius GetCornerRadius(DependencyObject element)
        {
            return (CornerRadius)element.GetValue(CornerRadiusProperty);
        }
        #endregion

        #region Watermark
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(ControlAttach), new PropertyMetadata(string.Empty));

        public static void SetWatermark(DependencyObject element, string value)
        {
            element.SetValue(WatermarkProperty, value);
        }

        public static string GetWatermark(DependencyObject element)
        {
            return (string)element.GetValue(WatermarkProperty);
        }
        #endregion

        #region FocusBorderBrush
        // DF UI focus color
        public static readonly DependencyProperty FocusBorderBrushProperty =
            DependencyProperty.RegisterAttached("FocusBorderBrush", typeof(Brush), typeof(ControlAttach), new PropertyMetadata(null));

        public static void SetFocusBorderBrush(DependencyObject element, Brush value)
        {
            element.SetValue(FocusBorderBrushProperty, value);
        }

        public static Brush GetFocusBorderBrush(DependencyObject element)
        {
            return (Brush)element.GetValue(FocusBorderBrushProperty);
        }
        #endregion

        #region HoverBackground
        public static readonly DependencyProperty HoverBackgroundProperty =
            DependencyProperty.RegisterAttached("HoverBackground", typeof(Brush), typeof(ControlAttach), new PropertyMetadata(null));

        public static void SetHoverBackground(DependencyObject element, Brush value)
        {
            element.SetValue(HoverBackgroundProperty, value);
        }

        public static Brush GetHoverBackground(DependencyObject element)
        {
            return (Brush)element.GetValue(HoverBackgroundProperty);
        }
        #endregion

        #region Icon
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.RegisterAttached("Icon", typeof(object), typeof(ControlAttach), new PropertyMetadata(null));

        public static void SetIcon(DependencyObject element, object value)
        {
            element.SetValue(IconProperty, value);
        }

        public static object GetIcon(DependencyObject element)
        {
            return element.GetValue(IconProperty);
        }
        #endregion
    }
}

