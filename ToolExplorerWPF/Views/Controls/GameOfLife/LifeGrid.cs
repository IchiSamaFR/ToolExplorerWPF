using System.Windows.Input;
using System.Windows.Media;

namespace ToolExplorerWPF.Views.Controls.GameOfLife
{
    public class LifeGrid : ALifeGrid
    {
        #region Dependency Properties
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
        #endregion

        private const double BaseCellSize = 30.0;

        protected override LifeGridOptions GetGridOptions()
        {
            double cellSize = BaseCellSize * Zoom;

            double width = ActualWidth / cellSize;
            double height = ActualHeight / cellSize;

            double offsetX = OriginX + width / 2;
            double offsetY = OriginY + height / 2;

            return new LifeGridOptions(width, height, cellSize, offsetX, offsetY);
        }

        protected override void OnRender(DrawingContext dc)
        {
            var options = GetGridOptions();

            DrawBackground(dc);
            DrawAliveCells(dc, AliveCells, options);
            DrawGridLines(dc, options);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (IsReadOnly)
            {
                return;
            }

            // Utilise GetGridOptions pour obtenir les options de la grille
            var options = GetGridOptions();

            Point pos = e.GetPosition(this);
            var x = pos.X / options.CellSize;
            var y = pos.Y / options.CellSize;

            var cellX = x - options.OffsetX;
            var cellY = y - options.OffsetY;

            if (cellX < 0)
                cellX--;
            if (cellY < 0)
                cellY--;

            var cellPoint = ((int)cellX, (int)cellY);
            CellClickCommand?.Execute(cellPoint);

            InvalidateVisual();
        }
    }
}