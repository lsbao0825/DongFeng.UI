using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace DongFeng.UI.Controls
{
    [TemplatePart(Name = ElementStarsPanel, Type = typeof(Panel))]
    public class Rating : Control
    {
        private const string ElementStarsPanel = "PART_StarsPanel";
        private Panel? _starsPanel;

        static Rating()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Rating), new FrameworkPropertyMetadata(typeof(Rating)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _starsPanel = GetTemplateChild(ElementStarsPanel) as Panel;
            GenerateStars();
            UpdateStars(Value);
        }

        #region Properties

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(Rating),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (Rating)d;
            control.UpdateStars((double)e.NewValue);
            control.RaiseValueChangedEvent((double)e.OldValue, (double)e.NewValue);
        }

        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int), typeof(Rating),
                new PropertyMetadata(5, OnMaximumChanged));

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Rating)d).GenerateStars();
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(Rating), new PropertyMetadata(false));

        public double ItemSize
        {
            get { return (double)GetValue(ItemSizeProperty); }
            set { SetValue(ItemSizeProperty, value); }
        }

        public static readonly DependencyProperty ItemSizeProperty =
            DependencyProperty.Register("ItemSize", typeof(double), typeof(Rating), new PropertyMetadata(24.0));

        public bool AllowHalf
        {
            get { return (bool)GetValue(AllowHalfProperty); }
            set { SetValue(AllowHalfProperty, value); }
        }

        public static readonly DependencyProperty AllowHalfProperty =
            DependencyProperty.Register("AllowHalf", typeof(bool), typeof(Rating), new PropertyMetadata(false));

        #endregion

        #region Events

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
            "ValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(Rating));

        public event RoutedPropertyChangedEventHandler<double> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        private void RaiseValueChangedEvent(double oldValue, double newValue)
        {
            RaiseEvent(new RoutedPropertyChangedEventArgs<double>(oldValue, newValue, ValueChangedEvent));
        }

        #endregion

        private void GenerateStars()
        {
            if (_starsPanel == null) return;

            _starsPanel.Children.Clear();
            for (int i = 1; i <= Maximum; i++)
            {
                var item = new RatingItem
                {
                    Value = i,
                    Width = ItemSize,
                    Height = ItemSize
                };

                var binding = new System.Windows.Data.Binding("ItemSize") { Source = this };
                item.SetBinding(WidthProperty, binding);
                item.SetBinding(HeightProperty, binding);

                // Pass mouse events to parent logic
                item.MouseMove += Item_MouseMove;
                item.MouseLeave += Item_MouseLeave;
                item.Click += Item_Click;

                _starsPanel.Children.Add(item);
            }
            UpdateStars(Value);
        }

        private void Item_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsReadOnly) return;
            if (sender is RatingItem item)
            {
                double val = item.Value;
                if (AllowHalf)
                {
                    Point p = e.GetPosition(item);
                    if (p.X < item.ActualWidth / 2)
                    {
                        val -= 0.5;
                    }
                }
                UpdateStars(val);
            }
        }

        private void Item_MouseLeave(object sender, MouseEventArgs e)
        {
            if (IsReadOnly) return;
            // Only reset if we leave the whole control, handled by parent OnMouseLeave
        }
        
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (!IsReadOnly)
            {
                UpdateStars(Value);
            }
        }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            if (IsReadOnly) return;
            if (sender is RatingItem item)
            {
                double val = item.Value;
                if (AllowHalf)
                {
                    // We need the mouse position relative to the item to determine half
                    // But Click event doesn't give mouse pos easily without args.
                    // We can use Mouse.GetPosition
                    Point p = Mouse.GetPosition(item);
                    if (p.X < item.ActualWidth / 2)
                    {
                        val -= 0.5;
                    }
                }
                Value = val;
            }
        }

        private void UpdateStars(double value)
        {
            if (_starsPanel == null) return;
            foreach (UIElement child in _starsPanel.Children)
            {
                if (child is RatingItem item)
                {
                    if (value >= item.Value)
                    {
                        item.State = RatingItemState.Full;
                    }
                    else if (value >= item.Value - 0.5)
                    {
                        item.State = RatingItemState.Half;
                    }
                    else
                    {
                        item.State = RatingItemState.Empty;
                    }
                }
            }
        }
    }

    public class RatingItem : ButtonBase
    {
        static RatingItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RatingItem), new FrameworkPropertyMetadata(typeof(RatingItem)));
        }

        public int Value { get; set; }

        public RatingItemState State
        {
            get { return (RatingItemState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(RatingItemState), typeof(RatingItem), new PropertyMetadata(RatingItemState.Empty));
    }

    public enum RatingItemState
    {
        Empty,
        Half,
        Full
    }
}
