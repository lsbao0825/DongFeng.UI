using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace DongFeng.UI.Controls
{
    [TemplatePart(Name = "PART_Track", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_Selection", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_LowerThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_UpperThumb", Type = typeof(Thumb))]
    public class RangeSlider : Control
    {
        static RangeSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RangeSlider), new FrameworkPropertyMetadata(typeof(RangeSlider)));
        }

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(RangeSlider), new PropertyMetadata(0.0, OnPropertyChanged));

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(RangeSlider), new PropertyMetadata(100.0, OnPropertyChanged));

        public double LowerValue
        {
            get { return (double)GetValue(LowerValueProperty); }
            set { SetValue(LowerValueProperty, value); }
        }
        public static readonly DependencyProperty LowerValueProperty =
            DependencyProperty.Register("LowerValue", typeof(double), typeof(RangeSlider), new PropertyMetadata(0.0, OnLowerValueChanged, CoerceLowerValue));

        public double UpperValue
        {
            get { return (double)GetValue(UpperValueProperty); }
            set { SetValue(UpperValueProperty, value); }
        }
        public static readonly DependencyProperty UpperValueProperty =
            DependencyProperty.Register("UpperValue", typeof(double), typeof(RangeSlider), new PropertyMetadata(100.0, OnUpperValueChanged, CoerceUpperValue));

        public double TickFrequency
        {
            get { return (double)GetValue(TickFrequencyProperty); }
            set { SetValue(TickFrequencyProperty, value); }
        }
        public static readonly DependencyProperty TickFrequencyProperty =
            DependencyProperty.Register("TickFrequency", typeof(double), typeof(RangeSlider), new PropertyMetadata(1.0));

        public bool IsSnapToTickEnabled
        {
            get { return (bool)GetValue(IsSnapToTickEnabledProperty); }
            set { SetValue(IsSnapToTickEnabledProperty, value); }
        }
        public static readonly DependencyProperty IsSnapToTickEnabledProperty =
            DependencyProperty.Register("IsSnapToTickEnabled", typeof(bool), typeof(RangeSlider), new PropertyMetadata(false));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RangeSlider)d).UpdateVisuals();
        }

        private static object CoerceLowerValue(DependencyObject d, object baseValue)
        {
            var slider = (RangeSlider)d;
            double val = (double)baseValue;
            return Math.Max(slider.Minimum, Math.Min(val, slider.UpperValue));
        }

        private static object CoerceUpperValue(DependencyObject d, object baseValue)
        {
            var slider = (RangeSlider)d;
            double val = (double)baseValue;
            return Math.Max(slider.LowerValue, Math.Min(val, slider.Maximum));
        }

        private static void OnLowerValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RangeSlider)d).UpdateVisuals();
        }

        private static void OnUpperValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RangeSlider)d).UpdateVisuals();
        }

        private FrameworkElement? _track;
        private FrameworkElement? _selection;
        private Thumb? _lowerThumb;
        private Thumb? _upperThumb;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _track = GetTemplateChild("PART_Track") as FrameworkElement;
            _selection = GetTemplateChild("PART_Selection") as FrameworkElement;
            _lowerThumb = GetTemplateChild("PART_LowerThumb") as Thumb;
            _upperThumb = GetTemplateChild("PART_UpperThumb") as Thumb;

            if (_lowerThumb != null)
            {
                _lowerThumb.DragDelta += LowerThumb_DragDelta;
                _lowerThumb.MouseEnter += Thumb_MouseEnter;
            }
            if (_upperThumb != null)
            {
                _upperThumb.DragDelta += UpperThumb_DragDelta;
                _upperThumb.MouseEnter += Thumb_MouseEnter;
            }
            
            SizeChanged += (s, e) => UpdateVisuals();
            UpdateVisuals();
        }

        private void Thumb_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is UIElement element)
            {
                Panel.SetZIndex(element, 100);
                if (element == _lowerThumb && _upperThumb != null) Panel.SetZIndex(_upperThumb, 0);
                if (element == _upperThumb && _lowerThumb != null) Panel.SetZIndex(_lowerThumb, 0);
            }
        }

        private double SnapToTick(double value)
        {
            if (IsSnapToTickEnabled && TickFrequency > 0)
            {
                double result = Math.Round(value / TickFrequency) * TickFrequency;
                return Math.Max(Minimum, Math.Min(Maximum, result));
            }
            return value;
        }

        private void LowerThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (_track == null) return;
            double change = e.HorizontalChange;
            double width = _track.ActualWidth;
            if (width <= 0) return;

            double range = Maximum - Minimum;
            if (range <= 0) return;
            
            double changeVal = (change / width) * range;
            double newValue = LowerValue + changeVal;
            
            newValue = SnapToTick(newValue);
            
            // We set current value to allow binding updates but not destroy bindings
            SetCurrentValue(LowerValueProperty, Math.Min(Math.Max(Minimum, newValue), UpperValue));
        }

        private void UpperThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (_track == null) return;
            double change = e.HorizontalChange;
            double width = _track.ActualWidth;
            if (width <= 0) return;

            double range = Maximum - Minimum;
            if (range <= 0) return;

            double changeVal = (change / width) * range;
            double newValue = UpperValue + changeVal;
            
            newValue = SnapToTick(newValue);

            SetCurrentValue(UpperValueProperty, Math.Max(Math.Min(Maximum, newValue), LowerValue));
        }

        private void UpdateVisuals()
        {
            if (_track == null || _selection == null || _lowerThumb == null || _upperThumb == null) return;

            double width = _track.ActualWidth;
            if (width <= 0) return;

            double range = Maximum - Minimum;
            if (range <= 0) return;

            double lowerOffset = ((LowerValue - Minimum) / range) * width;
            double upperOffset = ((UpperValue - Minimum) / range) * width;

            Canvas.SetLeft(_lowerThumb, lowerOffset - (_lowerThumb.ActualWidth / 2));
            Canvas.SetLeft(_upperThumb, upperOffset - (_upperThumb.ActualWidth / 2));
            
            Canvas.SetLeft(_selection, lowerOffset);
            _selection.Width = Math.Max(0, upperOffset - lowerOffset);
        }
    }
}
