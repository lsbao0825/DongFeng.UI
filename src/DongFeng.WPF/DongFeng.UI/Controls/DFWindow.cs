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
    [TemplatePart(Name = "PART_WindowDFLoading", Type = typeof(DFLoading))]
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

        // IsDFLoading
        public static readonly DependencyProperty IsDFLoadingProperty =
            DependencyProperty.Register("IsDFLoading", typeof(bool), typeof(DFWindow), new PropertyMetadata(false));

        public bool IsDFLoading
        {
            get { return (bool)GetValue(IsDFLoadingProperty); }
            set { SetValue(IsDFLoadingProperty, value); }
        }

        // DFLoadingText
        public static readonly DependencyProperty DFLoadingTextProperty =
            DependencyProperty.Register("DFLoadingText", typeof(string), typeof(DFWindow), new PropertyMetadata("DFLoading..."));

        public string DFLoadingText
        {
            get { return (string)GetValue(DFLoadingTextProperty); }
            set { SetValue(DFLoadingTextProperty, value); }
        }

        // IsDFLoadingCancellable
        public static readonly DependencyProperty IsDFLoadingCancellableProperty =
            DependencyProperty.Register("IsDFLoadingCancellable", typeof(bool), typeof(DFWindow), new PropertyMetadata(false));

        public bool IsDFLoadingCancellable
        {
            get { return (bool)GetValue(IsDFLoadingCancellableProperty); }
            set { SetValue(IsDFLoadingCancellableProperty, value); }
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

        public static readonly RoutedEvent DFLoadingCancelledEvent = EventManager.RegisterRoutedEvent(
            "DFLoadingCancelled", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DFWindow));

        public event RoutedEventHandler DFLoadingCancelled
        {
            add { AddHandler(DFLoadingCancelledEvent, value); }
            remove { RemoveHandler(DFLoadingCancelledEvent, value); }
        }

        #endregion

        #region DFLoading Helpers

        private CancellationTokenSource _activeDFLoadingCts;

        /// <summary>
        /// Starts DFLoading, executes the async action, and stops DFLoading automatically.
        /// </summary>
        public async Task StartDFLoading(Func<Task> action, string text = "DFLoading...", bool cancellable = false)
        {
            await StartDFLoading(async (_) => await action(), text, cancellable);
        }

        /// <summary>
        /// Starts DFLoading, executes the async action with cancellation support, and stops DFLoading automatically.
        /// </summary>
        public async Task StartDFLoading(Func<CancellationToken, Task> action, string text = "DFLoading...", bool cancellable = false)
        {
            // Prevent concurrent DFLoading overrides if needed, or just overwrite.
            // For simplicity, we overwrite the current state.

            IsDFLoading = true;
            DFLoadingText = text;
            IsDFLoadingCancellable = cancellable;

            _activeDFLoadingCts = new CancellationTokenSource();

            // Handler to cancel the token when user clicks cancel
            RoutedEventHandler cancelHandler = (s, e) =>
            {
                _activeDFLoadingCts?.Cancel();
            };

            if (cancellable)
            {
                this.DFLoadingCancelled += cancelHandler;
            }

            try
            {
                await action(_activeDFLoadingCts.Token);
            }
            finally
            {
                if (cancellable)
                {
                    this.DFLoadingCancelled -= cancelHandler;
                }

                IsDFLoading = false;
                IsDFLoadingCancellable = false;
                _activeDFLoadingCts?.Dispose();
                _activeDFLoadingCts = null;
            }
        }

        /// <summary>
        /// Starts DFLoading, executes the async function returning a value, and stops DFLoading automatically.
        /// </summary>
        public async Task<T> StartDFLoading<T>(Func<Task<T>> action, string text = "DFLoading...", bool cancellable = false)
        {
            return await StartDFLoading(async (_) => await action(), text, cancellable);
        }

        /// <summary>
        /// Starts DFLoading, executes the async function returning a value with cancellation support, and stops DFLoading automatically.
        /// </summary>
        public async Task<T> StartDFLoading<T>(Func<CancellationToken, Task<T>> action, string text = "DFLoading...", bool cancellable = false)
        {
            IsDFLoading = true;
            DFLoadingText = text;
            IsDFLoadingCancellable = cancellable;

            _activeDFLoadingCts = new CancellationTokenSource();

            RoutedEventHandler cancelHandler = (s, e) =>
            {
                _activeDFLoadingCts?.Cancel();
            };

            if (cancellable)
            {
                this.DFLoadingCancelled += cancelHandler;
            }

            try
            {
                return await action(_activeDFLoadingCts.Token);
            }
            finally
            {
                if (cancellable)
                {
                    this.DFLoadingCancelled -= cancelHandler;
                }

                IsDFLoading = false;
                IsDFLoadingCancellable = false;
                _activeDFLoadingCts?.Dispose();
                _activeDFLoadingCts = null;
            }
        }

        #endregion

        #region Command Handlers

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _messageContainer = GetTemplateChild("PART_MessageContainer") as StackPanel;
            
            if (GetTemplateChild("PART_WindowDFLoading") is DFLoading DFLoading)
            {
                DFLoading.Cancel += (s, e) => RaiseEvent(new RoutedEventArgs(DFLoadingCancelledEvent));
            }
        }

        private StackPanel _messageContainer;

        public void ShowMessage(string content, DFMessageType type, TimeSpan duration)
        {
            if (_messageContainer == null) return;

            var message = new DFMessageItem
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
