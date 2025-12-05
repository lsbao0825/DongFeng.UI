using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DongFeng.UI.Controls;
using DongFeng.UI.Helpers;
using MahApps.Metro.IconPacks;

namespace DongFeng
{
    public partial class MainWindow
    {
        // Add new region for Icon handling
        #region Icon Gallery

        public ObservableCollection<string> AllIcons { get; set; }
        public ObservableCollection<string> FilteredIcons { get; set; }
        private int _currentPage = 1;
        private int _pageSize = 100;

        private void InitializeIcons()
        {
            AllIcons = new ObservableCollection<string>(IconHelper.GetAllMaterialIconNames());
            FilteredIcons = new ObservableCollection<string>();
            
            // Set DataContext for icon binding if not already set or needs specific handling
            // Assuming MainWindow DataContext is self or viewmodel. 
            // For this demo, let's just set ItemsSource in code behind or ensure property is accessible.
            
            UpdateIconPage();
        }

        private void UpdateIconPage()
        {
            if (AllIcons == null) return;

            var paged = AllIcons.Skip((_currentPage - 1) * _pageSize).Take(_pageSize).ToList();
            FilteredIcons.Clear();
            foreach (var icon in paged)
            {
                FilteredIcons.Add(icon);
            }
            
            if (IconPagination != null)
            {
                IconPagination.TotalCount = AllIcons.Count;
                IconPagination.CurrentPage = _currentPage;
                IconPagination.PageSize = _pageSize;
            }
        }

        private void IconSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            string query = textBox?.Text?.ToLower() ?? "";

            if (string.IsNullOrWhiteSpace(query))
            {
                 AllIcons = new ObservableCollection<string>(IconHelper.GetAllMaterialIconNames());
            }
            else
            {
                 var filtered = IconHelper.GetAllMaterialIconNames()
                    .Where(x => x.ToLower().Contains(query))
                    .ToList();
                 AllIcons = new ObservableCollection<string>(filtered);
            }
            
            _currentPage = 1;
            UpdateIconPage();
        }

        private void IconPagination_PageChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            _currentPage = e.NewValue;
            UpdateIconPage();
        }

        private void IconItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is string iconName)
            {
                Clipboard.SetText($"<iconPacks:PackIconMaterial Kind=\"{iconName}\" />");
                Message.Success($"Copied to clipboard: {iconName}");
            }
        }

        #endregion
    }
}

