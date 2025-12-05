using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DongFeng.UI.Controls
{
    [TemplatePart(Name = ElementPath, Type = typeof(Path))]
    [TemplatePart(Name = ElementBackgroundPath, Type = typeof(Path))]
    public class CircleProgressBar : ProgressBar
    {
        private const string ElementPath = "PART_IndicatorPath";
        private const string ElementBackgroundPath = "PART_BackgroundPath";
        private Path? _path;
        private Path? _bgPath;

        static CircleProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircleProgressBar), new FrameworkPropertyMetadata(typeof(CircleProgressBar)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _path = GetTemplateChild(ElementPath) as Path;
            _bgPath = GetTemplateChild(ElementBackgroundPath) as Path;
            UpdatePath();
            UpdateIndeterminate();
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            UpdatePath();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdatePath();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == IsIndeterminateProperty)
            {
                UpdateIndeterminate();
            }
        }

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(CircleProgressBar), 
                new PropertyMetadata(10.0, OnStrokeThicknessChanged));

        public bool IsPie
        {
            get { return (bool)GetValue(IsPieProperty); }
            set { SetValue(IsPieProperty, value); }
        }

        public static readonly DependencyProperty IsPieProperty =
            DependencyProperty.Register("IsPie", typeof(bool), typeof(CircleProgressBar), 
                new PropertyMetadata(false, OnIsPieChanged));

        private static void OnStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CircleProgressBar)d).UpdatePath();
        }

        private static void OnIsPieChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
             ((CircleProgressBar)d).UpdatePath();
        }

        private void UpdateIndeterminate()
        {
            if (_path == null) return;

            if (IsIndeterminate)
            {
                // Start rotation animation
                DoubleAnimation animation = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromSeconds(1)));
                animation.RepeatBehavior = RepeatBehavior.Forever;
                RotateTransform rotate = new RotateTransform();
                _path.RenderTransform = rotate;
                _path.RenderTransformOrigin = new Point(0.5, 0.5);
                rotate.BeginAnimation(RotateTransform.AngleProperty, animation);
                
                UpdatePath(isIndeterminate: true);
            }
            else
            {
                _path.RenderTransform = null;
                UpdatePath();
            }
        }

        private void UpdatePath(bool isIndeterminate = false)
        {
            double diameter = Math.Min(ActualWidth, ActualHeight);
            if (diameter <= 0) return;

            double thickness = StrokeThickness;
            
            // If IsPie is true, radius is full radius (diameter/2)
            // If Ring, radius is (diameter - thickness)/2
            double radius = IsPie ? diameter / 2 : (diameter - thickness) / 2;
            if(radius <= 0) radius = 0;

            Point center = new Point(ActualWidth / 2, ActualHeight / 2);

            // Update Background Path
            if (_bgPath != null)
            {
                _bgPath.StrokeThickness = IsPie ? 0 : thickness;
                _bgPath.Fill = IsPie ? Background : null; // Use Background property for fill if pie
                
                StreamGeometry bgGeometry = new StreamGeometry();
                using (StreamGeometryContext ctx = bgGeometry.Open())
                {
                    Point start = new Point(center.X + radius, center.Y);
                    Point mid = new Point(center.X - radius, center.Y);
                    
                    ctx.BeginFigure(start, true, true); // Filled if closed
                    ctx.ArcTo(mid, new Size(radius, radius), 0, false, SweepDirection.Clockwise, true, false);
                    ctx.ArcTo(start, new Size(radius, radius), 0, false, SweepDirection.Clockwise, true, false);
                }
                _bgPath.Data = bgGeometry;
            }

            if (_path == null) return;

            double min = Minimum;
            double max = Maximum;
            double val = Value;

            double percentage = (max <= min) ? 0 : (val - min) / (max - min);
            if (percentage > 1) percentage = 1;
            if (percentage < 0) percentage = 0;

            if (isIndeterminate)
            {
                percentage = 0.25; 
            }

            double angle = percentage * 360;
            bool isFullCircle = angle >= 360;
            if (isFullCircle) angle = 359.99;

            double angleRad = (angle - 90) * Math.PI / 180;
            double startAngleRad = -90 * Math.PI / 180;

            Point startPoint = new Point(
                center.X + radius * Math.Cos(startAngleRad),
                center.Y + radius * Math.Sin(startAngleRad));

            Point endPoint = new Point(
                center.X + radius * Math.Cos(angleRad),
                center.Y + radius * Math.Sin(angleRad));

            bool isLargeArc = angle > 180;

            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open())
            {
                if (IsPie)
                {
                    // Pie Logic: Start from Center -> Top Edge -> Arc -> Center
                    ctx.BeginFigure(center, true, true);
                    ctx.LineTo(startPoint, true, false);
                    ctx.ArcTo(endPoint, new Size(radius, radius), 0, isLargeArc, SweepDirection.Clockwise, true, false);
                    // LineTo center is implicit if closed
                }
                else
                {
                    // Ring Logic: Just the Arc stroke
                    ctx.BeginFigure(startPoint, false, false);
                    ctx.ArcTo(endPoint, new Size(radius, radius), 0, isLargeArc, SweepDirection.Clockwise, true, false);
                }
            }

            _path.Data = geometry;
            
            if (IsPie)
            {
                _path.Fill = Foreground;
                _path.StrokeThickness = 0;
            }
            else
            {
                _path.Fill = null;
                _path.StrokeThickness = thickness;
            }
        }
    }
}
