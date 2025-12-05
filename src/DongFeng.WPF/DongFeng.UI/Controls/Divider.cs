using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DongFeng.UI.Controls
{
    public class Divider : ContentControl
    {
        static Divider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Divider), new FrameworkPropertyMetadata(typeof(Divider)));
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Divider), new PropertyMetadata(Orientation.Horizontal));

        public Dock ContentPlacement
        {
            get { return (Dock)GetValue(ContentPlacementProperty); }
            set { SetValue(ContentPlacementProperty, value); }
        }

        // Using Dock enum for Left (Start), Right (End). Top/Bottom usually not used for Horizontal divider but we can map.
        // Let's simplify: Left, Center, Right.
        // But for Vertical divider: Top, Center, Bottom.
        // Let's use a custom enum or HorizontalAlignment? 
        // HorizontalContentAlignment works for Horizontal divider.
        // VerticalContentAlignment works for Vertical divider.
        // Let's just rely on HorizontalContentAlignment/VerticalContentAlignment inherited from ContentControl.
        
        public static readonly DependencyProperty ContentPlacementProperty =
            DependencyProperty.Register("ContentPlacement", typeof(Dock), typeof(Divider), new PropertyMetadata(Dock.Left));
        
        // Actually, standard alignment properties are enough if we build the template right.
        // But usually Divider has "Line - Text - Line" (Center) or "Line - Text" (Left) or "Text - Line" (Right/Left with specific look).
        // Let's stick to standard HorizontalContentAlignment.
    }
}

