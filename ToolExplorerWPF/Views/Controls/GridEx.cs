using AstarLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ToolExplorerWPF.Views.Controls
{
    internal class GridEx : Grid
    {
        private ObservableCollection<ContentControl> _contentControls = new ObservableCollection<ContentControl>();

        #region ItemTemplate Property
        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ItemTemplateProperty);
            }
            set
            {
                SetValue(ItemTemplateProperty, value);
            }
        }

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.RegisterAttached("ItemTemplate", typeof(DataTemplate), typeof(GridEx), new PropertyMetadata(null, ItemTemplateChanged));
        public static void ItemTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is GridEx) || e.NewValue == null)
                return;

            GridEx grid = (GridEx)obj;

            grid.UpdateControls();
        }
        #endregion

        #region RowCount Property
        public int RowCount
        {
            get
            {
                return (int)GetValue(RowCountProperty);
            }
            set
            {
                SetValue(RowCountProperty, value);
            }
        }

        public static readonly DependencyProperty RowCountProperty = DependencyProperty.RegisterAttached("RowCount", typeof(int), typeof(GridEx), new PropertyMetadata(-1, RowCountChanged));
        public static void RowCountChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is GridEx) || (int)e.NewValue < 0)
                return;

            GridEx grid = (GridEx)obj;
            grid.RowDefinitions.Clear();

            for (int i = 0; i < (int)e.NewValue; i++)
                grid.RowDefinitions.Add(
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            grid.UpdateControls();
        }
        #endregion

        #region ColumnCount Property
        public int ColumnCount
        {
            get
            {
                return (int)GetValue(ColumnCountProperty);
            }
            set
            {
                SetValue(ColumnCountProperty, value);
            }
        }

        public static readonly DependencyProperty ColumnCountProperty = DependencyProperty.RegisterAttached("ColumnCount", typeof(int), typeof(GridEx), new PropertyMetadata(-1, ColumnCountChanged));
        public static void ColumnCountChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is GridEx) || (int)e.NewValue < 0)
                return;

            GridEx grid = (GridEx)obj;
            grid.ColumnDefinitions.Clear();

            for (int i = 0; i < (int)e.NewValue; i++)
                grid.ColumnDefinitions.Add(
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            grid.UpdateControls();
        }
        #endregion

        #region ItemSource Property
        public IList ItemsSource
        {
            get
            {
                return (IList)GetValue(ItemSourceProperty);
            }
            set
            {
                SetValue(ItemSourceProperty, value);
            }
        }

        public static readonly DependencyProperty ItemSourceProperty = DependencyProperty.RegisterAttached("ItemsSource", typeof(IList), typeof(GridEx), new PropertyMetadata(null, ItemsSourceChanged));
        public static void ItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is GridEx) || e.NewValue == null)
                return;

            GridEx grid = (GridEx)obj;

            grid.UpdateControls();
        }
        #endregion

        public void UpdateControls()
        {
            if (ItemsSource == null)
                return;

            if (_contentControls.Count > ItemsSource.Count)
            {
                var lst = new List<UIElement>();

                for (int i = _contentControls.Count - 1; i >= ItemsSource.Count; i--)
                {
                    lst.Add(Children[i]);
                    _contentControls.RemoveAt(i);
                }
                RemoveChildren(lst);
            }
            if (_contentControls.Count < ItemsSource.Count)
            {
                for (int i = _contentControls.Count; i < ItemsSource.Count; i++)
                {
                    var control = new ContentControl()
                    {
                        ContentTemplate = ItemTemplate
                    };
                    _contentControls.Add(control);
                    Children.Add(control);
                }
            }

            int index = 0;
            foreach (var item in ItemsSource)
            {
                // Calculer la position de l'élément dans la grille
                int row = index / ColumnCount;
                int column = index % ColumnCount;


                // Créer un ContentControl pour afficher chaque élément
                var contentControl = _contentControls[index];
                contentControl.DataContext = item;
                contentControl.Content = item;

                // Ajouter l'élément dans la grille à la position calculée
                SetRow(contentControl, row);
                SetColumn(contentControl, column);

                index++;
            }
        }
        private void RemoveChildren(IList<UIElement> itemsToRemove)
        {
            foreach (var item in itemsToRemove)
            {
                if (item is FrameworkElement frameworkElement)
                {
                    frameworkElement.DataContext = null;
                }

                Children.Remove(item);
            }
        }
    }
}
