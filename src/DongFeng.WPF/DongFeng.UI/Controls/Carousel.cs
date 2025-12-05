using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace DongFeng.UI.Controls
{
    [TemplatePart(Name = ElementItemsPresenter, Type = typeof(ItemsPresenter))]
    [TemplatePart(Name = ElementPrevButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementNextButton, Type = typeof(Button))]
    public class Carousel : ItemsControl
    {
        private const string ElementItemsPresenter = "PART_ItemsPresenter";
        private const string ElementPrevButton = "PART_PrevButton";
        private const string ElementNextButton = "PART_NextButton";
        private DispatcherTimer? _autoPlayTimer;

        static Carousel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Carousel), new FrameworkPropertyMetadata(typeof(Carousel)));
        }

        public Carousel()
        {
            this.Loaded += Carousel_Loaded;
            this.Unloaded += Carousel_Unloaded;
        }

        private void Carousel_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTimer();
        }

        private void Carousel_Unloaded(object sender, RoutedEventArgs e)
        {
            StopTimer();
        }

        #region Properties

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Carousel), 
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedIndexChanged));

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var carousel = (Carousel)d;
            carousel.UpdateSelection((int)e.NewValue);
        }

        public bool AutoPlay
        {
            get { return (bool)GetValue(AutoPlayProperty); }
            set { SetValue(AutoPlayProperty, value); }
        }

        public static readonly DependencyProperty AutoPlayProperty =
            DependencyProperty.Register("AutoPlay", typeof(bool), typeof(Carousel), 
                new PropertyMetadata(false, OnAutoPlayChanged));

        private static void OnAutoPlayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Carousel)d).UpdateTimer();
        }

        public TimeSpan Interval
        {
            get { return (TimeSpan)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(TimeSpan), typeof(Carousel), 
                new PropertyMetadata(TimeSpan.FromSeconds(3), OnIntervalChanged));

        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Carousel)d).UpdateTimer();
        }

        #endregion

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is CarouselItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CarouselItem();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
            if (GetTemplateChild(ElementPrevButton) is Button prevBtn)
            {
                prevBtn.Click += (s, e) => Previous();
            }
            
            if (GetTemplateChild(ElementNextButton) is Button nextBtn)
            {
                nextBtn.Click += (s, e) => Next();
            }

            UpdateSelection(SelectedIndex);
        }

        private void UpdateSelection(int index)
        {
            if (Items.Count == 0) return;

            // Wrap around logic is handled by the changer, but SelectedIndex should be valid
            if (index < 0) index = 0;
            if (index >= Items.Count) index = Items.Count - 1;

            for (int i = 0; i < Items.Count; i++)
            {
                var container = ItemContainerGenerator.ContainerFromIndex(i) as CarouselItem;
                if (container != null)
                {
                    container.IsSelected = (i == index);
                }
            }
        }

        private void UpdateTimer()
        {
            StopTimer();
            if (AutoPlay && Interval > TimeSpan.Zero)
            {
                _autoPlayTimer = new DispatcherTimer();
                _autoPlayTimer.Interval = Interval;
                _autoPlayTimer.Tick += _autoPlayTimer_Tick;
                _autoPlayTimer.Start();
            }
        }

        private void StopTimer()
        {
            if (_autoPlayTimer != null)
            {
                _autoPlayTimer.Stop();
                _autoPlayTimer.Tick -= _autoPlayTimer_Tick;
                _autoPlayTimer = null;
            }
        }

        private void _autoPlayTimer_Tick(object? sender, EventArgs e)
        {
            if (Items.Count == 0) return;
            int nextIndex = SelectedIndex + 1;
            if (nextIndex >= Items.Count) nextIndex = 0;
            SelectedIndex = nextIndex;
        }

        // Command methods for buttons
        public void Next()
        {
            if (Items.Count == 0) return;
            int nextIndex = SelectedIndex + 1;
            if (nextIndex >= Items.Count) nextIndex = 0;
            SelectedIndex = nextIndex;
            ResetTimer(); // Reset timer on manual interaction
        }

        public void Previous()
        {
            if (Items.Count == 0) return;
            int prevIndex = SelectedIndex - 1;
            if (prevIndex < 0) prevIndex = Items.Count - 1;
            SelectedIndex = prevIndex;
            ResetTimer();
        }

        private void ResetTimer()
        {
            if (AutoPlay)
            {
                UpdateTimer();
            }
        }
    }

    public class CarouselItem : ContentControl
    {
        static CarouselItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CarouselItem), new FrameworkPropertyMetadata(typeof(CarouselItem)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            // Forward click event from item to parent or handle custom ItemClick?
            // Standard WPF ItemsControl doesn't have ItemClick, usually managed by content.
            // But for Carousel, maybe clicking an image triggers navigation?
            this.MouseLeftButtonUp += CarouselItem_MouseLeftButtonUp;
        }

        private void CarouselItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var parent = ItemsControl.ItemsControlFromItemContainer(this) as Carousel;
            // We could expose an ItemClick event on Carousel if needed.
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(CarouselItem), new PropertyMetadata(false));
    }
}
