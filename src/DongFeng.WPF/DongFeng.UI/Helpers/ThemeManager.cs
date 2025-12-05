using System;
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
            ResourceDictionary oldDict = null;

            // Find the existing theme dictionary to replace
            // We identify it by checking source uri if possible, or content
            foreach (var dict in dictionaries)
            {
                // If the dictionary source matches one of our theme files
                if (dict.Source != null && 
                   (dict.Source.ToString().EndsWith("Themes/Theme.Light.xaml") || 
                    dict.Source.ToString().EndsWith("Themes/Theme.Dark.xaml")))
                {
                    oldDict = dict;
                    break;
                }
                
                // Fallback: check if it contains a specific key unique to our theme files
                // This is useful if the source URI is lost or if it was merged via Generic.xaml and flattened (though usually MergedDictionaries preserves structure)
                // But wait, if it's merged in Generic.xaml, Generic.xaml is one dictionary.
                // If Generic.xaml is merged into App.xaml, App.xaml's MergedDictionaries contains Generic.xaml.
                // Theme.Light.xaml is inside Generic.xaml's MergedDictionaries.
                
                // The hierarchy is:
                // App.Resources
                //   MergedDictionaries:
                //     1. Generic.xaml
                //          MergedDictionaries:
                //             a. Colors.xaml
                //             b. Theme.Light.xaml <--- We want to replace this
            }

            // If we can't find it at the top level, we might need to dig into Generic.xaml (if we can find it)
            // OR, we can just Add the new one to App.Resources.MergedDictionaries. 
            // If we add it at the end, it overrides previous ones.
            
            // Better approach:
            // Since Generic.xaml loads the default theme, and we want to switch it at runtime.
            // We can add the new theme to App.Resources.MergedDictionaries.
            // But we should remove the PREVIOUSLY added theme by ThemeManager to avoid accumulation.
            
            // Let's try to find the one we added previously.
            
            foreach (var dict in dictionaries)
            {
                 if (dict.Source != null && 
                   (dict.Source.ToString().Contains("Theme.Light.xaml") || 
                    dict.Source.ToString().Contains("Theme.Dark.xaml")))
                {
                    oldDict = dict;
                    break;
                }
            }

            if (oldDict != null)
            {
                dictionaries.Remove(oldDict);
            }
            else
            {
                // If we didn't find a top-level theme dictionary, it means we are running with the default
                // embedded in Generic.xaml.
                // We don't remove Generic.xaml. We just add the new theme at the end of App.Resources.MergedDictionaries.
                // This will override the keys in Generic.xaml.
            }

            dictionaries.Add(newDict);
            CurrentTheme = theme;
        }
    }
}

