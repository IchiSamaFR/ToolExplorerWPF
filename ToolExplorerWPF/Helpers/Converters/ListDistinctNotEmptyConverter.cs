using System;
using System.Collections;
using System.Globalization;
using System.Linq;

namespace ToolExplorerWPF.Helpers.Converters
{
    public sealed class ListDistinctNotEmptyConverter : ListDistinctConverter
    {
        public new object? Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not IEnumerable enumerable)
                return Array.Empty<object>();

            var propertyPath = parameter as string;

            // Filter out items where the property is empty before applying distinct
            var filtered = enumerable
                .Cast<object>()
                .Where(item => !IsPropertyEmpty(item, propertyPath));

            // Apply distinct logic from base class
            var distinctSeq = GetDistinctSequence(filtered, propertyPath);
            return MaterializeToTargetType(distinctSeq, targetType);
        }

        private static bool IsPropertyEmpty(object item, string? propertyPath)
        {
            var propertyValue = string.IsNullOrWhiteSpace(propertyPath)
                ? item
                : GetPropertyPathValue(item, propertyPath);

            return propertyValue switch
            {
                null => true,
                string str => string.IsNullOrWhiteSpace(str),
                IEnumerable enumerable => !enumerable.Cast<object>().Any(),
                _ => false
            };
        }
    }
}