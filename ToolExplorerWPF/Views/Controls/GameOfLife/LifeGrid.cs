using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ToolExplorerWPF.Views.Controls.GameOfLife
{
    public class LifeGrid : AMouseLifeGrid
    {
        #region Dependency Properties

        public static readonly DependencyProperty PreviewCellsProperty =
            DependencyProperty.Register(
                nameof(PreviewCells),
                typeof(ICollection<(int x, int y)>),
                typeof(LifeGrid),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnPreviewCellsChanged)
            );

        public ICollection<(int x, int y)> PreviewCells
        {
            get => (ICollection<(int x, int y)>)GetValue(PreviewCellsProperty);
            set => SetValue(PreviewCellsProperty, value);
        }
        private ICollection<(int x, int y)>? _displayedPreviewCells;


        public static readonly DependencyProperty PreviewCellBrushProperty =
            DependencyProperty.Register(
                nameof(PreviewCellBrush),
                typeof(Brush),
                typeof(LifeGrid),
                new FrameworkPropertyMetadata(
                    Brushes.Cyan,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
                )
            );
        public Brush PreviewCellBrush
        {
            get => (Brush)GetValue(PreviewCellBrushProperty);
            set => SetValue(PreviewCellBrushProperty, value);
        }

        #endregion

        private const double BaseCellSize = 30.0;

        protected override LifeGridOptions GetGridOptions()
        {
            double cellSize = BaseCellSize * Zoom;
            double width = ActualWidth / cellSize;
            double height = ActualHeight / cellSize;
            double offsetX = OriginX + width / 2.0;
            double offsetY = OriginY + height / 2.0;
            return new LifeGridOptions(width, height, cellSize, offsetX, offsetY);
        }

        protected static void OnPreviewCellsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LifeGrid grid)
            {
                grid.OnPreviewCellsChanged();
            }
        }
        protected virtual void OnPreviewCellsChanged()
        {
            _displayedPreviewCells = new List<(int x, int y)>();
            InvalidateVisual();
        }
        protected override void OnRender(DrawingContext dc)
        {
            var options = GetGridOptions();
            DrawBackground(dc);
            DrawAliveCells(dc, AliveCells, options);
            DrawCells(dc, _displayedPreviewCells, options, PreviewCellBrush);
            DrawGridLines(dc, options);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            HandlePreviewMouseMove(e);
        }

        private void HandlePreviewMouseMove(MouseEventArgs e)
        {
            if (PreviewCells == null || PreviewCells.Count == 0)
            {
                return;
            }

            // Get mouse position relative to the grid

            var options = GetGridOptions();
            Point pos = e.GetPosition(this);

            double x = pos.X / options.CellSize;
            double y = pos.Y / options.CellSize;
            double cellX = x - options.OffsetX;
            double cellY = y - options.OffsetY;

            if (cellX < 0) cellX--;
            if (cellY < 0) cellY--;

            // Update PreviewCells with the hovered cell
            _displayedPreviewCells = PreviewCells.Select(c => (c.x + (int)cellX, c.y + (int)cellY)).ToList();
            InvalidateVisual();
        }
    }
}