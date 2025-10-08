using System;
using System.Globalization;
using System.Windows.Data;

namespace ToolExplorerWPF.Helpers.Converters
{
    /// <summary>
    /// Convertit un long représentant un timestamp Unix (ms ou s) vers DateTime (Local) et inversement.
    /// Paramètre (string):
    ///  - "ms"  : force millisecondes (défaut)
    ///  - "s"   : force secondes
    ///  - "auto": détection (>= 10^12 => ms, sinon s)
    /// </summary>
    public sealed class UnixEpochToDateTimeConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return null;
            if (value is not long l) return null;

            var mode = (parameter as string)?.ToLowerInvariant() ?? "ms";
            long unixMs;

            if (mode == "s")
            {
                unixMs = l * 1000;
            }
            else if (mode == "auto")
            {
                // Heuristique: timestamps en secondes ~ 10 digits, en ms ~ 13 digits
                unixMs = l >= 1_000_000_000_000 ? l : l * 1000;
            }
            else
            {
                // "ms" (par défaut)
                unixMs = l;
            }

            try
            {
                var dto = DateTimeOffset.FromUnixTimeMilliseconds(unixMs);
                return dto.LocalDateTime;
            }
            catch
            {
                return null;
            }
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return null;
            if (value is not DateTime dt) return null;

            var mode = (parameter as string)?.ToLowerInvariant() ?? "ms";
            var dto = new DateTimeOffset(DateTime.SpecifyKind(dt, DateTimeKind.Local)).ToUniversalTime();
            long unixMs = dto.ToUnixTimeMilliseconds();

            return mode switch
            {
                "s" => unixMs / 1000,
                "auto" => unixMs, // On ne peut pas deviner si l’utilisateur veut s ou ms => on renvoie ms
                _ => unixMs
            };
        }
    }
}