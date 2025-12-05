using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DongFeng.UI.Controls
{
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_IncreaseButton", Type = typeof(System.Windows.Controls.Primitives.ButtonBase))]
    [TemplatePart(Name = "PART_DecreaseButton", Type = typeof(System.Windows.Controls.Primitives.ButtonBase))]
    public class DFNumericUpDown : Control
    {
        static DFNumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DFNumericUpDown), new FrameworkPropertyMetadata(typeof(DFNumericUpDown)));
        }

        public DFNumericUpDown()
        {
        }

        private TextBox _textBox;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _textBox = GetTemplateChild("PART_TextBox") as TextBox;
            if (_textBox != null)
            {
                _textBox.LostFocus += TextBox_LostFocus;
                _textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
                _textBox.TextChanged += TextBox_TextChanged;
            }

            if (GetTemplateChild("PART_IncreaseButton") is System.Windows.Controls.Primitives.ButtonBase upBtn)
                upBtn.Click += (s, e) => Value = Math.Min(Maximum, Value + Increment);

            if (GetTemplateChild("PART_DecreaseButton") is System.Windows.Controls.Primitives.ButtonBase downBtn)
                downBtn.Click += (s, e) => Value = Math.Max(Minimum, Value - Increment);
            
            UpdateText();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_textBox == null) return;
            // Optional: Real-time validation
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CommitInput();
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                Value = Math.Min(Maximum, Value + Increment);
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                Value = Math.Max(Minimum, Value - Increment);
                e.Handled = true;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            CommitInput();
        }

        private void CommitInput()
        {
            if (_textBox == null) return;
            if (double.TryParse(_textBox.Text, out double val))
            {
                val = Math.Max(Minimum, Math.Min(Maximum, val));
                Value = val;
            }
            UpdateText();
        }

        private void UpdateText()
        {
            if (_textBox != null)
                _textBox.Text = Value.ToString(StringFormat);
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(DFNumericUpDown), 
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DFNumericUpDown)d).UpdateText();
        }

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(DFNumericUpDown), new PropertyMetadata(0.0));

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(DFNumericUpDown), new PropertyMetadata(100.0));

        public double Increment
        {
            get { return (double)GetValue(IncrementProperty); }
            set { SetValue(IncrementProperty, value); }
        }
        public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register("Increment", typeof(double), typeof(DFNumericUpDown), new PropertyMetadata(1.0));

        public string StringFormat
        {
            get { return (string)GetValue(StringFormatProperty); }
            set { SetValue(StringFormatProperty, value); }
        }
        public static readonly DependencyProperty StringFormatProperty = DependencyProperty.Register("StringFormat", typeof(string), typeof(DFNumericUpDown), new PropertyMetadata("F0", (d,e)=> ((DFNumericUpDown)d).UpdateText()));
    }
}

