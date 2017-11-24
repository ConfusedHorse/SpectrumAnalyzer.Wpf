using System.Windows.Media;

namespace SpectrumAnalyzer.Bass.ViewModel.Helpers
{
    public static class ColorHelper
    {
        public static Color Merge(this Color color1, Color color2)
        {
            return Color.FromRgb(
                (byte) ((color1.R + color2.R) / 2), 
                (byte) ((color1.G + color2.G) / 2),
                (byte) ((color1.B + color2.B) / 2));
        }
        public static Color Merge(this Color color1, Color color2, double weight)
        {
            if (weight > 1 || weight < 0) return Merge(color1, color2);
            return Color.FromRgb(
                (byte)(color1.R * (1 - weight) + color2.R * weight),
                (byte)(color1.G * (1 - weight) + color2.G * weight),
                (byte)(color1.B * (1 - weight) + color2.B * weight));
        }
    }
}