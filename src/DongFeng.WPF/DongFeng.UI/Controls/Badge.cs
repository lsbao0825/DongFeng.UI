using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DongFeng.UI.Controls
{
    public class Badge : ContentControl
    {
        static Badge()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Badge), new FrameworkPropertyMetadata(typeof(Badge)));
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(Badge), new PropertyMetadata(null, OnValueChanged));

        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int), typeof(Badge), new PropertyMetadata(99, OnValueChanged));

        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty IsDotProperty =
            DependencyProperty.Register("IsDot", typeof(bool), typeof(Badge), new PropertyMetadata(false));

        public bool IsDot
        {
            get { return (bool)GetValue(IsDotProperty); }
            set { SetValue(IsDotProperty, value); }
        }

        public static readonly DependencyProperty ShowZeroProperty =
            DependencyProperty.Register("ShowZero", typeof(bool), typeof(Badge), new PropertyMetadata(false, OnValueChanged));

        public bool ShowZero
        {
            get { return (bool)GetValue(ShowZeroProperty); }
            set { SetValue(ShowZeroProperty, value); }
        }
        
        public static readonly DependencyProperty BadgeBackgroundProperty =
            DependencyProperty.Register("BadgeBackground", typeof(System.Windows.Media.Brush), typeof(Badge), new PropertyMetadata(null));

        public System.Windows.Media.Brush BadgeBackground
        {
            get { return (System.Windows.Media.Brush)GetValue(BadgeBackgroundProperty); }
            set { SetValue(BadgeBackgroundProperty, value); }
        }
        
        // Offset Property
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(Point), typeof(Badge), new PropertyMetadata(new Point(0, 0)));

        public Point Offset
        {
            get { return (Point)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        private static readonly DependencyPropertyKey BadgeTextPropertyKey =
            DependencyProperty.RegisterReadOnly("BadgeText", typeof(string), typeof(Badge), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty BadgeTextProperty = BadgeTextPropertyKey.DependencyProperty;

        public string BadgeText
        {
            get { return (string)GetValue(BadgeTextProperty); }
            private set { SetValue(BadgeTextPropertyKey, value); }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Badge)?.UpdateBadgeText();
        }

        private void UpdateBadgeText()
        {
            if (Value is int i)
            {
                if (i == 0 && !ShowZero) 
                    BadgeText = string.Empty;
                else if (i > Maximum) 
                    BadgeText = $"{Maximum}+";
                else 
                    BadgeText = i.ToString();
            }
            else
            {
                BadgeText = Value?.ToString();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateBadgeText();
        }
    }
}
