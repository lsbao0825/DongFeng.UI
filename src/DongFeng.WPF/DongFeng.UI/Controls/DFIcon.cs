using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.IconPacks;

namespace DongFeng.UI.Controls
{
    /// <summary>
    /// Wrapper for PackIconMaterial to ensure consistent styling across the application.
    /// </summary>
    public class DFIcon : Control
    {
        static DFIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DFIcon), new FrameworkPropertyMetadata(typeof(DFIcon)));
        }



        public PackIconMaterialKind Icon
        {
            get { return (PackIconMaterialKind)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(PackIconMaterialKind), typeof(DFIcon), new PropertyMetadata(null));


    }
}

