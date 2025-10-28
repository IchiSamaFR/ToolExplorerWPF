using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace ToolExplorerWPF.Views.Controls.GameOfLife
{
    public abstract class AMouseLifeGrid : ALifeGrid
    {
        #region Dependency Properties

        public static readonly DependencyProperty CellClickCommandProperty =
            DependencyProperty.Register(
                nameof(CellClickCommand),
                typeof(ICommand),
                typeof(AMouseLifeGrid),
                new PropertyMetadata(null)
            );

        public ICommand CellClickCommand
        {
            get => (ICommand)GetValue(CellClickCommandProperty);
            set => SetValue(CellClickCommandProperty, value);
        }

        #endregion

        private bool _isLeftMouseDown = false;
        private readonly HashSet<(int x, int y)> _draggedCells = new();

        // Middle mouse drag state
        private bool _isMiddleMouseDown = false;
        private Point _lastMiddleMousePos;


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

            if (e.ChangedButton == MouseButton.Left && !IsReadOnly)
            {
                CaptureMouse();
                _isLeftMouseDown = true;
                _draggedCells.Clear();
                HandleLeftMouse(e);
            }
            else if (e.ChangedButton == MouseButton.Middle)
            {
                CaptureMouse();
                _isMiddleMouseDown = true;
                _lastMiddleMousePos = e.GetPosition(this);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // Left click drag for cell editing
            if (_isLeftMouseDown && !IsReadOnly)
            {
                HandleLeftMouse(e);
            }

            // Middle click drag for panning
            if (_isMiddleMouseDown)
            {
                HandleMiddleMouse(e);
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.ChangedButton == MouseButton.Left)
            {
                _isLeftMouseDown = false;
            }
            else if (e.ChangedButton == MouseButton.Middle)
            {
                _isMiddleMouseDown = false;
            }
            ReleaseMouseCapture();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            HandleMouseWheel(e);
        }


        private void HandleLeftMouse(MouseEventArgs e)
        {
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

        private void HandleMiddleMouse(MouseEventArgs e)
        {
            // Only handle if middle mouse drag is active
            if (!_isMiddleMouseDown)
            {
                return;
            }

            Point currentPos = e.GetPosition(this);
            double dx = currentPos.X - _lastMiddleMousePos.X;
            double dy = currentPos.Y - _lastMiddleMousePos.Y;

            var options = GetGridOptions();

            double cellSize = options.CellSize;
            if (cellSize > 0)
            {
                // Move the grid origin according to the drag delta (in cell units)
                OriginX += dx / cellSize;
                OriginY += dy / cellSize;
            }

            _lastMiddleMousePos = currentPos;
        }

        private void HandleMouseWheel(MouseWheelEventArgs e)
        {
            const double ZoomStep = 0.1;
            const double MinZoom = 0.1;
            const double MaxZoom = 5.0;

            // Get mouse position relative to the control
            Point mousePos = e.GetPosition(this);

            // Calculate the cell under the mouse before zoom
            var options = GetGridOptions();
            double cellX = (mousePos.X / options.CellSize) - options.OffsetX;
            double cellY = (mousePos.Y / options.CellSize) - options.OffsetY;

            // Adjust zoom
            double newZoom = Zoom;
            if (e.Delta > 0)
            {
                newZoom = Math.Min(MaxZoom, Zoom + ZoomStep);
            }
            else if (e.Delta < 0)
            {
                newZoom = Math.Max(MinZoom, Zoom - ZoomStep);
            }

            if (Math.Abs(newZoom - Zoom) < double.Epsilon)
            {
                return;
            }

            Zoom = newZoom;

            // Recalculate options after zoom
            var newOptions = GetGridOptions();

            // Adjust OriginX and OriginY to keep the cell under the mouse at the same place
            OriginX += cellX - ((mousePos.X / newOptions.CellSize) - newOptions.OffsetX);
            OriginY += cellY - ((mousePos.Y / newOptions.CellSize) - newOptions.OffsetY);
        }
    }
}
