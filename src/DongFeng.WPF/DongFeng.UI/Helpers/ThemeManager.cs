using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DongFeng.UI.Helpers
{
    public enum ThemeType
    {
        Light,
        Dark
    }

    public static class ThemeManager
    {
        public static ThemeType CurrentTheme { get; private set; } = ThemeType.Light;

        public static void SetTheme(ThemeType theme)
        {
            string themeFile = theme == ThemeType.Light ? "Themes/Theme.Light.xaml" : "Themes/Theme.Dark.xaml";
            var uri = new Uri($"pack://application:,,,/DongFeng.UI;component/{themeFile}");
            
            var newDict = new ResourceDictionary { Source = uri };

            var dictionaries = Application.Current.Resources.MergedDictionaries;

            // 收集所有旧的主题字典 (Theme.Light.xaml 或 Theme.Dark.xaml)
            var oldDicts = new List<ResourceDictionary>();
            
            foreach (var dict in dictionaries)
            {
                if (dict.Source != null)
                {
                    var source = dict.Source.ToString();
                    // 检查是否是我们管理的主题文件
                    if (source.EndsWith("Themes/Theme.Light.xaml", StringComparison.OrdinalIgnoreCase) || 
                        source.EndsWith("Themes/Theme.Dark.xaml", StringComparison.OrdinalIgnoreCase))
                    {
                        oldDicts.Add(dict);
                    }
                }
            }

            // 移除旧字典
            foreach (var oldDict in oldDicts)
            {
                dictionaries.Remove(oldDict);
            }

            // 添加新字典到末尾，以覆盖默认样式（如 Generic.xaml 中的）
            dictionaries.Add(newDict);
            CurrentTheme = theme;
        }
    }
}
