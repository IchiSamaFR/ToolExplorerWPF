using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ToolExplorerWPF.Helpers.Converters
{
    public class BooleanDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isDetached = value as bool? == true;

            if (parameter == null)
            {
                throw new ArgumentException("Parameter must be in the format 'trueWidth,falseWidth'");
            }

            string paramStr = parameter.ToString()?.ToLower() ?? string.Empty;
            var param = paramStr.Split(',');
            if (!paramStr.Contains(','))
            {
                throw new ArgumentException("Parameter must be in the format 'trueWidth,falseWidth'");
            }

            string[] parts = paramStr.Split(',');
            return isDetached ? parts[0].Trim() : parts[1].Trim();

            //return ParseGridLength(toReturn);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}