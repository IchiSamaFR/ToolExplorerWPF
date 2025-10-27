using System.Collections.Generic;
using System.Windows;
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
        private bool _isLeftMouseDown = false;
        private readonly HashSet<(int x, int y)> _draggedCells = new();

        protected override LifeGridOptions GetGridOptions()
        {
            double cellSize = BaseCellSize * Zoom;
            double width = ActualWidth / cellSize;
            double height = ActualHeight / cellSize;
            double offsetX = OriginX + width / 2.0;
            double offsetY = OriginY + height / 2.0;
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

            if (IsReadOnly || e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            CaptureMouse();
            _isLeftMouseDown = e.LeftButton == MouseButtonState.Pressed;

            // Only handle if left button is pressed and drag is active
            if (_isLeftMouseDown)
            {
                _draggedCells.Clear();
                HandleLeftMouseClick(e);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (IsReadOnly)
            {
                return;
            }

            // Only handle if left button is pressed and drag is active
            if (_isLeftMouseDown)
            {
                HandleLeftMouseClick(e);
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            _isLeftMouseDown = e.LeftButton == MouseButtonState.Pressed;
            ReleaseMouseCapture();
        }

        private void HandleLeftMouseClick(MouseEventArgs e)
        {
            // Only handle left button
            if (!_isLeftMouseDown)
            {
                return;
            }

            var options = GetGridOptions();
            Point pos = e.GetPosition(this);

            double x = pos.X / options.CellSize;
            double y = pos.Y / options.CellSize;
            double cellX = x - options.OffsetX;
            double cellY = y - options.OffsetY;

            if (cellX < 0) cellX--;
            if (cellY < 0) cellY--;

            var cellPoint = ((int)cellX, (int)cellY);

            if (!_draggedCells.Contains(cellPoint) && CellClickCommand != null && CellClickCommand.CanExecute(cellPoint))
            {
                CellClickCommand.Execute(cellPoint);
                _draggedCells.Add(cellPoint);
            }
        }
    }
}