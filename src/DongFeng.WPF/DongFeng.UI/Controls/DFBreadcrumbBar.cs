using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace DongFeng.UI.Controls
{
    public class DFBreadcrumbBar : ItemsControl
    {
        static DFBreadcrumbBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DFBreadcrumbBar), new FrameworkPropertyMetadata(typeof(DFBreadcrumbBar)));
        }

        public static readonly DependencyProperty SeparatorProperty =
            DependencyProperty.Register("Separator", typeof(object), typeof(DFBreadcrumbBar), new PropertyMetadata(" / "));

        public object Separator
        {
            get { return GetValue(SeparatorProperty); }
            set { SetValue(SeparatorProperty, value); }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new BreadcrumbItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is BreadcrumbItem;
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            RefreshSeparators();
        }

        private void RefreshSeparators()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (ItemContainerGenerator.ContainerFromIndex(i) is BreadcrumbItem item)
                {
                    item.ShowSeparator = (i < Items.Count - 1);
                    item.Separator = Separator;
                }
            }
        }
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            RefreshSeparators();
        }
    }

    public class BreadcrumbItem : ContentControl
    {
        static BreadcrumbItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadcrumbItem), new FrameworkPropertyMetadata(typeof(BreadcrumbItem)));
        }

        public static readonly DependencyProperty SeparatorProperty =
            DependencyProperty.Register("Separator", typeof(object), typeof(BreadcrumbItem), new PropertyMetadata(null));

        public object Separator
        {
            get { return GetValue(SeparatorProperty); }
            set { SetValue(SeparatorProperty, value); }
        }

        public static readonly DependencyProperty ShowSeparatorProperty =
            DependencyProperty.Register("ShowSeparator", typeof(bool), typeof(BreadcrumbItem), new PropertyMetadata(true));

        public bool ShowSeparator
        {
            get { return (bool)GetValue(ShowSeparatorProperty); }
            set { SetValue(ShowSeparatorProperty, value); }
        }
        
        // Command Support
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(System.Windows.Input.ICommand), typeof(BreadcrumbItem), new PropertyMetadata(null));

        public System.Windows.Input.ICommand Command
        {
            get { return (System.Windows.Input.ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(BreadcrumbItem), new PropertyMetadata(null));

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }
    }
}
