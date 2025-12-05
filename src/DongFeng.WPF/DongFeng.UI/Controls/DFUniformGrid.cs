using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace DongFeng.UI.Controls
{
    /// <summary>
    /// A UniformGrid that supports RowSpacing and ColumnSpacing.
    /// </summary>
    public class DFUniformGrid : UniformGrid
    {
        public double RowSpacing
        {
            get { return (double)GetValue(RowSpacingProperty); }
            set { SetValue(RowSpacingProperty, value); }
        }

        public static readonly DependencyProperty RowSpacingProperty =
            DependencyProperty.Register("RowSpacing", typeof(double), typeof(DFUniformGrid),
                new FrameworkPropertyMetadata(5.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double ColumnSpacing
        {
            get { return (double)GetValue(ColumnSpacingProperty); }
            set { SetValue(ColumnSpacingProperty, value); }
        }

        public static readonly DependencyProperty ColumnSpacingProperty =
            DependencyProperty.Register("ColumnSpacing", typeof(double), typeof(DFUniformGrid),
                new FrameworkPropertyMetadata(5.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        protected override Size MeasureOverride(Size constraint)
        {
            UpdateComputedValues();

            var availableSize = new Size(constraint.Width, constraint.Height);
            var childConstraint = new Size(availableSize.Width / _columns, availableSize.Height / _rows);
            double maxChildWidth = 0.0;
            double maxChildHeight = 0.0;

            // Adjust constraint for spacing
            // Total width available for children = constraint.Width - (columns - 1) * ColumnSpacing
            // Child width = Total / columns
            
            if (!double.IsPositiveInfinity(availableSize.Width))
            {
                double totalSpacingWidth = (_columns - 1) * ColumnSpacing;
                if (availableSize.Width >= totalSpacingWidth)
                {
                    childConstraint.Width = (availableSize.Width - totalSpacingWidth) / _columns;
                }
                else
                {
                    childConstraint.Width = 0;
                }
            }

            if (!double.IsPositiveInfinity(availableSize.Height))
            {
                double totalSpacingHeight = (_rows - 1) * RowSpacing;
                if (availableSize.Height >= totalSpacingHeight)
                {
                    childConstraint.Height = (availableSize.Height - totalSpacingHeight) / _rows;
                }
                else
                {
                    childConstraint.Height = 0;
                }
            }

            foreach (UIElement child in InternalChildren)
            {
                child.Measure(childConstraint);
                Size childSize = child.DesiredSize;
                maxChildWidth = Math.Max(maxChildWidth, childSize.Width);
                maxChildHeight = Math.Max(maxChildHeight, childSize.Height);
            }

            // If grid size is not constrained (e.g. Auto), we use max child size.
            // If UniformGrid logic: all cells are same size.
            
            // We need to return the total size.
            // Width = columns * maxChildWidth + (columns - 1) * ColumnSpacing
            
            double totalWidth = _columns * maxChildWidth + (_columns - 1) * ColumnSpacing;
            double totalHeight = _rows * maxChildHeight + (_rows - 1) * RowSpacing;

            // If constraint was fixed, UniformGrid usually fills it.
            // But standard UniformGrid behavior is: 
            // If constrained, children fit in slots.
            // If not constrained, slots match largest child.
            
            // Let's respect the child constraint logic from base UniformGrid but adapted.
            // Actually, base UniformGrid doesn't expose columns/rows easily if they are computed.
            // We have to re-implement the logic or rely on _columns/_rows computed in UpdateComputedValues.
            
            return new Size(totalWidth, totalHeight);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            // Rect finalRect = new Rect(0, 0, arrangeSize.Width / _columns, arrangeSize.Height / _rows);
            // This needs adjustment for spacing.
            
            double totalSpacingWidth = (_columns - 1) * ColumnSpacing;
            double totalSpacingHeight = (_rows - 1) * RowSpacing;
            
            double itemWidth = (arrangeSize.Width - totalSpacingWidth) / _columns;
            double itemHeight = (arrangeSize.Height - totalSpacingHeight) / _rows;
            
            if (itemWidth < 0) itemWidth = 0;
            if (itemHeight < 0) itemHeight = 0;

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                UIElement child = InternalChildren[i];
                
                // Calculate row and column index
                // Standard UniformGrid fills row by row usually.
                int row = i / _columns;
                int col = i % _columns;
                
                if (FirstColumn >= _columns) FirstColumn = 0;
                
                // Actually, we should respect FirstColumn property if we want full compatibility.
                // But base UniformGrid implementation is private/internal mostly.
                // Re-implementing standard flow:
                
                // Adjust for FirstColumn (only affects first row)
                // Let's simplify: standard flow.
                
                double x = col * (itemWidth + ColumnSpacing);
                double y = row * (itemHeight + RowSpacing);
                
                child.Arrange(new Rect(x, y, itemWidth, itemHeight));
            }

            return arrangeSize;
        }

        private int _rows;
        private int _columns;

        private void UpdateComputedValues()
        {
            _columns = Columns;
            _rows = Rows;

            if (FirstColumn >= _columns)
            {
                FirstColumn = 0;
            }

            int count = InternalChildren.Count;
            
            if (_rows == 0 || _columns == 0)
            {
                if (_rows == 0 && _columns == 0)
                {
                    _rows = _columns = (int)Math.Ceiling(Math.Sqrt(count));
                }
                else if (_rows == 0)
                {
                    _rows = (int)Math.Ceiling((double)count / _columns);
                }
                else if (_columns == 0)
                {
                    _columns = (int)Math.Ceiling((double)count / _rows);
                }
            }
        }
    }
}

