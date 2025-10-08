using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using PsychonautJournalLibrary.Entities.Interfaces;

namespace ToolExplorerWPF.Helpers.Converters
{
    public sealed class IngestionsSummaryConverter : IValueConverter
    {
        // value : IReadOnlyList<IIngestion> (IExperience.IIngestions)
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not System.Collections.Generic.IEnumerable<IIngestion> list)
                return string.Empty;

            return string.Join(", ", list.Select(l => l.SubstanceName).Distinct());
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}