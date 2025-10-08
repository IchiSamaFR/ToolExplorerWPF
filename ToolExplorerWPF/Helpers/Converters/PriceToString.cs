using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ToolExplorerWPF.Helpers.Converters
{
    public class PriceToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal)
            {
                return ((decimal)value).ToString(parameter.ToString()) + "€";
            }
            if (value is float)
            {
                return ((float)value).ToString(parameter.ToString()) + "€";
            }
            if (value is double)
            {
                return ((double)value).ToString(parameter.ToString()) + "€";
            }
            return value?.ToString() + "€";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal)
            {
                return ((decimal)value).ToString(parameter.ToString()) + "€";
            }
            if (value is float)
            {
                return ((float)value).ToString(parameter.ToString()) + "€";
            }
            if (value is double)
            {
                return ((double)value).ToString(parameter.ToString()) + "€";
            }
            return value?.ToString() + "€";
        }
    }
}
