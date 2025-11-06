using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ToolExplorerWPF.Views.Controls.Commons
{
    public abstract class AGrid : FrameworkElement
    {
        protected struct GridOptions
        {
            public double Width;
            public double Height;
            public double CellSize;
            public double OffsetX;
            public double OffsetY;

            public GridOptions(double width, double height, double cellSize, double offsetX, double offsetY)
            {
                Width = width;
                Height = height;
                CellSize = cellSize;
                OffsetX = offsetX;
                OffsetY = offsetY;
            }
        }

        #region DependencyProperties
        public static readonly DependencyProperty BackgroundBrushProperty =
            DependencyProperty.Register(
                nameof(BackgroundBrush),
                typeof(Brush),
                typeof(AGrid),
                new FrameworkPropertyMetadata(
                    Brushes.White,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
                )
            );

        /// <summary>
        /// The brush used to paint the grid background.
        /// </summary>
        public Brush BackgroundBrush
        {
            get => (Brush)GetValue(BackgroundBrushProperty);
            set => SetValue(BackgroundBrushProperty, value);
        }

        public static readonly DependencyProperty GridLineBrushProperty =
            DependencyProperty.Register(
                nameof(GridLineBrush),
                typeof(Brush),
                typeof(AGrid),
                new FrameworkPropertyMetadata(
                    Brushes.LightGray,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
                )
            );

        /// <summary>
        /// The brush used to paint the grid lines.
        /// </summary>
        public Brush GridLineBrush
        {
            get => (Brush)GetValue(GridLineBrushProperty);
            set => SetValue(GridLineBrushProperty, value);
        }

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                nameof(IsReadOnly),
                typeof(bool),
                typeof(AGrid),
                new FrameworkPropertyMetadata(false)
            );

        public virtual bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public static readonly DependencyProperty AliveCellsProperty =
            DependencyProperty.Register(
                nameof(AliveCells),
                typeof(ICollection<(int x, int y)>),
                typeof(AGrid),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnCellsChanged)
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
                typeof(AGrid),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
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
                typeof(AGrid),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
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
                typeof(AGrid),
                new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
            );

        public double Zoom
        {
            get => (double)GetValue(ZoomProperty);
            set => SetValue(ZoomProperty, value);
        }

        #endregion

        // Pens are static to avoid recreating them on every render
        private const int DefaultThinBlock = 1;
        private const int DefaultThickBlock = 10;

        protected abstract GridOptions GetGridOptions();

        protected static void OnCellsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AGrid grid)
            {
                grid.OnCellsChanged();
            }
        }
        protected virtual void OnCellsChanged()
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            var options = GetGridOptions();
            DrawBackground(dc);
            DrawGridLines(dc, options);
        }

        protected void DrawBackground(DrawingContext dc)
        {
            dc.DrawRectangle(BackgroundBrush, null, new Rect(0, 0, ActualWidth, ActualHeight));
        }

        protected void DrawGridLines(DrawingContext dc, GridOptions options)
        {
            int thinBlock = DefaultThinBlock;
            int thickBlock = DefaultThickBlock;

            while (options.CellSize * thinBlock < 10)
            {
                thinBlock *= 10;
                thickBlock *= 10;
            }

            // Use the current GridLineBrush for both pens
            Pen thinPen = new Pen(GridLineBrush, 1);
            thinPen.Freeze();
            Pen thickPen = new Pen(GridLineBrush, 2);
            thickPen.Freeze();

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
                    dc.DrawLine(thickPen, new Point(xPos, 0), new Point(xPos, ActualHeight));
                }
                else if (x % thinBlock == 0)
                {
                    dc.DrawLine(thinPen, new Point(xPos, 0), new Point(xPos, ActualHeight));
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
                    dc.DrawLine(thickPen, new Point(0, yPos), new Point(ActualWidth, yPos));
                }
                else if (y % thinBlock == 0)
                {
                    dc.DrawLine(thinPen, new Point(0, yPos), new Point(ActualWidth, yPos));
                }
            }
        }


        protected (int x, int y) GetCellFromPoint(MouseEventArgs e, double pixelX, double pixelY)
        {
            var options = GetGridOptions();

            double x = pixelX / options.CellSize;
            double y = pixelY / options.CellSize;
            double cellX = x - options.OffsetX;
            double cellY = y - options.OffsetY;

            if (cellX < 0) cellX--;
            if (cellY < 0) cellY--;

            return ((int)cellX, (int)cellY);
        }
    }
}