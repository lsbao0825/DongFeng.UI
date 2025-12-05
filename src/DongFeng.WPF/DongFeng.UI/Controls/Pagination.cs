using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DongFeng.UI.Controls
{
    [TemplatePart(Name = "PART_FirstPageButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_PreviousPageButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_NextPageButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_LastPageButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_PageInput", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_PageSizeComboBox", Type = typeof(ComboBox))]
    public class Pagination : Control
    {
        static Pagination()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pagination), new FrameworkPropertyMetadata(typeof(Pagination)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BindButton("PART_FirstPageButton", () => CurrentPage = 1);
            BindButton("PART_PreviousPageButton", () => CurrentPage = Math.Max(1, CurrentPage - 1));
            BindButton("PART_NextPageButton", () => CurrentPage = Math.Min(TotalPages, CurrentPage + 1));
            BindButton("PART_LastPageButton", () => CurrentPage = TotalPages);
            
            if (GetTemplateChild("PART_PageInput") is TextBox tb)
            {
                tb.KeyDown += (s, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        if (int.TryParse(tb.Text, out int p))
                        {
                            CurrentPage = Math.Max(1, Math.Min(TotalPages, p));
                        }
                        // Refresh text even if page didn't change effectively (e.g. entered 999 -> max page)
                        tb.Text = CurrentPage.ToString();
                    }
                };
                tb.LostFocus += (s, e) => tb.Text = CurrentPage.ToString();
            }

            if (GetTemplateChild("PART_PageSizeComboBox") is ComboBox cb)
            {
                cb.SelectionChanged += (s, e) =>
                {
                    if (cb.SelectedItem is int size)
                    {
                        PageSize = size;
                    }
                    else if (cb.SelectedItem is ComboBoxItem cbi && int.TryParse(cbi.Content?.ToString(), out int sizeVal))
                    {
                        PageSize = sizeVal;
                    }
                };
            }
        }

        private void BindButton(string name, Action action)
        {
            if (GetTemplateChild(name) is Button btn)
            {
                btn.Click += (s, e) => action();
            }
        }

        #region Properties

        public int CurrentPage
        {
            get { return (int)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register("CurrentPage", typeof(int), typeof(Pagination), 
                new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPageChanged));

        public int TotalCount
        {
            get { return (int)GetValue(TotalCountProperty); }
            set { SetValue(TotalCountProperty, value); }
        }
        public static readonly DependencyProperty TotalCountProperty =
            DependencyProperty.Register("TotalCount", typeof(int), typeof(Pagination), new PropertyMetadata(0, OnTotalChanged));

        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }
        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register("PageSize", typeof(int), typeof(Pagination), 
                new FrameworkPropertyMetadata(20, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTotalChanged));

        public int TotalPages
        {
            get { return (int)GetValue(TotalPagesProperty); }
            private set { SetValue(TotalPagesPropertyKey, value); }
        }
        private static readonly DependencyPropertyKey TotalPagesPropertyKey =
            DependencyProperty.RegisterReadOnly("TotalPages", typeof(int), typeof(Pagination), new PropertyMetadata(1));
        public static readonly DependencyProperty TotalPagesProperty = TotalPagesPropertyKey.DependencyProperty;

        #endregion

        #region Events

        public static readonly RoutedEvent PageChangedEvent = EventManager.RegisterRoutedEvent(
            "PageChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<int>), typeof(Pagination));

        public event RoutedPropertyChangedEventHandler<int> PageChanged
        {
            add { AddHandler(PageChangedEvent, value); }
            remove { RemoveHandler(PageChangedEvent, value); }
        }

        public static readonly RoutedEvent PageSizeChangedEvent = EventManager.RegisterRoutedEvent(
            "PageSizeChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<int>), typeof(Pagination));

        public event RoutedPropertyChangedEventHandler<int> PageSizeChanged
        {
            add { AddHandler(PageSizeChangedEvent, value); }
            remove { RemoveHandler(PageSizeChangedEvent, value); }
        }

        #endregion

        private static void OnPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (Pagination)d;
            control.RaiseEvent(new RoutedPropertyChangedEventArgs<int>((int)e.OldValue, (int)e.NewValue, PageChangedEvent));
        }

        private static void OnTotalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (Pagination)d;
            if (e.Property == PageSizeProperty)
            {
                control.RaiseEvent(new RoutedPropertyChangedEventArgs<int>((int)e.OldValue, (int)e.NewValue, PageSizeChangedEvent));
            }
            control.CalculateTotalPages();
        }

        private void CalculateTotalPages()
        {
            if (PageSize <= 0) PageSize = 20;
            TotalPages = (int)Math.Ceiling((double)TotalCount / PageSize);
            if (TotalPages < 1) TotalPages = 1;
            if (CurrentPage > TotalPages) CurrentPage = TotalPages;
        }
    }
}
