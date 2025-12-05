using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace DongFeng.UI.Controls
{
    public enum DFPopoverTrigger
    {
        Click,
        Hover
    }

    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    public class DFPopover : ContentControl
    {
        private Popup? _popup;
        private long _closedTimestamp;

        static DFPopover()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DFPopover), new FrameworkPropertyMetadata(typeof(DFPopover)));
        }

        public object Title
        {
            get { return GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(object), typeof(DFPopover), new PropertyMetadata(null));

        public object PopupContent
        {
            get { return GetValue(PopupContentProperty); }
            set { SetValue(PopupContentProperty, value); }
        }
        public static readonly DependencyProperty PopupContentProperty =
            DependencyProperty.Register("PopupContent", typeof(object), typeof(DFPopover), new PropertyMetadata(null));

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(DFPopover), new PropertyMetadata(false));

        public PlacementMode Placement
        {
            get { return (PlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register("Placement", typeof(PlacementMode), typeof(DFPopover), new PropertyMetadata(PlacementMode.Bottom));

        public DFPopoverTrigger Trigger
        {
            get { return (DFPopoverTrigger)GetValue(TriggerProperty); }
            set { SetValue(TriggerProperty, value); }
        }
        public static readonly DependencyProperty TriggerProperty =
            DependencyProperty.Register("Trigger", typeof(DFPopoverTrigger), typeof(DFPopover), new PropertyMetadata(DFPopoverTrigger.Click));

        public bool StaysOpen
        {
            get { return (bool)GetValue(StaysOpenProperty); }
            set { SetValue(StaysOpenProperty, value); }
        }
        public static readonly DependencyProperty StaysOpenProperty =
            DependencyProperty.Register("StaysOpen", typeof(bool), typeof(DFPopover), new PropertyMetadata(false));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _popup = GetTemplateChild("PART_Popup") as Popup;
            if (_popup != null)
            {
                _popup.Closed += Popup_Closed;
                _popup.Opened += Popup_Opened;
            }
        }

        private void Popup_Opened(object? sender, EventArgs e)
        {
            // Optional: Handle open logic
        }

        private void Popup_Closed(object? sender, EventArgs e)
        {
            _closedTimestamp = DateTime.Now.Ticks;
            // Ensure property is synced if closed via StaysOpen=False
            SetCurrentValue(IsOpenProperty, false);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            // Only toggle if trigger is Click
            if (Trigger == DFPopoverTrigger.Click)
            {
                // Check if we just closed (within 200ms)
                // If so, this click was likely the cause of the closure (clicking the anchor), so don't reopen.
                long now = DateTime.Now.Ticks;
                long diff = now - _closedTimestamp;
                TimeSpan elapsed = new TimeSpan(diff);

                if (elapsed.TotalMilliseconds > 200)
                {
                    SetCurrentValue(IsOpenProperty, !IsOpen);
                }
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (Trigger == DFPopoverTrigger.Hover)
            {
                SetCurrentValue(IsOpenProperty, true);
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (Trigger == DFPopoverTrigger.Hover)
            {
                // We need to check if mouse moved into the popup
                // This is tricky with pure WPF. Usually we use a timer to close, 
                // and clear timer if mouse enters popup.
                // For simplicity here, we'll just close.
                // Ideally: MouseLeave -> Delay -> Close. 
                // If MouseEnter Popup -> Cancel Close.
                // But Popup is separate visual tree.
                SetCurrentValue(IsOpenProperty, false);
            }
        }
    }
}
