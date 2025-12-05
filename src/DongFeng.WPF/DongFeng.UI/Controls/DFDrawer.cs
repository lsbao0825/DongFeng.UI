using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DongFeng.UI.Controls
{
    [TemplatePart(Name = "PART_Mask", Type = typeof(Border))]
    [TemplatePart(Name = "PART_Content", Type = typeof(Border))]
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
    public class DFDrawer : ContentControl
    {
        private Border _mask;
        private Border _content;

        static DFDrawer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DFDrawer), new FrameworkPropertyMetadata(typeof(DFDrawer)));
            CommandManager.RegisterClassCommandBinding(typeof(DFDrawer), 
                new CommandBinding(CloseCommand, OnCloseCommand));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _mask = GetTemplateChild("PART_Mask") as Border;
            _content = GetTemplateChild("PART_Content") as Border;

            if (_mask != null)
            {
                _mask.MouseLeftButtonDown += (s, e) =>
                {
                    if (MaskCanClose)
                    {
                        SetCurrentValue(IsOpenProperty, false);
                    }
                };
            }
            
            UpdateVisualState(false);
        }

        public static readonly RoutedUICommand CloseCommand = new RoutedUICommand("Close", "Close", typeof(DFDrawer));

        private static void OnCloseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var DFDrawer = sender as DFDrawer;
            if (DFDrawer != null)
            {
                DFDrawer.IsOpen = false;
            }
        }

        #region Dependency Properties

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(DFDrawer), 
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsOpenChanged));

        public Dock Position
        {
            get { return (Dock)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(Dock), typeof(DFDrawer), new PropertyMetadata(Dock.Right));

        public bool MaskCanClose
        {
            get { return (bool)GetValue(MaskCanCloseProperty); }
            set { SetValue(MaskCanCloseProperty, value); }
        }
        public static readonly DependencyProperty MaskCanCloseProperty =
            DependencyProperty.Register("MaskCanClose", typeof(bool), typeof(DFDrawer), new PropertyMetadata(true));

        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(DFDrawer), new PropertyMetadata(null));

        #endregion

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DFDrawer)d).UpdateVisualState(true);
        }

        private void UpdateVisualState(bool useAnimation)
        {
            if (IsOpen)
            {
                Visibility = Visibility.Visible;
            }

            if (_content == null || _mask == null)
            {
                if (!IsOpen)
                {
                    Visibility = Visibility.Collapsed;
                }
                return;
            }

            if (IsOpen)
            {
                _mask.IsHitTestVisible = true;
                if (useAnimation)
                {
                    AnimateOpen();
                }
                else
                {
                    _mask.Opacity = 0.5;
                    var transform = GetTranslateTransform(_content);
                    transform.X = 0;
                    transform.Y = 0;
                }
            }
            else
            {
                _mask.IsHitTestVisible = false;
                if (useAnimation)
                {
                    AnimateClose();
                }
                else
                {
                    Visibility = Visibility.Collapsed;
                }
            }
        }

        private TranslateTransform GetTranslateTransform(UIElement element)
        {
            var currentTransform = element.RenderTransform as TranslateTransform;
            if (currentTransform == null || currentTransform.IsFrozen)
            {
                var newTransform = new TranslateTransform();
                element.RenderTransform = newTransform;
                return newTransform;
            }
            return currentTransform;
        }

        private void AnimateOpen()
        {
            // Mask Fade In
            var maskAnimation = new DoubleAnimation(0, 0.5, TimeSpan.FromMilliseconds(300));
            _mask.BeginAnimation(UIElement.OpacityProperty, maskAnimation);

            // Content Slide In
            var transform = GetTranslateTransform(_content);
            DoubleAnimation slideAnimation = null;

            // Use fixed width/height for calculation if ActualWidth is 0 (e.g. first open)
            double width = _content.ActualWidth;
            if (width <= 0) width = !double.IsNaN(_content.Width) ? _content.Width : 300;
            double height = _content.ActualHeight;
            if (height <= 0) height = !double.IsNaN(_content.Height) ? _content.Height : 300;

            switch (Position)
            {
                case Dock.Left:
                    transform.X = -width;
                    slideAnimation = new DoubleAnimation(-width, 0, TimeSpan.FromMilliseconds(300));
                    slideAnimation.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
                    transform.BeginAnimation(TranslateTransform.XProperty, slideAnimation);
                    break;
                case Dock.Right:
                    transform.X = width;
                    slideAnimation = new DoubleAnimation(width, 0, TimeSpan.FromMilliseconds(300));
                    slideAnimation.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
                    transform.BeginAnimation(TranslateTransform.XProperty, slideAnimation);
                    break;
                case Dock.Top:
                    transform.Y = -height;
                    slideAnimation = new DoubleAnimation(-height, 0, TimeSpan.FromMilliseconds(300));
                    slideAnimation.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
                    transform.BeginAnimation(TranslateTransform.YProperty, slideAnimation);
                    break;
                case Dock.Bottom:
                    transform.Y = height;
                    slideAnimation = new DoubleAnimation(height, 0, TimeSpan.FromMilliseconds(300));
                    slideAnimation.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
                    transform.BeginAnimation(TranslateTransform.YProperty, slideAnimation);
                    break;
            }
        }

        private void AnimateClose()
        {
            // Mask Fade Out
            var maskAnimation = new DoubleAnimation(0.5, 0, TimeSpan.FromMilliseconds(300));
            _mask.BeginAnimation(UIElement.OpacityProperty, maskAnimation);

            // Content Slide Out
            var transform = GetTranslateTransform(_content);
            DoubleAnimation slideAnimation = null;

            // Use ActualWidth/Height of the content grid, or the control if stretched?
            // Usually the DFDrawer panel has fixed width or height.
            double width = _content.ActualWidth;
            if (width <= 0) width = !double.IsNaN(_content.Width) ? _content.Width : 300;
            double height = _content.ActualHeight;
            if (height <= 0) height = !double.IsNaN(_content.Height) ? _content.Height : 300;

            switch (Position)
            {
                case Dock.Left:
                    slideAnimation = new DoubleAnimation(0, -width, TimeSpan.FromMilliseconds(300));
                    slideAnimation.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn };
                    slideAnimation.Completed += (s, e) => Visibility = Visibility.Collapsed;
                    transform.BeginAnimation(TranslateTransform.XProperty, slideAnimation);
                    break;
                case Dock.Right:
                    slideAnimation = new DoubleAnimation(0, width, TimeSpan.FromMilliseconds(300));
                    slideAnimation.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn };
                    slideAnimation.Completed += (s, e) => Visibility = Visibility.Collapsed;
                    transform.BeginAnimation(TranslateTransform.XProperty, slideAnimation);
                    break;
                case Dock.Top:
                    slideAnimation = new DoubleAnimation(0, -height, TimeSpan.FromMilliseconds(300));
                    slideAnimation.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn };
                    slideAnimation.Completed += (s, e) => Visibility = Visibility.Collapsed;
                    transform.BeginAnimation(TranslateTransform.YProperty, slideAnimation);
                    break;
                case Dock.Bottom:
                    slideAnimation = new DoubleAnimation(0, height, TimeSpan.FromMilliseconds(300));
                    slideAnimation.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn };
                    slideAnimation.Completed += (s, e) => Visibility = Visibility.Collapsed;
                    transform.BeginAnimation(TranslateTransform.YProperty, slideAnimation);
                    break;
            }
        }
    }
}

