using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DongFeng.UI.Helpers
{
    public class PasswordBoxAttach
    {
        #region IsMonitoring
        public static readonly DependencyProperty IsMonitoringProperty =
            DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(PasswordBoxAttach), new PropertyMetadata(false, OnIsMonitoringChanged));

        public static bool GetIsMonitoring(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMonitoringProperty);
        }

        public static void SetIsMonitoring(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMonitoringProperty, value);
        }

        private static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox pb)
            {
                if ((bool)e.NewValue)
                {
                    pb.PasswordChanged += Pb_PasswordChanged;
                }
                else
                {
                    pb.PasswordChanged -= Pb_PasswordChanged;
                }
            }
        }

        private static void Pb_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb)
            {
                SetHasText(pb, pb.Password.Length > 0);
                SetPassword(pb, pb.Password); // Sync password for reveal logic
            }
        }
        #endregion

        #region HasText
        public static readonly DependencyProperty HasTextProperty =
            DependencyProperty.RegisterAttached("HasText", typeof(bool), typeof(PasswordBoxAttach), new PropertyMetadata(false));

        public static bool GetHasText(DependencyObject obj)
        {
            return (bool)obj.GetValue(HasTextProperty);
        }

        public static void SetHasText(DependencyObject obj, bool value)
        {
            obj.SetValue(HasTextProperty, value);
        }
        #endregion

        #region Password (for Binding/Sync)
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordBoxAttach), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPasswordPropertyChanged));

        public static string GetPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject obj, string value)
        {
            obj.SetValue(PasswordProperty, value);
        }

        private static void OnPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
             // Optional: Sync back to PasswordBox if changed from VM
             // Caution: PasswordBox.Password is not bindable by default, this is a hack.
             // To avoid infinite loops, check if values differ.
             if(d is PasswordBox pb)
             {
                 if(pb.Password != (string)e.NewValue)
                 {
                     pb.Password = (string)e.NewValue ?? "";
                 }
             }
        }
        #endregion

        #region ShowEyeButton
        public static readonly DependencyProperty ShowEyeButtonProperty =
            DependencyProperty.RegisterAttached("ShowEyeButton", typeof(bool), typeof(PasswordBoxAttach), new PropertyMetadata(false));

        public static bool GetShowEyeButton(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowEyeButtonProperty);
        }

        public static void SetShowEyeButton(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowEyeButtonProperty, value);
        }
        #endregion

        #region IsPasswordRevealed
        public static readonly DependencyProperty IsPasswordRevealedProperty =
            DependencyProperty.RegisterAttached("IsPasswordRevealed", typeof(bool), typeof(PasswordBoxAttach), new PropertyMetadata(false));

        public static bool GetIsPasswordRevealed(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsPasswordRevealedProperty);
        }

        public static void SetIsPasswordRevealed(DependencyObject obj, bool value)
        {
            obj.SetValue(IsPasswordRevealedProperty, value);
        }
        #endregion
    }
}

