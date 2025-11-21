using System.Windows.Controls;

namespace ToolExplorerWPF.Behaviors
{
    public static class GridBehavior
    {
        public static readonly DependencyProperty ColumnDefinitionsProperty =
            DependencyProperty.RegisterAttached(
                "ColumnDefinitions",
                typeof(string),
                typeof(GridBehavior),
                new PropertyMetadata(null, OnColumnDefinitionsChanged));

        public static void SetColumnDefinitions(DependencyObject element, string value)
        {
            element.SetValue(ColumnDefinitionsProperty, value);
        }

        public static string GetColumnDefinitions(DependencyObject element)
        {
            return (string)element.GetValue(ColumnDefinitionsProperty);
        }

        private static void OnColumnDefinitionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Grid grid && e.NewValue is string columns)
            {
                grid.ColumnDefinitions.Clear();
                var definitions = columns.Split(',');
                foreach (var def in definitions)
                {
                    var cd = new ColumnDefinition();
                    cd.Width = ParseGridLength(def.Trim());
                    grid.ColumnDefinitions.Add(cd);
                }
            }
        }

        public static readonly DependencyProperty RowDefinitionsProperty =
            DependencyProperty.RegisterAttached(
                "RowDefinitions",
                typeof(string),
                typeof(GridBehavior),
                new PropertyMetadata(null, OnRowDefinitionsChanged));

        public static void SetRowDefinitions(DependencyObject element, string value)
        {
            element.SetValue(RowDefinitionsProperty, value);
        }

        public static string GetRowDefinitions(DependencyObject element)
        {
            return (string)element.GetValue(RowDefinitionsProperty);
        }

        private static void OnRowDefinitionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Grid grid && e.NewValue is string rows)
            {
                grid.RowDefinitions.Clear();
                var definitions = rows.Split(',');
                foreach (var def in definitions)
                {
                    var rd = new RowDefinition();
                    rd.Height = ParseGridLength(def.Trim());
                    grid.RowDefinitions.Add(rd);
                }
            }
        }

        private static GridLength ParseGridLength(string value)
        {
            if (string.Equals(value, "Auto", StringComparison.OrdinalIgnoreCase))
                return GridLength.Auto;
            if (value.EndsWith("*"))
            {
                var starValue = value.TrimEnd('*');
                if (string.IsNullOrEmpty(starValue))
                    return new GridLength(1, GridUnitType.Star);
                if (double.TryParse(starValue, out var v))
                    return new GridLength(v, GridUnitType.Star);
            }
            if (double.TryParse(value, out var pixel))
                return new GridLength(pixel, GridUnitType.Pixel);

            // Default fallback
            return new GridLength(1, GridUnitType.Star);
        }
    }
}
