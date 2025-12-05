using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DongFeng.UI.Controls
{
    public enum TimelineLineType
    {
        Solid,
        Dashed,
        Dotted
    }

    public class Timeline : ItemsControl
    {
        static Timeline()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata(typeof(Timeline)));
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TimelineItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TimelineItem();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            UpdateItem(element as TimelineItem);
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            for (int i = 0; i < this.Items.Count; i++)
            {
                var container = this.ItemContainerGenerator.ContainerFromIndex(i) as TimelineItem;
                if (container != null)
                {
                    UpdateItem(container);
                }
            }
        }

        private void UpdateItem(TimelineItem? item)
        {
            if (item == null) return;
            int index = this.ItemContainerGenerator.IndexFromContainer(item);
            item.IsLast = index == this.Items.Count - 1;
        }
    }

    public class TimelineItem : ContentControl
    {
        static TimelineItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimelineItem), new FrameworkPropertyMetadata(typeof(TimelineItem)));
        }

        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(TimelineItem), new PropertyMetadata(null));

        public Brush Color
        {
            get { return (Brush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(TimelineItem), new PropertyMetadata(null));

        public TimelineLineType LineType
        {
            get { return (TimelineLineType)GetValue(LineTypeProperty); }
            set { SetValue(LineTypeProperty, value); }
        }
        public static readonly DependencyProperty LineTypeProperty =
            DependencyProperty.Register("LineType", typeof(TimelineLineType), typeof(TimelineItem), new PropertyMetadata(TimelineLineType.Solid));

        public bool IsLast
        {
            get { return (bool)GetValue(IsLastProperty); }
            set { SetValue(IsLastProperty, value); }
        }
        public static readonly DependencyProperty IsLastProperty =
            DependencyProperty.Register("IsLast", typeof(bool), typeof(TimelineItem), new PropertyMetadata(false));
    }
}
