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
        public static string CurrentPalette { get; private set; } = "Default";

        public static void SetTheme(ThemeType theme)
        {
            string themeFile = theme == ThemeType.Light ? "Themes/Theme.Light.xaml" : "Themes/Theme.Dark.xaml";
            
            // 1. 更新 Theme.Light/Dark.xaml
            UpdateDictionary("Theme", themeFile);
            CurrentTheme = theme;
            
            // 2. 强制重新加载 Palette
            // 因为 Theme 文件可能会覆盖某些 DynamicResource 的查找路径，
            // 或者仅仅是确保资源层级顺序正确： Palette (Base Colors) -> Theme (Semantic Brush)
            // 通常正确的顺序是 Palette 先加载，Theme 后加载（如果 Theme 依赖 Palette 中的静态资源）
            // 但是在这里我们全用了 DynamicResource，所以顺序应该不敏感。
            // 
            // 真正的问题可能是：切换 Palette 后，WPF 没有自动刷新那些使用了 DynamicResource 的控件？
            // 不，WPF DynamicResource 会监听字典变化。
            // 
            // 让我们检查 UpdateDictionary 的逻辑。
            // 当我们 SetPalette 时，只是替换了 Colors.xaml。
            // 如果 Theme.Light.xaml 之前已经解析了颜色，WPF 需要知道 "DF_Color_Primary" 变了。
            // 替换 MergedDictionary 是正确的方法。
            // 
            // 尝试一个 Trick：重新加载 Theme 也可以触发刷新。
            // 但其实，保持 Palette 在 Theme 之前可能更好。
            
            // Re-apply palette to ensure consistency if order matters or glitches occur
            SetPalette(CurrentPalette);
        }

        public static void SetPalette(string paletteName)
        {
            // Default maps to Colors.xaml, others to Palettes/{name}.xaml
            string paletteFile = (string.IsNullOrEmpty(paletteName) || paletteName.Equals("Default", StringComparison.OrdinalIgnoreCase))
                ? "Themes/Colors.xaml"
                : $"Themes/Palettes/{paletteName}.xaml";

            UpdateDictionary("Palette", paletteFile);
            CurrentPalette = paletteName;
        }

        private static void UpdateDictionary(string tag, string filePath)
        {
            var uri = new Uri($"pack://application:,,,/DongFeng.UI;component/{filePath}");
            
            var dictionaries = Application.Current.Resources.MergedDictionaries;

            // 1. 找到旧字典
            ResourceDictionary oldDict = null;
            
            foreach (var dict in dictionaries)
            {
                if (dict.Source != null)
                {
                    var source = dict.Source.ToString();
                    bool isTarget = false;

                    if (tag == "Theme")
                    {
                        isTarget = source.EndsWith("Themes/Theme.Light.xaml", StringComparison.OrdinalIgnoreCase) ||
                                   source.EndsWith("Themes/Theme.Dark.xaml", StringComparison.OrdinalIgnoreCase);
                    }
                    else if (tag == "Palette")
                    {
                        isTarget = source.EndsWith("Themes/Colors.xaml", StringComparison.OrdinalIgnoreCase) ||
                                   source.Contains("Themes/Palettes/");
                    }

                    if (isTarget)
                    {
                        oldDict = dict;
                        break; // 假设只有一份
                    }
                }
            }

            // 2. 创建新字典
            var newDict = new ResourceDictionary { Source = uri };

            // 3. 替换逻辑
            if (oldDict != null)
            {
                // 如果是 Palette，我们需要它在 Theme 之前。
                // 现有的逻辑是 Remove 然后 Add (添加到末尾)。
                // 如果 Theme 在列表里，Palette 被加到了 Theme 后面。
                // 如果 Theme 用的是 StaticResource (之前是，现在我改成了 DynamicResource)，这会导致问题。
                // 即便 DynamicResource，保持层级清晰也是好的： Palette (基础) -> Theme (映射) -> App资源
                
                // 让我们尝试原地替换，或者根据 Tag 决定插入位置。
                
                int index = dictionaries.IndexOf(oldDict);
                dictionaries.RemoveAt(index);
                dictionaries.Insert(index, newDict);
            }
            else
            {
                // 如果没找到旧的（比如第一次启动可能没正确识别），就添加到开头或结尾
                // Palette 应该比较靠前。
                if (tag == "Palette")
                {
                    dictionaries.Insert(0, newDict);
                }
                else
                {
                    dictionaries.Add(newDict);
                }
            }
        }
    }
}
