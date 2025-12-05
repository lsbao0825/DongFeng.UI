using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace DongFeng.UI.Helpers
{
    public static class IconHelper
    {
        private static List<string> _allIcons;

        public static List<string> GetAllMaterialIconNames()
        {
            if (_allIcons != null) return _allIcons;

            _allIcons = new List<string>();

            var iconType = typeof(PackIconMaterialKind);
            var values = Enum.GetValues(iconType);

            foreach (var val in values)
            {
                _allIcons.Add(val.ToString());
            }

            _allIcons.Sort();
            return _allIcons;
        }

        public static PackIconMaterial CreateIcon(string kindName)
        {
            if (Enum.TryParse(kindName, out PackIconMaterialKind kind))
            {
                return new PackIconMaterial { Kind = kind };
            }
            return null;
        }
    }
}

