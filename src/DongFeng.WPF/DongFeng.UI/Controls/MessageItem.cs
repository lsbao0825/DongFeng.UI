using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DongFeng.UI.Controls
{
    public enum MessageType
    {
        Info,
        Success,
        Warning,
        Error
    }

    public class MessageItem : ContentControl
    {
        static MessageItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageItem), new FrameworkPropertyMetadata(typeof(MessageItem)));
        }

        public MessageItem()
        {
            this.Loaded += MessageItem_Loaded;
        }

        private void MessageItem_Loaded(object sender, RoutedEventArgs e)
        {
            // Entrance Animation
            var translate = new TranslateTransform(0, -20);
            this.RenderTransform = translate;
            
            var fadeAnim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
            var slideAnim = new DoubleAnimation(-20, 0, TimeSpan.FromMilliseconds(300))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            this.BeginAnimation(OpacityProperty, fadeAnim);
            translate.BeginAnimation(TranslateTransform.YProperty, slideAnim);

            // Auto Close Timer
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = Duration;
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                Close();
            };
            timer.Start();
        }

        public void Close()
        {
            // Exit Animation
            var fadeAnim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
            fadeAnim.Completed += (s, e) =>
            {
                if (this.Parent is Panel parent)
                {
                    parent.Children.Remove(this);
                }
                RaiseEvent(new RoutedEventArgs(ClosedEvent));
            };
            
            var translate = this.RenderTransform as TranslateTransform ?? new TranslateTransform();
            this.RenderTransform = translate;
            var slideAnim = new DoubleAnimation(0, -20, TimeSpan.FromMilliseconds(300));

            this.BeginAnimation(OpacityProperty, fadeAnim);
            translate.BeginAnimation(TranslateTransform.YProperty, slideAnim);
        }

        public static readonly RoutedEvent ClosedEvent = EventManager.RegisterRoutedEvent(
            "Closed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MessageItem));

        public event RoutedEventHandler Closed
        {
            add { AddHandler(ClosedEvent, value); }
            remove { RemoveHandler(ClosedEvent, value); }
        }

        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(MessageItem), new PropertyMetadata(TimeSpan.FromSeconds(3)));

        public MessageType Type
        {
            get { return (MessageType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(MessageType), typeof(MessageItem), new PropertyMetadata(MessageType.Info));
    }
}

