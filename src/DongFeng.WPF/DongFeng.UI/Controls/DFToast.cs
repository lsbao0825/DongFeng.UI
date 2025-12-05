using System;
using System.Linq;
using System.Windows;

namespace DongFeng.UI.Controls
{
    public static class DFToast
    {
        public static void Show(string content, DFMessageType type = DFMessageType.Info)
        {
            Show(content, type, TimeSpan.FromSeconds(3));
        }

        public static void Show(string content, DFMessageType type, TimeSpan duration)
        {
            if (Application.Current == null) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var window = GetActiveWindow();
                if (window != null)
                {
                    window.ShowMessage(content, type, duration);
                }
            });
        }
        
        public static void Info(string content) => Show(content, DFMessageType.Info);
        public static void Success(string content) => Show(content, DFMessageType.Success);
        public static void Warning(string content) => Show(content, DFMessageType.Warning);
        public static void Error(string content) => Show(content, DFMessageType.Error);

        private static DFWindow GetActiveWindow()
        {
            var active = Application.Current.Windows.OfType<DFWindow>().SingleOrDefault(x => x.IsActive);
            if (active == null)
            {
                active = Application.Current.MainWindow as DFWindow;
            }
            if (active == null && Application.Current.Windows.Count > 0)
            {
                 active = Application.Current.Windows.OfType<DFWindow>().LastOrDefault();
            }
            return active;
        }
    }
}

