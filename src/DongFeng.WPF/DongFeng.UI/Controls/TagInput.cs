using DongFeng.UI.Helpers;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DongFeng.UI.Controls
{
    [TemplatePart(Name = "PART_ListBox", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_FocusBorder", Type = typeof(Border))]
    public class TagInput : Control
    {
        private ListBox _listBox;
        private Border _focusBorder;
        private TagInputPlaceholder _inputItem = new TagInputPlaceholder();
        private bool _isSyncing;

        static TagInput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TagInput), new FrameworkPropertyMetadata(typeof(TagInput)));
            
            CommandManager.RegisterClassCommandBinding(typeof(TagInput), 
                new CommandBinding(RemoveTagCommand, OnRemoveTagCommand));
            CommandManager.RegisterClassCommandBinding(typeof(TagInput), 
                new CommandBinding(AddTagCommand, OnAddTagCommand));
        }

        public TagInput()
        {
            DisplayItems = new ObservableCollection<object>();
            DisplayItems.Add(_inputItem);
        }

        #region Events

        public static readonly RoutedEvent TagAddedEvent = EventManager.RegisterRoutedEvent(
            "TagAdded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TagInput));

        public event RoutedEventHandler TagAdded
        {
            add { AddHandler(TagAddedEvent, value); }
            remove { RemoveHandler(TagAddedEvent, value); }
        }

        public static readonly RoutedEvent TagRemovedEvent = EventManager.RegisterRoutedEvent(
            "TagRemoved", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TagInput));

        public event RoutedEventHandler TagRemoved
        {
            add { AddHandler(TagRemovedEvent, value); }
            remove { RemoveHandler(TagRemovedEvent, value); }
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _listBox = GetTemplateChild("PART_ListBox") as ListBox;
            _focusBorder = GetTemplateChild("PART_FocusBorder") as Border;
            
            if (_focusBorder != null)
            {
                _focusBorder.MouseDown += OnBorderMouseDown;
            }
            
            if (_listBox != null)
            {
                _listBox.ItemsSource = DisplayItems;
                
                // Handle click on ListBox background (when it has items but some empty space)
                _listBox.PreviewMouseDown += (s, e) =>
                {
                     // If clicked directly on the listbox or one of its internal containers (like ScrollViewer/Grid/WrapPanel)
                     // but NOT on a ListBoxItem (which handles its own events or bubbles up)
                     // We want to catch "empty space" clicks.
                     
                     // Simple heuristic: if original source is not inside a ListBoxItem (except for the input one)
                     var source = e.OriginalSource as DependencyObject;
                     var itemContainer = FindParent<ListBoxItem>(source);
                     
                     // If we clicked on nothing or the input item itself, we are good.
                     // If we clicked on a tag, let the tag handle it (e.g. delete button).
                     
                     if (itemContainer == null || itemContainer.Content == _inputItem)
                     {
                         // Delay focus to ensure click is processed
                         Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new System.Action(FocusInput));
                     }
                };
            }
        }

        // Event handler for the Border in ControlTemplate
        private void OnBorderMouseDown(object sender, MouseButtonEventArgs e)
        {
            FocusInput();
        }

        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            if (parentObject is T parent) return parent;
            return FindParent<T>(parentObject);
        }

        private void FocusInput()
        {
             if (_listBox == null) return;
             
             var item = _listBox.ItemContainerGenerator.ContainerFromItem(_inputItem) as ListBoxItem;
             if (item != null)
             {
                 var textBox = FindVisualChild<TextBox>(item);
                 if (textBox != null)
                 {
                     textBox.Focus();
                     Keyboard.Focus(textBox);
                     
                     // Ensure event handler is attached only once
                     textBox.PreviewKeyDown -= OnInputKeyDown;
                     textBox.PreviewKeyDown += OnInputKeyDown;
                 }
             }
        }
        
        private void OnInputKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back && string.IsNullOrEmpty(InputText) && Tags != null && Tags.Count > 0)
            {
                // Remove the last tag if input is empty and backspace is pressed
                RemoveTag(Tags[Tags.Count - 1]);
            }
        }
        
        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj == null) return null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child is T t) return t;
                var result = FindVisualChild<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        #region Commands

        public static readonly RoutedUICommand RemoveTagCommand = new RoutedUICommand("Remove Tag", "RemoveTag", typeof(TagInput));
        public static readonly RoutedUICommand AddTagCommand = new RoutedUICommand("Add Tag", "AddTag", typeof(TagInput));

        private static void OnRemoveTagCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var control = sender as TagInput;
            if (control != null && e.Parameter != null)
            {
                control.RemoveTag(e.Parameter);
            }
        }

        private static void OnAddTagCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var control = sender as TagInput;
            control?.AddTag(control.InputText);
        }

        #endregion

        #region Dependency Properties

        public IList Tags
        {
            get { return (IList)GetValue(TagsProperty); }
            set { SetValue(TagsProperty, value); }
        }
        public static readonly DependencyProperty TagsProperty =
            DependencyProperty.Register("Tags", typeof(IList), typeof(TagInput), 
                new PropertyMetadata(null, OnTagsChanged));

        public string InputText
        {
            get { return (string)GetValue(InputTextProperty); }
            set { SetValue(InputTextProperty, value); }
        }
        public static readonly DependencyProperty InputTextProperty =
            DependencyProperty.Register("InputText", typeof(string), typeof(TagInput), 
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        #endregion

        #region Internal Properties
        
        public ObservableCollection<object> DisplayItems { get; }

        public object InputItem => _inputItem;

        #endregion

        private static void OnTagsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TagInput)d;
            control.SyncTags(e.OldValue as IList, e.NewValue as IList);
        }

        private void SyncTags(IList oldList, IList newList)
        {
            if (oldList is INotifyCollectionChanged oldNotify)
            {
                oldNotify.CollectionChanged -= OnTagsCollectionChanged;
            }
            if (newList is INotifyCollectionChanged newNotify)
            {
                newNotify.CollectionChanged += OnTagsCollectionChanged;
            }

            ReloadDisplayItems();
        }

        private void OnTagsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isSyncing) return;
            ReloadDisplayItems();
        }

        private void ReloadDisplayItems()
        {
            _isSyncing = true;
            DisplayItems.Clear();
            if (Tags != null)
            {
                foreach (var item in Tags)
                {
                    DisplayItems.Add(item);
                }
            }
            DisplayItems.Add(_inputItem);
            _isSyncing = false;
        }

        public void RemoveTag(object tag)
        {
            if (Tags != null && Tags.Contains(tag))
            {
                Tags.Remove(tag);
                RaiseEvent(new RoutedEventArgs(TagRemovedEvent, tag)); // Use RoutedEventArgs with Source/OriginalSource? Or Custom
                // Simpler: Just raise generic RoutedEventArgs
            }
        }

        public void AddTag(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            
            if (Tags != null)
            {
                Tags.Add(text);
                RaiseEvent(new RoutedEventArgs(TagAddedEvent, text));
            }
            InputText = string.Empty;
            
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new System.Action(() => 
            {
                FocusInput();
            }));
        }
    }

    public class TagInputPlaceholder { }

    public class TagInputTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TagTemplate { get; set; }
        public DataTemplate InputTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is TagInputPlaceholder)
                return InputTemplate;
            
            return TagTemplate;
        }
    }
}
