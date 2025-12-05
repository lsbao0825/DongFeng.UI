using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DongFeng.UI.Helpers;

namespace DongFeng.UI.Controls
{
    [TemplatePart(Name = "PART_SpectrumSlider", Type = typeof(Slider))]
    [TemplatePart(Name = "PART_AlphaSlider", Type = typeof(Slider))]
    [TemplatePart(Name = "PART_ColorCanvas", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_ColorThumb", Type = typeof(FrameworkElement))]
    public class DFColorPicker : Control
    {
        static DFColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DFColorPicker), new FrameworkPropertyMetadata(typeof(DFColorPicker)));
        }

        private Canvas _colorCanvas;
        private FrameworkElement _colorThumb;
        private bool _isUpdating;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_ColorCanvas") is Canvas canvas)
            {
                _colorCanvas = canvas;
                _colorCanvas.MouseLeftButtonDown += OnCanvasMouseDown;
                _colorCanvas.MouseMove += OnCanvasMouseMove;
                _colorCanvas.MouseLeftButtonUp += OnCanvasMouseUp;
                _colorCanvas.SizeChanged += (s, e) => UpdateThumbPosition();
            }

            _colorThumb = GetTemplateChild("PART_ColorThumb") as FrameworkElement;

            if (GetTemplateChild("PART_SpectrumSlider") is Slider spectrum)
            {
                spectrum.ValueChanged += (s, e) => 
                {
                     if (_isUpdating) return;
                     UpdateColorFromHsv();
                };
            }

            if (GetTemplateChild("PART_AlphaSlider") is Slider alphaSlider)
            {
                alphaSlider.ValueChanged += (s, e) =>
                {
                    if (_isUpdating) return;
                    // Direct update color alpha
                    var c = Color;
                    c.A = (byte)e.NewValue;
                    Color = c;
                };
            }
            
            UpdateThumbPosition();
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(DFColorPicker), 
                new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnColorChanged));

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register("Hue", typeof(double), typeof(DFColorPicker), new PropertyMetadata(0.0, OnHsvChanged));
        
        public double Hue
        {
            get { return (double)GetValue(HueProperty); }
            set { SetValue(HueProperty, value); }
        }

        public static readonly DependencyProperty SaturationProperty =
            DependencyProperty.Register("Saturation", typeof(double), typeof(DFColorPicker), new PropertyMetadata(0.0, OnHsvChanged));

        public double Saturation
        {
            get { return (double)GetValue(SaturationProperty); }
            set { SetValue(SaturationProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(DFColorPicker), new PropertyMetadata(0.0, OnHsvChanged));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        
        public static readonly DependencyProperty AProperty =
            DependencyProperty.Register("A", typeof(byte), typeof(DFColorPicker), new PropertyMetadata((byte)255, OnAlphaChanged));

        public byte A
        {
            get { return (byte)GetValue(AProperty); }
            set { SetValue(AProperty, value); }
        }

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DFColorPicker)d).OnColorChanged((Color)e.NewValue);
        }

        private void OnColorChanged(Color newColor)
        {
            if (_isUpdating) return;
            _isUpdating = true;

            // Update HSV
            var hsv = ColorHelper.ColorToHsv(newColor);
            Hue = hsv.H;
            Saturation = hsv.S;
            Value = hsv.V;
            A = newColor.A;
            
            UpdateThumbPosition();
            _isUpdating = false;
        }

        private static void OnAlphaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cp = (DFColorPicker)d;
            if (cp._isUpdating) return;
            
            var c = cp.Color;
            c.A = (byte)e.NewValue;
            cp.Color = c;
        }

        private static void OnHsvChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DFColorPicker)d).UpdateColorFromHsv();
        }

        private void UpdateColorFromHsv()
        {
            if (_isUpdating) return;
            _isUpdating = true;
            var c = ColorHelper.HsvToColor(Hue, Saturation, Value);
            c.A = A; // Preserve current Alpha
            Color = c;
            
            UpdateThumbPosition();
            _isUpdating = false;
        }

        private void OnCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            _colorCanvas.CaptureMouse();
            UpdateSVSomMouse(e.GetPosition(_colorCanvas));
        }

        private void OnCanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (_colorCanvas.IsMouseCaptured)
            {
                UpdateSVSomMouse(e.GetPosition(_colorCanvas));
            }
        }

        private void OnCanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            _colorCanvas.ReleaseMouseCapture();
        }

        private void UpdateSVSomMouse(Point p)
        {
            double width = _colorCanvas.ActualWidth;
            double height = _colorCanvas.ActualHeight;

            double x = Math.Max(0, Math.Min(width, p.X));
            double y = Math.Max(0, Math.Min(height, p.Y));

            Saturation = x / width;
            Value = 1 - (y / height);
        }

        private void UpdateThumbPosition()
        {
            if (_colorCanvas == null || _colorThumb == null) return;

            double x = Saturation * _colorCanvas.ActualWidth;
            double y = (1 - Value) * _colorCanvas.ActualHeight;

            Canvas.SetLeft(_colorThumb, x - _colorThumb.ActualWidth / 2);
            Canvas.SetTop(_colorThumb, y - _colorThumb.ActualHeight / 2);
        }
    }
}
