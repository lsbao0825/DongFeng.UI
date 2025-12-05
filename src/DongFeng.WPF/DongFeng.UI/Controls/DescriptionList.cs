using System.Windows;
using System.Windows.Controls;

namespace DongFeng.UI.Controls
{
    public class DescriptionList : ItemsControl
    {
        static DescriptionList()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DescriptionList), new FrameworkPropertyMetadata(typeof(DescriptionList)));
        }
        
        // No special logic needed for now, just an ItemsControl. 
        // We might want to add "Layout" property later (Vertical vs Horizontal).
        // For now, default style handles Grid/WrapPanel layout.
    }

    public class DescriptionItem : ContentControl
    {
        static DescriptionItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DescriptionItem), new FrameworkPropertyMetadata(typeof(DescriptionItem)));
        }

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(DescriptionItem), new PropertyMetadata(null));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(DescriptionItem), new PropertyMetadata(Orientation.Horizontal));
    }
}

