using System.Windows.Media;
using ToolExplorerWPF.Views.Controls.Commons;

namespace ToolExplorerWPF.Views.Controls.Astar
{
    public class AstarGrid : AMouseGrid
    {
        #region Dependency Properties
        // Rows and Columns count
        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register(
                nameof(Rows),
                typeof(int),
                typeof(AstarGrid),
                new FrameworkPropertyMetadata(20, FrameworkPropertyMetadataOptions.AffectsRender)
            );
        public int Rows
        {
            get => (int)GetValue(RowsProperty);
            set => SetValue(RowsProperty, value);
        }

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(
                nameof(Columns),
                typeof(int),
                typeof(AstarGrid),
                new FrameworkPropertyMetadata(20, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public int Columns
        {
            get => (int)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        public static readonly DependencyProperty WallCellsProperty =
            DependencyProperty.Register(
                nameof(WallCells),
                typeof(ICollection<(int x, int y)>),
                typeof(AstarGrid),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public ICollection<(int x, int y)> WallCells
        {
            get => (ICollection<(int x, int y)>)GetValue(WallCellsProperty);
            set => SetValue(WallCellsProperty, value);
        }

        public static readonly DependencyProperty WallCellBrushProperty =
            DependencyProperty.Register(
                nameof(WallCellBrush),
                typeof(Brush),
                typeof(AstarGrid),
                new FrameworkPropertyMetadata(
                    Brushes.Black,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
                )
            );

        public Brush WallCellBrush
        {
            get => (Brush)GetValue(WallCellBrushProperty);
            set => SetValue(WallCellBrushProperty, value);
        }

        public static readonly DependencyProperty CheckCellsProperty =
            DependencyProperty.Register(
                nameof(CheckCells),
                typeof(ICollection<(int x, int y)>),
                typeof(AstarGrid),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public ICollection<(int x, int y)> CheckCells
        {
            get => (ICollection<(int x, int y)>)GetValue(CheckCellsProperty);
            set => SetValue(CheckCellsProperty, value);
        }

        public static readonly DependencyProperty CheckCellBrushProperty =
            DependencyProperty.Register(
                nameof(CheckCellBrush),
                typeof(Brush),
                typeof(AstarGrid),
                new FrameworkPropertyMetadata(
                    Brushes.LightBlue,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
                )
            );

        public Brush CheckCellBrush
        {
            get => (Brush)GetValue(CheckCellBrushProperty);
            set => SetValue(CheckCellBrushProperty, value);
        }

        public static readonly DependencyProperty PathCellsProperty =
            DependencyProperty.Register(
                nameof(PathCells),
                typeof(ICollection<(int x, int y)>),
                typeof(AstarGrid),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public ICollection<(int x, int y)> PathCells
        {
            get => (ICollection<(int x, int y)>)GetValue(PathCellsProperty);
            set => SetValue(PathCellsProperty, value);
        }

        public static readonly DependencyProperty PathCellBrushProperty =
            DependencyProperty.Register(
                nameof(PathCellBrush),
                typeof(Brush),
                typeof(AstarGrid),
                new FrameworkPropertyMetadata(
                    Brushes.Yellow,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
                )
            );

        public Brush PathCellBrush
        {
            get => (Brush)GetValue(PathCellBrushProperty);
            set => SetValue(PathCellBrushProperty, value);
        }

        public static readonly DependencyProperty StartCellProperty =
            DependencyProperty.Register(
                nameof(StartCell),
                typeof((int x, int y)),
                typeof(AstarGrid),
                new FrameworkPropertyMetadata((0, 0), FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public (int x, int y) StartCell
        {
            get => ((int x, int y))GetValue(StartCellProperty);
            set => SetValue(StartCellProperty, value);
        }

        public static readonly DependencyProperty StartCellBrushProperty =
            DependencyProperty.Register(
                nameof(StartCellBrush),
                typeof(Brush),
                typeof(AstarGrid),
                new FrameworkPropertyMetadata(
                    Brushes.Green,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
                )
            );

        public Brush StartCellBrush
        {
            get => (Brush)GetValue(StartCellBrushProperty);
            set => SetValue(StartCellBrushProperty, value);
        }

        public static readonly DependencyProperty EndCellProperty =
            DependencyProperty.Register(
                nameof(EndCell),
                typeof((int x, int y)),
                typeof(AstarGrid),
                new FrameworkPropertyMetadata((0, 0), FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public (int x, int y) EndCell
        {
            get => ((int x, int y))GetValue(EndCellProperty);
            set => SetValue(EndCellProperty, value);
        }

        public static readonly DependencyProperty EndCellBrushProperty =
            DependencyProperty.Register(
                nameof(EndCellBrush),
                typeof(Brush),
                typeof(AstarGrid),
                new FrameworkPropertyMetadata(
                    Brushes.Red,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
                )
            );

        public Brush EndCellBrush
        {
            get => (Brush)GetValue(EndCellBrushProperty);
            set => SetValue(EndCellBrushProperty, value);
        }
        #endregion

        private const double PaddingRatio = 0.1; // 10% padding

        protected override GridOptions GetGridOptions()
        {
            // Guard against invalid state
            if (Columns <= 0 || Rows <= 0 || ActualWidth <= 0 || ActualHeight <= 0)
            {
                return new GridOptions(10, 10, 1.0, 0.0, 0.0);
            }

            // Compute target cell size with padding, keep square cells
            double paddingMultiplier = 1.0 + PaddingRatio;
            double targetCellW = ActualWidth / (Columns * paddingMultiplier);
            double targetCellH = ActualHeight / (Rows * paddingMultiplier);
            double cellSize = Math.Min(targetCellW, targetCellH);
            if (cellSize <= 0) cellSize = 1.0; // safety

            // Use reciprocal to avoid repeated divisions
            double invCell = 1.0 / cellSize;
            double widthCells = ActualWidth * invCell;
            double heightCells = ActualHeight * invCell;

            // Fractional leftovers for centering
            double widthOut = widthCells - Math.Floor(widthCells);
            double heightOut = heightCells - Math.Floor(heightCells);

            // Center the grid and apply pan (OriginX/OriginY)
            double offsetX = OriginX + (widthCells - Columns) * 0.5 + widthOut * 0.5;
            double offsetY = OriginY + (heightCells - Rows) * 0.5 + heightOut * 0.5;

            return new GridOptions(widthCells, heightCells, cellSize, offsetX, offsetY);
        }


        protected override void OnRender(DrawingContext dc)
        {
            var options = GetGridOptions();
            DrawBackground(dc);
            DrawCells(dc, CheckCells, options, CheckCellBrush);
            DrawCells(dc, PathCells, options, PathCellBrush);
            DrawCells(dc, WallCells, options, WallCellBrush);
            DrawCell(dc, StartCell, options, StartCellBrush);
            DrawCell(dc, EndCell, options, EndCellBrush);
            DrawGridLines(dc, options);
        }
    }
}