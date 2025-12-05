using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DongFeng.UI.Controls
{
    public class DFAvatar : Control
    {
        static DFAvatar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DFAvatar), new FrameworkPropertyMetadata(typeof(DFAvatar)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            // Listen for Image Failed
            if (GetTemplateChild("PART_Image") is Image img)
            {
                img.ImageFailed += (s, e) =>
                {
                    // Hide image, show icon/text fallback
                    img.Visibility = Visibility.Collapsed;
                };
            }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(DFAvatar), new PropertyMetadata(null));

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(object), typeof(DFAvatar), new PropertyMetadata(null));

        public object Icon
        {
            get { return (object)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(DFAvatar), new PropertyMetadata(null));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(DFAvatar), new PropertyMetadata(new CornerRadius(50))); // Default circle-ish

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
    }
}
