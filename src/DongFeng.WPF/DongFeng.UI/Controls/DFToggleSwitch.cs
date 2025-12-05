using System.Windows;
using System.Windows.Controls.Primitives;

namespace DongFeng.UI.Controls
{
    public class DFToggleSwitch : ToggleButton
    {
        static DFToggleSwitch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DFToggleSwitch), new FrameworkPropertyMetadata(typeof(DFToggleSwitch)));
        }

        public static readonly DependencyProperty OnContentProperty =
            DependencyProperty.Register("OnContent", typeof(object), typeof(DFToggleSwitch), new PropertyMetadata(null));

        public object OnContent
        {
            get { return GetValue(OnContentProperty); }
            set { SetValue(OnContentProperty, value); }
        }

        public static readonly DependencyProperty OffContentProperty =
            DependencyProperty.Register("OffContent", typeof(object), typeof(DFToggleSwitch), new PropertyMetadata(null));

        public object OffContent
        {
            get { return GetValue(OffContentProperty); }
            set { SetValue(OffContentProperty, value); }
        }
    }
}

