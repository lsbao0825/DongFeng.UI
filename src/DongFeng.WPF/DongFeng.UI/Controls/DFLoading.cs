using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DongFeng.UI.Controls
{
    [TemplatePart(Name = "PART_CancelButton", Type = typeof(Button))]
    public class DFLoading : ContentControl
    {
        static DFLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DFLoading), new FrameworkPropertyMetadata(typeof(DFLoading)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (GetTemplateChild("PART_CancelButton") is Button btn)
            {
                btn.Click += (s, e) =>
                {
                    RaiseEvent(new RoutedEventArgs(CancelEvent));
                };
            }
        }

        public bool IsDFLoading
        {
            get { return (bool)GetValue(IsDFLoadingProperty); }
            set { SetValue(IsDFLoadingProperty, value); }
        }
        public static readonly DependencyProperty IsDFLoadingProperty =
            DependencyProperty.Register("IsDFLoading", typeof(bool), typeof(DFLoading), new PropertyMetadata(false));

        public string DFLoadingText
        {
            get { return (string)GetValue(DFLoadingTextProperty); }
            set { SetValue(DFLoadingTextProperty, value); }
        }
        public static readonly DependencyProperty DFLoadingTextProperty =
            DependencyProperty.Register("DFLoadingText", typeof(string), typeof(DFLoading), new PropertyMetadata("DFLoading..."));

        public bool IsCancellable
        {
            get { return (bool)GetValue(IsCancellableProperty); }
            set { SetValue(IsCancellableProperty, value); }
        }
        public static readonly DependencyProperty IsCancellableProperty =
            DependencyProperty.Register("IsCancellable", typeof(bool), typeof(DFLoading), new PropertyMetadata(false));

        // 路由事件
        public static readonly RoutedEvent CancelEvent = EventManager.RegisterRoutedEvent(
            "Cancel", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DFLoading));

        public event RoutedEventHandler Cancel
        {
            add { AddHandler(CancelEvent, value); }
            remove { RemoveHandler(CancelEvent, value); }
        }
    }
}
