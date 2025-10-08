using System.Globalization;
using System.Windows.Data;

namespace ToolExplorerWPF.Helpers.Converters
{
    public class TextToPassConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string str)
            {
                if(parameter is char character)
                {
                    return new string(character, str.Length);
                }
                return new string('*', str.Length);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
