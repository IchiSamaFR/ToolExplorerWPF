using System.Windows.Input;
using System.Windows.Media;

namespace ToolExplorerWPF.Views.Controls.GameOfLife
{
    public class LifeGrid : FrameworkElement
    {
        private struct LifeGridOptions
        {
            public int Width;
            public int Height;
            public double CellWidth;
            public double CellHeight;
            public int OffsetX;
            public int OffsetY;

            public LifeGridOptions(int width, int height, double cellWidth, double cellHeight, int offsetX, int offsetY)
            {
                Width = width;
                Height = height;
                CellWidth = cellWidth;
                CellHeight = cellHeight;
                OffsetX = offsetX;
                OffsetY = offsetY;
            }
        }

        #region Dependency Properties
        public static readonly DependencyProperty AliveCellsProperty =
            DependencyProperty.Register(
                nameof(AliveCells),
                typeof(ICollection<(int x, int y)>),
                typeof(LifeGrid),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnAliveCellsChanged)
            );

        public ICollection<(int x, int y)> AliveCells
        {
            get => (ICollection<(int x, int y)>)GetValue(AliveCellsProperty);
            set => SetValue(AliveCellsProperty, value);
        }

        public static readonly DependencyProperty CellClickCommandProperty =
            DependencyProperty.Register(
                nameof(CellClickCommand),
                typeof(ICommand),
                typeof(LifeGrid),
                new PropertyMetadata(null)
            );

        public ICommand CellClickCommand
        {
            get => (ICommand)GetValue(CellClickCommandProperty);
            set => SetValue(CellClickCommandProperty, value);
        }


        public static readonly DependencyProperty OriginXProperty =
            DependencyProperty.Register(
                nameof(OriginX),
                typeof(int),
                typeof(LifeGrid),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public int OriginX
        {
            get => (int)GetValue(OriginXProperty);
            set => SetValue(OriginXProperty, value);
        }

        public static readonly DependencyProperty OriginYProperty =
            DependencyProperty.Register(
                nameof(OriginY),
                typeof(int),
                typeof(LifeGrid),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public int OriginY
        {
            get => (int)GetValue(OriginYProperty);
            set => SetValue(OriginYProperty, value);
        }

        public static readonly DependencyProperty ZoomProperty =
            DependencyProperty.Register(
                nameof(Zoom),
                typeof(double),
                typeof(LifeGrid),
                new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        /// <summary>
        /// Zoom factor. 1.0 means 40px per cell.
        /// </summary>
        public double Zoom
        {
            get => (double)GetValue(ZoomProperty);
            set => SetValue(ZoomProperty, value);
        }
        #endregion

        /// <summary>
        /// Zoom out factor (inverse of Zoom). If Zoom = 2, zoomOut = 0.5.
        /// </summary>
        public double ZoomOut => 1.0 / Zoom;

        private const double BaseCellSize = 30.0;
        private const double ThinMinCellSize = 10.0;
        private const double ThickMinCellSize = 2.0;

        private static void OnAliveCellsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LifeGrid grid)
            {
                grid.InvalidateVisual();
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            double cellWidth = BaseCellSize * Zoom;
            double cellHeight = BaseCellSize * Zoom;

            int width = (int)(ActualWidth / cellWidth);
            int height = (int)(ActualHeight / cellHeight);

            int offsetX = OriginX - width / 2;
            int offsetY = OriginY - height / 2;

            var options = new LifeGridOptions(width, height, cellWidth, cellHeight, offsetX, offsetY);

            DrawBackground(dc);
            DrawAliveCells(dc, AliveCells, options);
            DrawGridLines(dc, options);
        }

        private void DrawBackground(DrawingContext dc)
        {
            dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, ActualWidth, ActualHeight));
        }

        private void DrawAliveCells(
            DrawingContext dc,
            ICollection<(int x, int y)> aliveCells,
            LifeGridOptions options)
        {
            if (aliveCells == null) return;
            foreach (var pt in aliveCells)
            {
                int x = pt.x - options.OffsetX;
                int y = pt.y - options.OffsetY;
                if (x >= 0 && x <= options.Width && y >= 0 && y <= options.Height)
                {
                    double cellWidth = options.CellWidth;
                    double cellHeight = options.CellHeight;

                    // Si la cellule est sur la dernière colonne, on ajuste la largeur
                    if (x == options.Width)
                    {
                        cellWidth = ActualWidth - x * options.CellWidth;
                    }
                    // Si la cellule est sur la dernière ligne, on ajuste la hauteur
                    if (y == options.Height)
                    {
                        cellHeight = ActualHeight - y * options.CellHeight;
                    }

                    dc.DrawRectangle(
                        Brushes.Black,
                        null,
                        new Rect(x * options.CellWidth, y * options.CellHeight, cellWidth, cellHeight));
                }
            }
        }

        private void DrawGridLines(
            DrawingContext dc,
            LifeGridOptions options)
        {
            bool thickVisible = options.CellWidth > ThickMinCellSize || options.CellHeight > ThickMinCellSize;
            bool thinVisible = options.CellWidth > ThinMinCellSize || options.CellHeight > ThinMinCellSize;

            Pen thinPen = new Pen(Brushes.LightGray, 1);
            thinPen.Freeze();

            Pen thickPen = new Pen(Brushes.Gray, thinVisible ? 2 : 1);
            thickPen.Freeze();

            // Draw 10x10 grid lines (vertical)
            for (int x = 0; x <= options.Width; x++)
            {
                int gridX = options.OffsetX + x;
                if (gridX % 10 == 0 && thickVisible)
                {
                    double xPos = x * options.CellWidth;
                    dc.DrawLine(
                        thickPen,
                        new Point(xPos, 0),
                        new Point(xPos, ActualHeight)
                    );
                }
                else if (thinVisible)
                {
                    double xPos = x * options.CellWidth;
                    dc.DrawLine(
                        thinPen,
                        new Point(xPos, 0),
                        new Point(xPos, ActualHeight)
                    );
                }
            }

            // Draw 10x10 grid lines (horizontal)
            for (int y = 0; y <= options.Height; y++)
            {
                int gridY = options.OffsetY + y;
                if (gridY % 10 == 0 && thickVisible)
                {
                    double yPos = y * options.CellHeight;
                    dc.DrawLine(
                        thickPen,
                        new Point(0, yPos),
                        new Point(ActualWidth, yPos)
                    );
                }
                else if (thinVisible)
                {
                    double yPos = y * options.CellHeight;
                    dc.DrawLine(
                        thinPen,
                        new Point(0, yPos),
                        new Point(ActualWidth, yPos)
                    );
                }
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            double cellWidth = BaseCellSize * Zoom;
            double cellHeight = BaseCellSize * Zoom;

            int width = (int)(ActualWidth / cellWidth);
            int height = (int)(ActualHeight / cellHeight);

            // Centrage sur OriginX, OriginY
            int offsetX = OriginX - width / 2;
            int offsetY = OriginY - height / 2;

            Point pos = e.GetPosition(this);
            int x = (int)(pos.X / cellWidth);
            int y = (int)(pos.Y / cellHeight);

            int cellX = offsetX + x;
            int cellY = offsetY + y;

            var cellPoint = (cellX, cellY);
            CellClickCommand?.Execute(cellPoint);

            InvalidateVisual();
        }
    }
}