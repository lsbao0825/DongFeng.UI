using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace DongFeng.UI.Controls
{
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_Button", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Calendar", Type = typeof(Calendar))]
    [TemplatePart(Name = "PART_HourList", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_MinuteList", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_SecondList", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_OkButton", Type = typeof(Button))]
    public class DateTimePicker : Control
    {
        static DateTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(typeof(DateTimePicker)));
        }

        private Popup _popup;
        private Calendar _calendar;
        private ListBox _hourList;
        private ListBox _minuteList;
        private ListBox _secondList;
        private bool _isInternalChange;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _popup = GetTemplateChild("PART_Popup") as Popup;
            if (GetTemplateChild("PART_Button") is Button btn)
            {
                btn.Click += (s, e) => IsDropDownOpen = !IsDropDownOpen;
            }

            _calendar = GetTemplateChild("PART_Calendar") as Calendar;
            if (_calendar != null)
            {
                _calendar.SelectedDatesChanged += OnCalendarSelectedDateChanged;
                _calendar.DisplayDateChanged += (s, e) => DisplayDate = _calendar.DisplayDate;
            }

            _hourList = SetupTimeList("PART_HourList", 0, 23, (v) => UpdateTime(h: v));
            _minuteList = SetupTimeList("PART_MinuteList", 0, 59, (v) => UpdateTime(m: v));
            _secondList = SetupTimeList("PART_SecondList", 0, 59, (v) => UpdateTime(s: v));
            
            if (GetTemplateChild("PART_OkButton") is Button okBtn)
            {
                okBtn.Click += (s, e) => IsDropDownOpen = false;
            }

            UpdateVisuals();
        }

        private ListBox SetupTimeList(string name, int min, int max, Action<int> updateAction)
        {
            var list = GetTemplateChild(name) as ListBox;
            if (list != null)
            {
                for (int i = min; i <= max; i++) list.Items.Add(i.ToString("00"));
                list.SelectionChanged += (s, e) =>
                {
                    if (_isInternalChange) return;
                    if (list.SelectedItem != null && int.TryParse(list.SelectedItem.ToString(), out int val))
                    {
                        updateAction(val);
                    }
                };
            }
            return list;
        }

        private void OnCalendarSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInternalChange) return;
            if (_calendar.SelectedDate.HasValue)
            {
                var current = SelectedDate ?? DateTime.Now;
                var date = _calendar.SelectedDate.Value;
                SelectedDate = new DateTime(date.Year, date.Month, date.Day, current.Hour, current.Minute, current.Second);
            }
        }

        private void UpdateTime(int? h = null, int? m = null, int? s = null)
        {
            var current = SelectedDate ?? DateTime.Now;
            SelectedDate = new DateTime(
                current.Year, current.Month, current.Day, 
                h ?? current.Hour, m ?? current.Minute, s ?? current.Second);
        }

        private void UpdateVisuals()
        {
            _isInternalChange = true;
            if (SelectedDate.HasValue)
            {
                var date = SelectedDate.Value;
                if (_calendar != null)
                {
                    _calendar.SelectedDate = date.Date;
                    _calendar.DisplayDate = date.Date;
                }
                if (_hourList != null) _hourList.SelectedIndex = date.Hour;
                if (_minuteList != null) _minuteList.SelectedIndex = date.Minute;
                if (_secondList != null) _secondList.SelectedIndex = date.Second;
            }
            else
            {
                if (_calendar != null) _calendar.SelectedDate = null;
                if (_hourList != null) _hourList.SelectedIndex = -1;
                if (_minuteList != null) _minuteList.SelectedIndex = -1;
                if (_secondList != null) _secondList.SelectedIndex = -1;
            }
            _isInternalChange = false;
        }

        public DateTime? SelectedDate
        {
            get { return (DateTime?)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }
        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register("SelectedDate", typeof(DateTime?), typeof(DateTimePicker), 
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDateChanged));

        private static void OnDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimePicker)d).UpdateVisuals();
        }

        public DateTime DisplayDate
        {
            get { return (DateTime)GetValue(DisplayDateProperty); }
            set { SetValue(DisplayDateProperty, value); }
        }
        public static readonly DependencyProperty DisplayDateProperty =
            DependencyProperty.Register("DisplayDate", typeof(DateTime), typeof(DateTimePicker), new PropertyMetadata(DateTime.Now));

        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(DateTimePicker), new PropertyMetadata(false));

        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(string), typeof(DateTimePicker), new PropertyMetadata("yyyy-MM-dd HH:mm:ss"));
    }
}

