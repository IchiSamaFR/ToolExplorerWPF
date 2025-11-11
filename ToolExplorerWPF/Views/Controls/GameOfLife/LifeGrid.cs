using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ToolExplorerWPF.Views.Controls.Commons;

namespace ToolExplorerWPF.Views.Controls.GameOfLife
{
    public class LifeGrid : AMouseGrid
    {
        #region Dependency Properties

        public static readonly DependencyProperty AliveCellBrushProperty =
            DependencyProperty.Register(
                nameof(AliveCellBrush),
                typeof(Brush),
                typeof(LifeGrid),
                new FrameworkPropertyMetadata(
                    Brushes.Black,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
                )
            );

        /// <summary>
        /// The brush used to paint alive cells.
        /// </summary>
        public Brush AliveCellBrush
        {
            get => (Brush)GetValue(AliveCellBrushProperty);
            set => SetValue(AliveCellBrushProperty, value);
        }

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

        public static readonly DependencyProperty IsCellFitProperty =
            DependencyProperty.Register(
                nameof(IsCellFit),
                typeof(bool),
                typeof(LifeGrid),
                new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
                )
            );
        public bool IsCellFit
        {
            get => (bool)GetValue(IsCellFitProperty);
            set => SetValue(IsCellFitProperty, value);
        }

        #endregion

        private const double PaddingRatio = 0.1; // 10% padding
        private const double BaseCellSize = 30.0;

        protected override GridOptions GetGridOptions()
        {
            return IsCellFit ? FitGetGridOptions() : BaseGetGridOptions();
        }
        private GridOptions FitGetGridOptions()
        {
            // If there are no alive cells, fallback to default grid
            if (AliveCells == null || AliveCells.Count == 0)
            {
                int baseSize = 10;
                double minDimension = Math.Min(ActualWidth, ActualHeight);
                double noneCellSize = minDimension / baseSize;
                return new GridOptions(ActualWidth / noneCellSize, ActualHeight / noneCellSize, noneCellSize, 0, 0);
            }

            // Find pattern bounds in a single pass
            int minX = int.MaxValue, maxX = int.MinValue, minY = int.MaxValue, maxY = int.MinValue;
            foreach (var (x, y) in AliveCells)
            {
                if (x < minX) minX = x;
                if (x > maxX) maxX = x;
                if (y < minY) minY = y;
                if (y > maxY) maxY = y;
            }

            int patternColumns = maxX - minX + 1;
            int patternRows = maxY - minY + 1;

            // Pre-calculate padding multiplier to avoid repeated multiplications
            double paddingMultiplier = 1.0 + PaddingRatio;

            // Calculate cell size to fit all cells and keep them square, with padding
            double cellWidth = ActualWidth / (patternColumns * paddingMultiplier);
            double cellHeight = ActualHeight / (patternRows * paddingMultiplier);
            double cellSize = Math.Min(cellWidth, cellHeight) * Zoom;

            // Calculate grid size in cells using division instead of cast
            double width = ActualWidth / cellSize;
            double height = ActualHeight / cellSize;

            // Calculate offset to center the pattern
            double offsetX = OriginX + (width - patternColumns) * 0.5;
            double offsetY = OriginY + (height - patternRows) * 0.5;

            return new GridOptions(
                width,
                height,
                cellSize,
                offsetX,
                offsetY
            );
        }
        private GridOptions BaseGetGridOptions()
        {
            double cellSize = BaseCellSize * Zoom;
            double invCellSize = 1.0 / cellSize;

            double width = ActualWidth * invCellSize;
            double height = ActualHeight * invCellSize;

            double offsetX = OriginX + width * 0.5;
            double offsetY = OriginY + height * 0.5;

            return new GridOptions(width, height, cellSize, offsetX, offsetY);
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
            DrawCells(dc, AliveCells, options, AliveCellBrush);
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