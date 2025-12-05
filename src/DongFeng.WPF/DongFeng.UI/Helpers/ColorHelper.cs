using System;
using System.Windows.Media;

namespace DongFeng.UI.Helpers
{
    public static class ColorHelper
    {
        public struct HsvColor
        {
            public double H;
            public double S;
            public double V;
        }

        public static HsvColor ColorToHsv(Color color)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            double h, s, v;
            
            // Manual HSV conversion
            double delta = max - min;
            v = max / 255.0;
            
            if (max == 0) s = 0;
            else s = delta / max;

            if (s == 0) h = 0;
            else
            {
                if (color.R == max)
                    h = (double)(color.G - color.B) / delta;
                else if (color.G == max)
                    h = 2 + (double)(color.B - color.R) / delta;
                else
                    h = 4 + (double)(color.R - color.G) / delta;

                h *= 60;
                if (h < 0) h += 360;
            }

            return new HsvColor { H = h, S = s, V = v };
        }

        public static Color HsvToColor(double h, double s, double v)
        {
            int hi = Convert.ToInt32(Math.Floor(h / 60)) % 6;
            double f = h / 60 - Math.Floor(h / 60);

            v = v * 255;
            byte v_byte = Convert.ToByte(v);
            byte p = Convert.ToByte(v * (1 - s));
            byte q = Convert.ToByte(v * (1 - f * s));
            byte t = Convert.ToByte(v * (1 - (1 - f) * s));

            switch (hi)
            {
                case 0: return Color.FromRgb(v_byte, t, p);
                case 1: return Color.FromRgb(q, v_byte, p);
                case 2: return Color.FromRgb(p, v_byte, t);
                case 3: return Color.FromRgb(p, q, v_byte);
                case 4: return Color.FromRgb(t, p, v_byte);
                default: return Color.FromRgb(v_byte, p, q);
            }
        }
    }
}

