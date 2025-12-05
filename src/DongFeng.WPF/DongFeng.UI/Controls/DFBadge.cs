using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DongFeng.UI.Controls
{
    public class DFBadge : ContentControl
    {
        static DFBadge()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DFBadge), new FrameworkPropertyMetadata(typeof(DFBadge)));
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(DFBadge), new PropertyMetadata(null, OnValueChanged));

        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int), typeof(DFBadge), new PropertyMetadata(99, OnValueChanged));

        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty IsDotProperty =
            DependencyProperty.Register("IsDot", typeof(bool), typeof(DFBadge), new PropertyMetadata(false));

        public bool IsDot
        {
            get { return (bool)GetValue(IsDotProperty); }
            set { SetValue(IsDotProperty, value); }
        }

        public static readonly DependencyProperty ShowZeroProperty =
            DependencyProperty.Register("ShowZero", typeof(bool), typeof(DFBadge), new PropertyMetadata(false, OnValueChanged));

        public bool ShowZero
        {
            get { return (bool)GetValue(ShowZeroProperty); }
            set { SetValue(ShowZeroProperty, value); }
        }
        
        public static readonly DependencyProperty DFBadgeBackgroundProperty =
            DependencyProperty.Register("DFBadgeBackground", typeof(System.Windows.Media.Brush), typeof(DFBadge), new PropertyMetadata(null));

        public System.Windows.Media.Brush DFBadgeBackground
        {
            get { return (System.Windows.Media.Brush)GetValue(DFBadgeBackgroundProperty); }
            set { SetValue(DFBadgeBackgroundProperty, value); }
        }
        
        // Offset Property
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(Point), typeof(DFBadge), new PropertyMetadata(new Point(0, 0)));

        public Point Offset
        {
            get { return (Point)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        private static readonly DependencyPropertyKey DFBadgeTextPropertyKey =
            DependencyProperty.RegisterReadOnly("DFBadgeText", typeof(string), typeof(DFBadge), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty DFBadgeTextProperty = DFBadgeTextPropertyKey.DependencyProperty;

        public string DFBadgeText
        {
            get { return (string)GetValue(DFBadgeTextProperty); }
            private set { SetValue(DFBadgeTextPropertyKey, value); }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DFBadge)?.UpdateDFBadgeText();
        }

        private void UpdateDFBadgeText()
        {
            if (Value is int i)
            {
                if (i == 0 && !ShowZero) 
                    DFBadgeText = string.Empty;
                else if (i > Maximum) 
                    DFBadgeText = $"{Maximum}+";
                else 
                    DFBadgeText = i.ToString();
            }
            else
            {
                DFBadgeText = Value?.ToString();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateDFBadgeText();
        }
    }
}
