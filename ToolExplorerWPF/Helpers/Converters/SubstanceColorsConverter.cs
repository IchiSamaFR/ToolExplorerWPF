using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using PsychonautJournalLibrary.Constants;

namespace ToolExplorerWPF.Helpers.Converters
{
    internal sealed class SubstanceColorsConverter : IValueConverter
    {
        private static readonly BrushConverter BrushConverter = new();
        private static readonly Dictionary<string, SolidColorBrush> BrushCache = new(StringComparer.OrdinalIgnoreCase);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var name = value?.ToString();
            if (string.IsNullOrWhiteSpace(name))
            {
                return targetType == typeof(Color) ? Colors.Transparent : Brushes.Transparent;
            }

            var isDark = ResolveIsDark(parameter);
            var hex = SubstanceColors.GetHex(name, isDark);

            if (targetType == typeof(Color))
            {
                return (Color)ColorConverter.ConvertFromString(hex);
            }

            if (BrushCache.TryGetValue(hex, out var cached))
            {
                return cached;
            }

            var brush = (SolidColorBrush)BrushConverter.ConvertFromString(hex);
            brush.Freeze();
            BrushCache[hex] = brush;
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static bool ResolveIsDark(object parameter)
        {
            if (parameter is string s)
            {
                if (bool.TryParse(s, out var b))
                    return b;

                if (string.Equals(s, "Dark", StringComparison.OrdinalIgnoreCase))
                    return true;

                if (string.Equals(s, "Light", StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return true;
        }
    }
}