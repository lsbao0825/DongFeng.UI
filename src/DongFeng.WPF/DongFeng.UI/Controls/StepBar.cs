using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System;
using System.Globalization;

namespace DongFeng.UI.Controls
{
    public class StepBar : ItemsControl
    {
        static StepBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StepBar), new FrameworkPropertyMetadata(typeof(StepBar)));
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is StepBarItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new StepBarItem();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (element is StepBarItem stepItem)
            {
                 // Determine index
                 int index = this.ItemContainerGenerator.IndexFromContainer(element);
                 stepItem.Index = index + 1;
                 stepItem.IsLast = index == this.Items.Count - 1;
                 stepItem.Click += StepItem_Click;
                 UpdateItemState(stepItem);
            }
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
            if (element is StepBarItem stepItem)
            {
                stepItem.Click -= StepItem_Click;
            }
        }

        private void StepItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is StepBarItem item)
            {
                StepIndex = item.Index - 1;
            }
        }


        public int StepIndex
        {
            get { return (int)GetValue(StepIndexProperty); }
            set { SetValue(StepIndexProperty, value); }
        }
        public static readonly DependencyProperty StepIndexProperty =
            DependencyProperty.Register("StepIndex", typeof(int), typeof(StepBar), 
                new PropertyMetadata(0, OnStepIndexChanged));

        private static void OnStepIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StepBar)d).UpdateAllItems();
        }

        private void UpdateAllItems()
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                var container = this.ItemContainerGenerator.ContainerFromIndex(i) as StepBarItem;
                if (container != null)
                {
                    container.IsLast = (i == this.Items.Count - 1);
                    UpdateItemState(container);
                }
            }
        }

        private void UpdateItemState(StepBarItem item)
        {
            int index = this.ItemContainerGenerator.IndexFromContainer(item);
            item.Index = index + 1;
            
            if (index < StepIndex)
            {
                item.Status = StepStatus.Complete;
            }
            else if (index == StepIndex)
            {
                item.Status = StepStatus.Active;
            }
            else
            {
                item.Status = StepStatus.Pending;
            }
        }
    }

    public class StepBarItem : ContentControl
    {
        static StepBarItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StepBarItem), new FrameworkPropertyMetadata(typeof(StepBarItem)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            // Attach click handler if template has a button or we make the whole control clickable
            // For simplicity, let's handle MouseLeftButtonDown on the control itself
            this.MouseLeftButtonDown += (s, e) => RaiseEvent(new RoutedEventArgs(ClickEvent));
        }

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(StepBarItem));

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(StepBarItem), new PropertyMetadata(0));

        public bool IsLast
        {
            get { return (bool)GetValue(IsLastProperty); }
            set { SetValue(IsLastProperty, value); }
        }
        public static readonly DependencyProperty IsLastProperty =
            DependencyProperty.Register("IsLast", typeof(bool), typeof(StepBarItem), new PropertyMetadata(false));

        public StepStatus Status
        {
            get { return (StepStatus)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(StepStatus), typeof(StepBarItem), new PropertyMetadata(StepStatus.Pending));
    }

    public enum StepStatus
    {
        Pending,
        Active,
        Complete
    }
}
