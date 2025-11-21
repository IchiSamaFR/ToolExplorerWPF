using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Media;

namespace ToolExplorerWPF.Behaviors
{
    public static class StackPanelBehavior
    {
        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.RegisterAttached(
                "Spacing",
                typeof(double),
                typeof(StackPanelBehavior),
                new PropertyMetadata(0.0, OnSpacingChanged));

        public static void SetSpacing(DependencyObject element, double value)
        {
            element.SetValue(SpacingProperty, value);
        }

        public static double GetSpacing(DependencyObject element)
        {
            return (double)element.GetValue(SpacingProperty);
        }

        private static void OnSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StackPanel stackPanel)
            {
                stackPanel.Loaded -= StackPanel_Loaded;
                stackPanel.Loaded += StackPanel_Loaded;

                ApplySpacing(stackPanel);
            }
        }

        private static void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is StackPanel stackPanel)
            {
                ApplySpacing(stackPanel);
            }
        }

        private static void ApplySpacing(StackPanel stackPanel)
        {
            double spacing = GetSpacing(stackPanel);
            int count = stackPanel.Children.Count;

            for (int i = 0; i < count; i++)
            {
                if (stackPanel.Children[i] is FrameworkElement child)
                {
                    if (stackPanel.Orientation == Orientation.Horizontal)
                    {
                        child.Margin = new Thickness(
                            child.Margin.Left,
                            child.Margin.Top,
                            (i < count - 1) ? spacing : 0,
                            child.Margin.Bottom);
                    }
                    else
                    {
                        child.Margin = new Thickness(
                            child.Margin.Left,
                            child.Margin.Top,
                            child.Margin.Right,
                            (i < count - 1) ? spacing : 0);
                    }
                }
            }
        }
    }
}