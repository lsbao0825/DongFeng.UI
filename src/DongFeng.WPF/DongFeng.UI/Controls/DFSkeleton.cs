using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DongFeng.UI.Controls
{
    public class DFSkeleton : Control
    {
        static DFSkeleton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DFSkeleton), new FrameworkPropertyMetadata(typeof(DFSkeleton)));
        }

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(DFSkeleton), new PropertyMetadata(true));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(DFSkeleton), new PropertyMetadata(new CornerRadius(4)));
    }
}

