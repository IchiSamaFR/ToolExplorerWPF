using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Media;

namespace ToolExplorerWPF.Behaviors
{
    public static class WrapPanelBehavior
    {
        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.RegisterAttached(
                "Spacing",
                typeof(double),
                typeof(WrapPanelBehavior),
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
            if (d is WrapPanel wrapPanel)
            {
                wrapPanel.Loaded -= WrapPanel_Loaded;
                wrapPanel.Loaded += WrapPanel_Loaded;

                ApplySpacing(wrapPanel);
            }
        }

        private static void WrapPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is WrapPanel wrapPanel)
            {
                ApplySpacing(wrapPanel);
            }
        }

        private static void ApplySpacing(WrapPanel wrapPanel)
        {
            double spacing = GetSpacing(wrapPanel);
            int count = wrapPanel.Children.Count;

            for (int i = 0; i < count; i++)
            {
                if (wrapPanel.Children[i] is FrameworkElement child)
                {
                    if (wrapPanel.Orientation == Orientation.Horizontal)
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