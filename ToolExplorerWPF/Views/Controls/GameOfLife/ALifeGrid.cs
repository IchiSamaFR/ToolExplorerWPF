using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        // IsReadOnly property (indicates if the grid is editable or not)
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
                typeof(int),
                typeof(ALifeGrid),
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
                typeof(ALifeGrid),
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
                typeof(ALifeGrid),
                new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender)
            );
        public double Zoom
        {
            get => (double)GetValue(ZoomProperty);
            set => SetValue(ZoomProperty, value);
        }
        #endregion

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
            if (aliveCells == null) return;
            foreach (var pt in aliveCells)
            {
                var x = pt.x + options.OffsetX;
                var y = pt.y + options.OffsetY;

                if (x > -1 && x <= options.Width && y > -1 && y <= options.Height)
                {
                    double cellWidth = options.CellSize;
                    double cellHeight = options.CellSize;

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

                    // Si la cellule est sur la dernière colonne, on ajuste la largeur
                    if ((int)x == (int)options.Width - 1)
                    {
                        cellWidth = ActualWidth - x * options.CellSize;
                    }
                    // Si la cellule est sur la dernière ligne, on ajuste la hauteur
                    if ((int)y == (int)options.Height - 1)
                    {
                        cellHeight = ActualHeight - y * options.CellSize;
                    }

                    dc.DrawRectangle(
                        Brushes.Black,
                        null,
                        new Rect(x * options.CellSize, y * options.CellSize, cellWidth, cellHeight));
                }
            }
        }

        protected void DrawGridLines(DrawingContext dc, LifeGridOptions options)
        {
            // Always two pens: thin (1px) and thick (2px)
            Pen thinPen = new Pen(Brushes.LightGray, 1);
            thinPen.Freeze();
            Pen thickPen = new Pen(Brushes.Gray, 2);
            thickPen.Freeze();

            int thinBlock = 1;
            int thickBlock = 10;

            while (options.CellSize * thinBlock < 10)
            {
                thinBlock *= 10;
                thickBlock *= 10;
            }

            // Draw vertical lines
            var halfWidth = (int)options.Width / 2;
            for (int x = -halfWidth; x <= halfWidth; x++)
            {
                var gridX = x + options.OffsetX;
                double xPos = gridX * options.CellSize;

                if ((int)x % thickBlock == 0)
                {
                    dc.DrawLine(thickPen, new Point(xPos, 0), new Point(xPos, ActualHeight));
                }
                else if ((int)x % thinBlock == 0)
                {
                    dc.DrawLine(thinPen, new Point(xPos, 0), new Point(xPos, ActualHeight));
                }
            }

            // Draw horizontal lines
            var halfHeight = (int)options.Height / 2;
            for (int y = -halfHeight; y <= halfHeight; y++)
            {
                var gridY = y + options.OffsetY;
                double yPos = gridY * options.CellSize;

                if ((int)y % thickBlock == 0)
                {
                    dc.DrawLine(thickPen, new Point(0, yPos), new Point(ActualWidth, yPos));
                }
                else if ((int)y % thinBlock == 0)
                {
                    dc.DrawLine(thinPen, new Point(0, yPos), new Point(ActualWidth, yPos));
                }
            }
        }
    }
}
