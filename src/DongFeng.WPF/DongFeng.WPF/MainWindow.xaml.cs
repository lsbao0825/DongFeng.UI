using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DongFeng.UI.Controls;
using DongFeng.UI.Helpers;

namespace DongFeng
{
    public class UserData
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
    }

    public partial class MainWindow : DFWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            
            var users = new List<UserData>
            {
                new UserData { ID = "001", Name = "Alice", Status = "Active", Date = "2023-10-01" },
                new UserData { ID = "002", Name = "Bob", Status = "Inactive", Date = "2023-10-02" },
                new UserData { ID = "003", Name = "Charlie", Status = "Active", Date = "2023-10-05" },
                new UserData { ID = "004", Name = "David", Status = "Pending", Date = "2023-10-08" },
            };
            
            UserDataGrid.ItemsSource = users;
            DashboardDataGrid.ItemsSource = users;

            // Initialize Tags
            OrderTags.Tags = new ObservableCollection<string> { "Urgent", "Fragile" };

            // Init Icons
            InitializeIcons();
            IconList.ItemsSource = FilteredIcons;
        }

        private void BtnInfo_Click(object sender, RoutedEventArgs e)
        {
            DFToast.Info("This is an info message.");
        }

        private void BtnSuccess_Click(object sender, RoutedEventArgs e)
        {
            DFToast.Success("Operation completed successfully!");
        }

        private void BtnWarn_Click(object sender, RoutedEventArgs e)
        {
            DFToast.Warning("Please check your input carefully.");
        }

        private void BtnError_Click(object sender, RoutedEventArgs e)
        {
            DFToast.Error("An unexpected error occurred.");
        }

        private void BtnDialog_Click(object sender, RoutedEventArgs e)
        {
            var result = DFMessageBox.Show("Do you like this new UI framework?", "Feedback", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                DFToast.Success("Thanks! We appreciate it.");
            }
            else if (result == MessageBoxResult.No)
            {
                DFToast.Warning("We will try harder next time.");
            }
            else
            {
                DFToast.Info("You cancelled the dialog.");
            }
        }

        private void BtnOpenDFDrawer_Click(object sender, RoutedEventArgs e)
        {
            SettingsDFDrawer.IsOpen = true;
        }

        private System.Threading.CancellationTokenSource _DFLoadingCts;

        private async void BtnDFLoading_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await StartDFLoading(async (token) =>
                {
                    // 模拟耗时操作，传入 token 支持取消
                    await System.Threading.Tasks.Task.Delay(3000, token);
                    DFToast.Success("DFLoading Completed!");
                }, "Processing...", true);
            }
            catch (OperationCanceledException)
            {
                DFToast.Warning("DFLoading Cancelled!");
            }
        }

        private async void BtnLoadSimple_Click(object sender, RoutedEventArgs e)
        {
            // Simple task without cancellation
            await StartDFLoading(async () => await System.Threading.Tasks.Task.Delay(2000), "Simple DFLoading...");
            DFToast.Success("Done!");
        }

        private async void BtnLoadCancel_Click(object sender, RoutedEventArgs e)
        {
            // Long running task with cancellation support
            try
            {
                await StartDFLoading(async (ct) => await System.Threading.Tasks.Task.Delay(5000, ct), "Long Task (Click X to cancel)", true);
                DFToast.Success("Long task finished.");
            }
            catch (OperationCanceledException)
            {
                DFToast.Info("Task was cancelled.");
            }
        }

        private async void BtnLoadResult_Click(object sender, RoutedEventArgs e)
        {
            // Task that returns a value
            var result = await StartDFLoading(async () =>
            {
                await System.Threading.Tasks.Task.Delay(1500);
                return 42;
            }, "Calculating...");

            DFToast.Info($"Calculation Result: {result}");
        }

        private void MainDFLoading_Cancel(object sender, RoutedEventArgs e)
        {
            _DFLoadingCts?.Cancel();
        }

        private void ThemeToggle_Checked(object sender, RoutedEventArgs e)
        {
            ThemeManager.SetTheme(ThemeType.Dark);
        }

        private void ThemeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            ThemeManager.SetTheme(ThemeType.Light);
        }
    }
}
