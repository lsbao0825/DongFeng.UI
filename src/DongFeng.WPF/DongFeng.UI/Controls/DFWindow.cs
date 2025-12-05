using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shell;

namespace DongFeng.UI.Controls
{
    [TemplatePart(Name = "PART_MessageContainer", Type = typeof(StackPanel))]
    [TemplatePart(Name = "PART_WindowLoading", Type = typeof(Loading))]
    public class DFWindow : Window
    {
        static DFWindow()
        {
            // Use Implicit Style from App.Resources instead of DefaultStyleKey to ensure stability
            // DefaultStyleKeyProperty.OverrideMetadata(typeof(DFWindow), new FrameworkPropertyMetadata(typeof(DFWindow)));
        }

        public DFWindow()
        {
            // Default setup
            this.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));

            // Enable System Rounded Corners (Win11+)
            Helpers.WindowRoundedCornerHelper.EnableRoundedCorners(this);
        }

        #region Dependency Properties

        // IsLoading
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(DFWindow), new PropertyMetadata(false));

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        // LoadingText
        public static readonly DependencyProperty LoadingTextProperty =
            DependencyProperty.Register("LoadingText", typeof(string), typeof(DFWindow), new PropertyMetadata("Loading..."));

        public string LoadingText
        {
            get { return (string)GetValue(LoadingTextProperty); }
            set { SetValue(LoadingTextProperty, value); }
        }

        // IsLoadingCancellable
        public static readonly DependencyProperty IsLoadingCancellableProperty =
            DependencyProperty.Register("IsLoadingCancellable", typeof(bool), typeof(DFWindow), new PropertyMetadata(false));

        public bool IsLoadingCancellable
        {
            get { return (bool)GetValue(IsLoadingCancellableProperty); }
            set { SetValue(IsLoadingCancellableProperty, value); }
        }

        // TitleBarContent
        public static readonly DependencyProperty TitleBarContentProperty =
            DependencyProperty.Register("TitleBarContent", typeof(object), typeof(DFWindow), new PropertyMetadata(null));

        public object TitleBarContent
        {
            get { return GetValue(TitleBarContentProperty); }
            set { SetValue(TitleBarContentProperty, value); }
        }

        // ExtendViewIntoTitleBar
        public static readonly DependencyProperty ExtendViewIntoTitleBarProperty =
            DependencyProperty.Register("ExtendViewIntoTitleBar", typeof(bool), typeof(DFWindow), new PropertyMetadata(false));

        public bool ExtendViewIntoTitleBar
        {
            get { return (bool)GetValue(ExtendViewIntoTitleBarProperty); }
            set { SetValue(ExtendViewIntoTitleBarProperty, value); }
        }

        // TitleBarHeight
        public static readonly DependencyProperty TitleBarHeightProperty =
            DependencyProperty.Register("TitleBarHeight", typeof(double), typeof(DFWindow), new PropertyMetadata(32.0));

        public double TitleBarHeight
        {
            get { return (double)GetValue(TitleBarHeightProperty); }
            set { SetValue(TitleBarHeightProperty, value); }
        }

        // ShowIcon
        public static readonly DependencyProperty ShowIconProperty =
            DependencyProperty.Register("ShowIcon", typeof(bool), typeof(DFWindow), new PropertyMetadata(true));

        public bool ShowIcon
        {
            get { return (bool)GetValue(ShowIconProperty); }
            set { SetValue(ShowIconProperty, value); }
        }

        // ShowTitle
        public static readonly DependencyProperty ShowTitleProperty =
            DependencyProperty.Register("ShowTitle", typeof(bool), typeof(DFWindow), new PropertyMetadata(true));

        public bool ShowTitle
        {
            get { return (bool)GetValue(ShowTitleProperty); }
            set { SetValue(ShowTitleProperty, value); }
        }

        // TitleBarBackground
        public static readonly DependencyProperty TitleBarBackgroundProperty =
            DependencyProperty.Register("TitleBarBackground", typeof(System.Windows.Media.Brush), typeof(DFWindow), new PropertyMetadata(null));

        public System.Windows.Media.Brush TitleBarBackground
        {
            get { return (System.Windows.Media.Brush)GetValue(TitleBarBackgroundProperty); }
            set { SetValue(TitleBarBackgroundProperty, value); }
        }

        #endregion

        #region Events

        public static readonly RoutedEvent LoadingCancelledEvent = EventManager.RegisterRoutedEvent(
            "LoadingCancelled", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DFWindow));

        public event RoutedEventHandler LoadingCancelled
        {
            add { AddHandler(LoadingCancelledEvent, value); }
            remove { RemoveHandler(LoadingCancelledEvent, value); }
        }

        #endregion

        #region Loading Helpers

        private CancellationTokenSource _activeLoadingCts;

        /// <summary>
        /// Starts loading, executes the async action, and stops loading automatically.
        /// </summary>
        public async Task StartLoading(Func<Task> action, string text = "Loading...", bool cancellable = false)
        {
            await StartLoading(async (_) => await action(), text, cancellable);
        }

        /// <summary>
        /// Starts loading, executes the async action with cancellation support, and stops loading automatically.
        /// </summary>
        public async Task StartLoading(Func<CancellationToken, Task> action, string text = "Loading...", bool cancellable = false)
        {
            // Prevent concurrent loading overrides if needed, or just overwrite.
            // For simplicity, we overwrite the current state.

            IsLoading = true;
            LoadingText = text;
            IsLoadingCancellable = cancellable;

            _activeLoadingCts = new CancellationTokenSource();

            // Handler to cancel the token when user clicks cancel
            RoutedEventHandler cancelHandler = (s, e) =>
            {
                _activeLoadingCts?.Cancel();
            };

            if (cancellable)
            {
                this.LoadingCancelled += cancelHandler;
            }

            try
            {
                await action(_activeLoadingCts.Token);
            }
            finally
            {
                if (cancellable)
                {
                    this.LoadingCancelled -= cancelHandler;
                }

                IsLoading = false;
                IsLoadingCancellable = false;
                _activeLoadingCts?.Dispose();
                _activeLoadingCts = null;
            }
        }

        /// <summary>
        /// Starts loading, executes the async function returning a value, and stops loading automatically.
        /// </summary>
        public async Task<T> StartLoading<T>(Func<Task<T>> action, string text = "Loading...", bool cancellable = false)
        {
            return await StartLoading(async (_) => await action(), text, cancellable);
        }

        /// <summary>
        /// Starts loading, executes the async function returning a value with cancellation support, and stops loading automatically.
        /// </summary>
        public async Task<T> StartLoading<T>(Func<CancellationToken, Task<T>> action, string text = "Loading...", bool cancellable = false)
        {
            IsLoading = true;
            LoadingText = text;
            IsLoadingCancellable = cancellable;

            _activeLoadingCts = new CancellationTokenSource();

            RoutedEventHandler cancelHandler = (s, e) =>
            {
                _activeLoadingCts?.Cancel();
            };

            if (cancellable)
            {
                this.LoadingCancelled += cancelHandler;
            }

            try
            {
                return await action(_activeLoadingCts.Token);
            }
            finally
            {
                if (cancellable)
                {
                    this.LoadingCancelled -= cancelHandler;
                }

                IsLoading = false;
                IsLoadingCancellable = false;
                _activeLoadingCts?.Dispose();
                _activeLoadingCts = null;
            }
        }

        #endregion

        #region Command Handlers

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _messageContainer = GetTemplateChild("PART_MessageContainer") as StackPanel;
            
            if (GetTemplateChild("PART_WindowLoading") is Loading loading)
            {
                loading.Cancel += (s, e) => RaiseEvent(new RoutedEventArgs(LoadingCancelledEvent));
            }
        }

        private StackPanel _messageContainer;

        public void ShowMessage(string content, MessageType type, TimeSpan duration)
        {
            if (_messageContainer == null) return;

            var message = new MessageItem
            {
                Content = content,
                Type = type,
                Duration = duration
            };

            _messageContainer.Children.Add(message);
        }

        private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode != ResizeMode.NoResize;
        }

        private void OnCloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void OnMaximizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void OnMinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void OnRestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        #endregion
    }
}
