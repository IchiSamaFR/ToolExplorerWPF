using System.Globalization;
using System.Windows.Data;

namespace ToolExplorerWPF.Helpers
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null && parameter.ToString().ToLower() == "invert")
            {
                return value as bool? == false ? Visibility.Visible : Visibility.Collapsed;
            }
            return value as bool? == true ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
