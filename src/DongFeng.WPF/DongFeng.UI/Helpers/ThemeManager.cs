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
            UpdateDictionary("Theme", themeFile);
            CurrentTheme = theme;
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
            var newDict = new ResourceDictionary { Source = uri };

            var dictionaries = Application.Current.Resources.MergedDictionaries;

            // Find existing dictionaries associated with this 'tag' (heuristic based on path patterns)
            var oldDicts = new List<ResourceDictionary>();

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
                        oldDicts.Add(dict);
                    }
                }
            }

            // Remove old
            foreach (var oldDict in oldDicts)
            {
                dictionaries.Remove(oldDict);
            }

            // Add new
            dictionaries.Add(newDict);
        }
    }
}
