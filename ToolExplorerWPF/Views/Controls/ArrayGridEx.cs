using System;
using System.Windows;
using System.Windows.Controls;

namespace ToolExplorerWPF.Views.Controls
{
    public class ArrayGridEx : Grid
    {
        public static readonly DependencyProperty ArraySourceProperty =
            DependencyProperty.Register(
                nameof(ArraySource),
                typeof(object[,]),
                typeof(ArrayGridEx),
                new PropertyMetadata(null, OnArraySourceChanged));

        public object[,] ArraySource
        {
            get => (object[,])GetValue(ArraySourceProperty);
            set => SetValue(ArraySourceProperty, value);
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(
                nameof(ItemTemplate),
                typeof(DataTemplate),
                typeof(ArrayGridEx),
                new PropertyMetadata(null, OnItemTemplateChanged));

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        private static void OnArraySourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ArrayGridEx grid)
            {
                grid.BuildGrid();
            }
        }

        private static void OnItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ArrayGridEx grid)
            {
                grid.BuildGrid();
            }
        }

        private void BuildGrid()
        {
            Children.Clear();
            RowDefinitions.Clear();
            ColumnDefinitions.Clear();

            if (ArraySource == null)
                return;

            int rows = ArraySource.GetLength(0);
            int cols = ArraySource.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }
            for (int j = 0; j < cols; j++)
            {
                ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var contentControl = new ContentControl
                    {
                        Content = ArraySource[i, j],
                        ContentTemplate = ItemTemplate
                    };
                    SetRow(contentControl, i);
                    SetColumn(contentControl, j);
                    Children.Add(contentControl);
                }
            }
        }
    }
}