using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DongFeng.UI.Controls
{
    public enum DFTimelineLineType
    {
        Solid,
        Dashed,
        Dotted
    }

    public class DFTimeline : ItemsControl
    {
        static DFTimeline()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DFTimeline), new FrameworkPropertyMetadata(typeof(DFTimeline)));
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DFTimelineItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DFTimelineItem();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            UpdateItem(element as DFTimelineItem);
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            for (int i = 0; i < this.Items.Count; i++)
            {
                var container = this.ItemContainerGenerator.ContainerFromIndex(i) as DFTimelineItem;
                if (container != null)
                {
                    UpdateItem(container);
                }
            }
        }

        private void UpdateItem(DFTimelineItem? item)
        {
            if (item == null) return;
            int index = this.ItemContainerGenerator.IndexFromContainer(item);
            item.IsLast = index == this.Items.Count - 1;
        }
    }

    public class DFTimelineItem : ContentControl
    {
        static DFTimelineItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DFTimelineItem), new FrameworkPropertyMetadata(typeof(DFTimelineItem)));
        }

        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(DFTimelineItem), new PropertyMetadata(null));

        public Brush Color
        {
            get { return (Brush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(DFTimelineItem), new PropertyMetadata(null));

        public DFTimelineLineType LineType
        {
            get { return (DFTimelineLineType)GetValue(LineTypeProperty); }
            set { SetValue(LineTypeProperty, value); }
        }
        public static readonly DependencyProperty LineTypeProperty =
            DependencyProperty.Register("LineType", typeof(DFTimelineLineType), typeof(DFTimelineItem), new PropertyMetadata(DFTimelineLineType.Solid));

        public bool IsLast
        {
            get { return (bool)GetValue(IsLastProperty); }
            set { SetValue(IsLastProperty, value); }
        }
        public static readonly DependencyProperty IsLastProperty =
            DependencyProperty.Register("IsLast", typeof(bool), typeof(DFTimelineItem), new PropertyMetadata(false));
    }
}
