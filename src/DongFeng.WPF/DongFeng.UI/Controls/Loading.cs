using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DongFeng.UI.Controls
{
    [TemplatePart(Name = "PART_CancelButton", Type = typeof(Button))]
    public class Loading : ContentControl
    {
        static Loading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Loading), new FrameworkPropertyMetadata(typeof(Loading)));
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

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(Loading), new PropertyMetadata(false));

        public string LoadingText
        {
            get { return (string)GetValue(LoadingTextProperty); }
            set { SetValue(LoadingTextProperty, value); }
        }
        public static readonly DependencyProperty LoadingTextProperty =
            DependencyProperty.Register("LoadingText", typeof(string), typeof(Loading), new PropertyMetadata("Loading..."));

        public bool IsCancellable
        {
            get { return (bool)GetValue(IsCancellableProperty); }
            set { SetValue(IsCancellableProperty, value); }
        }
        public static readonly DependencyProperty IsCancellableProperty =
            DependencyProperty.Register("IsCancellable", typeof(bool), typeof(Loading), new PropertyMetadata(false));

        // 路由事件
        public static readonly RoutedEvent CancelEvent = EventManager.RegisterRoutedEvent(
            "Cancel", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Loading));

        public event RoutedEventHandler Cancel
        {
            add { AddHandler(CancelEvent, value); }
            remove { RemoveHandler(CancelEvent, value); }
        }
    }
}
