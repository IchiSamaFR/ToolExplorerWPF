using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ToolExplorerWPF.Views.Controls.GameOfLife
{
    public abstract class ALifeGrid : FrameworkElement
    {
        protected struct LifeGridOptions
        {
            public double Width;
            public double Height;
            public double CellSize;
            public double OffsetX;
            public double OffsetY;

            public LifeGridOptions(double width, double height, double cellSize, double offsetX, double offsetY)
            {
                Width = width;
                Height = height;
                CellSize = cellSize;
                OffsetX = offsetX;
                OffsetY = offsetY;
            }
        }

        #region DependencyProperty

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                nameof(IsReadOnly),
                typeof(bool),
                typeof(ALifeGrid),
                new FrameworkPropertyMetadata(false)
            );

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public static readonly DependencyProperty AliveCellsProperty =
            DependencyProperty.Register(
                nameof(AliveCells),
                typeof(ICollection<(int x, int y)>),
                typeof(ALifeGrid),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnAliveCellsChanged)
            );

        public ICollection<(int x, int y)> AliveCells
        {
            get => (ICollection<(int x, int y)>)GetValue(AliveCellsProperty);
            set => SetValue(AliveCellsProperty, value);
        }

        public static readonly DependencyProperty OriginXProperty =
            DependencyProperty.Register(
                nameof(OriginX),
                typeof(double),
                typeof(ALifeGrid),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public double OriginX
        {
            get => (double)GetValue(OriginXProperty);
            set => SetValue(OriginXProperty, value);
        }

        public static readonly DependencyProperty OriginYProperty =
            DependencyProperty.Register(
                nameof(OriginY),
                typeof(double),
                typeof(ALifeGrid),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public double OriginY
        {
            get => (double)GetValue(OriginYProperty);
            set => SetValue(OriginYProperty, value);
        }

        public static readonly DependencyProperty ZoomProperty =
            DependencyProperty.Register(
                nameof(Zoom),
                typeof(double),
                typeof(ALifeGrid),
                new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        public double Zoom
        {
            get => (double)GetValue(ZoomProperty);
            set => SetValue(ZoomProperty, value);
        }

        #endregion

        // Pens are static to avoid recreating them on every render
        private static readonly Pen ThinPen;
        private static readonly Pen ThickPen;
        private const int DefaultThinBlock = 1;
        private const int DefaultThickBlock = 10;

        static ALifeGrid()
        {
            ThinPen = new Pen(Brushes.LightGray, 1);
            ThinPen.Freeze();
            ThickPen = new Pen(Brushes.Gray, 2);
            ThickPen.Freeze();
        }

        protected abstract LifeGridOptions GetGridOptions();

        protected static void OnAliveCellsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ALifeGrid grid)
            {
                grid.InvalidateVisual();
            }
        }

        protected void DrawBackground(DrawingContext dc)
        {
            dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, ActualWidth, ActualHeight));
        }

        protected void DrawAliveCells(DrawingContext dc, ICollection<(int x, int y)> aliveCells, LifeGridOptions options)
        {
            if (aliveCells == null || aliveCells.Count == 0)
            {
                return;
            }

            double cellSize = options.CellSize;
            double width = options.Width;
            double height = options.Height;
            double offsetX = options.OffsetX;
            double offsetY = options.OffsetY;

            foreach (var (xCell, yCell) in aliveCells)
            {
                double x = xCell + offsetX;
                double y = yCell + offsetY;

                // Only draw if inside visible grid
                if (x <= -1 || x >= width || y <= -1 || y >= height)
                {
                    continue;
                }

                double cellWidth = cellSize;
                double cellHeight = cellSize;

                // Si la cellule est sur la première colonne, on ajuste la largeur
                if (x < 0)
                {
                    cellWidth = (1 + x) * options.CellSize;
                    x = 0;
                }
                // Si la cellule est sur la première ligne, on ajuste la hauteur
                if (y < 0)
                {
                    cellHeight = (1 + y) * options.CellSize;
                    y = 0;
                }
                // Adjust width for last column
                if (x > width - 1)
                {
                    cellWidth = Math.Max(0, ActualWidth - x * cellSize);
                }
                // Adjust height for last row
                if (y > height - 1)
                {
                    cellHeight = Math.Max(0, ActualHeight - y * cellSize);
                }

                dc.DrawRectangle(
                    Brushes.Black,
                    null,
                    new Rect(x * cellSize, y * cellSize, cellWidth, cellHeight));
            }
        }

        protected void DrawGridLines(DrawingContext dc, LifeGridOptions options)
        {
            int thinBlock = DefaultThinBlock;
            int thickBlock = DefaultThickBlock;

            while (options.CellSize * thinBlock < 10)
            {
                thinBlock *= 10;
                thickBlock *= 10;
            }

            // Calculate visible range for vertical lines
            int minX = (int)Math.Floor(-options.OffsetX);
            int maxX = (int)Math.Ceiling((ActualWidth / options.CellSize) - options.OffsetX);

            for (int x = minX; x <= maxX; x++)
            {
                double xPos = (x + options.OffsetX) * options.CellSize;
                if (xPos < 0 || xPos > ActualWidth)
                    continue;

                if (x % thickBlock == 0)
                {
                    dc.DrawLine(ThickPen, new Point(xPos, 0), new Point(xPos, ActualHeight));
                }
                else if (x % thinBlock == 0)
                {
                    dc.DrawLine(ThinPen, new Point(xPos, 0), new Point(xPos, ActualHeight));
                }
            }

            // Calculate visible range for horizontal lines
            int minY = (int)Math.Floor(-options.OffsetY);
            int maxY = (int)Math.Ceiling((ActualHeight / options.CellSize) - options.OffsetY);

            for (int y = minY; y <= maxY; y++)
            {
                double yPos = (y + options.OffsetY) * options.CellSize;
                if (yPos < 0 || yPos > ActualHeight)
                    continue;

                if (y % thickBlock == 0)
                {
                    dc.DrawLine(ThickPen, new Point(0, yPos), new Point(ActualWidth, yPos));
                }
                else if (y % thinBlock == 0)
                {
                    dc.DrawLine(ThinPen, new Point(0, yPos), new Point(ActualWidth, yPos));
                }
            }
        }
    }
}