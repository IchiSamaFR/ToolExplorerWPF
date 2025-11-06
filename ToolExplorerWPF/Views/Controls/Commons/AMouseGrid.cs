using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace ToolExplorerWPF.Views.Controls.Commons
{
    public abstract class AMouseGrid : AGrid
    {
        #region Dependency Properties
        public static readonly DependencyProperty LeftCellClickCommandProperty =
            DependencyProperty.Register(
                nameof(LeftCellClickCommand),
                typeof(ICommand),
                typeof(AMouseGrid),
                new PropertyMetadata(null)
            );

        public ICommand LeftCellClickCommand
        {
            get => (ICommand)GetValue(LeftCellClickCommandProperty);
            set => SetValue(LeftCellClickCommandProperty, value);
        }

        public static readonly DependencyProperty RightCellClickCommandProperty =
            DependencyProperty.Register(
                nameof(RightCellClickCommand),
                typeof(ICommand),
                typeof(AMouseGrid),
                new PropertyMetadata(null)
            );

        public ICommand RightCellClickCommand
        {
            get => (ICommand)GetValue(RightCellClickCommandProperty);
            set => SetValue(RightCellClickCommandProperty, value);
        }

        public static readonly DependencyProperty LeftCellDragCommandProperty =
            DependencyProperty.Register(
                nameof(LeftCellDragCommand),
                typeof(ICommand),
                typeof(AMouseGrid),
                new PropertyMetadata(null)
            );

        public ICommand LeftCellDragCommand
        {
            get => (ICommand)GetValue(LeftCellDragCommandProperty);
            set => SetValue(LeftCellDragCommandProperty, value);
        }

        public static readonly DependencyProperty RightCellDragCommandProperty =
            DependencyProperty.Register(
                nameof(RightCellDragCommand),
                typeof(ICommand),
                typeof(AMouseGrid),
                new PropertyMetadata(null)
            );

        public ICommand RightCellDragCommand
        {
            get => (ICommand)GetValue(RightCellDragCommandProperty);
            set => SetValue(RightCellDragCommandProperty, value);
        }

        #endregion

        private bool _mouseClickable = false;

        private bool _isLeftMouseDown = false;
        private bool _isRightMouseDown = false;
        private readonly HashSet<(int x, int y)> _draggedCells = new();

        // Middle mouse drag state
        private bool _isMiddleMouseDown = false;
        private Point _lastMiddleMousePos;

        protected void DrawCell(DrawingContext dc, (int x, int y)? cell, GridOptions options, Brush cellBrush)
        {
            if (cell == null)
            {
                return;
            }
            DrawCells(dc, new List<(int x, int y)> { cell.Value }, options, cellBrush);
        }
        protected void DrawCells(DrawingContext dc, ICollection<(int x, int y)>? cells, GridOptions options, Brush cellBrush)
        {
            if (cells == null || cells.Count == 0)
            {
                return;
            }

            double cellSize = options.CellSize;
            double width = options.Width;
            double height = options.Height;
            double offsetX = options.OffsetX;
            double offsetY = options.OffsetY;

            foreach (var (xCell, yCell) in cells)
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
                    cellBrush,
                    null,
                    new Rect(x * cellSize, y * cellSize, cellWidth, cellHeight));
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.ChangedButton == MouseButton.Left)
            {
                if (IsReadOnly)
                    return;

                CaptureMouse();
                _mouseClickable = true;
                _isLeftMouseDown = true;
                _isRightMouseDown = false;
                _draggedCells.Clear();
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                if (IsReadOnly)
                    return;

                CaptureMouse();
                _mouseClickable = true;
                _isLeftMouseDown = false;
                _isRightMouseDown = true;
                _draggedCells.Clear();
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

            if (_mouseClickable)
            {
                _mouseClickable = false;
            }

            // Left click drag for cell editing
            if ((_isLeftMouseDown || _isRightMouseDown) && !IsReadOnly)
            {
                HandleDragMouse(e);
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
                if(_mouseClickable && LeftCellClickCommand != null)
                {
                    Point pos = e.GetPosition(this);
                    var cellPoint = GetCellFromPoint(e, pos.X, pos.Y);
                    if (LeftCellClickCommand.CanExecute(cellPoint))
                    {
                        LeftCellClickCommand.Execute(cellPoint);
                    }
                }
                _isLeftMouseDown = false;
            }
            if (e.ChangedButton == MouseButton.Right)
            {
                if (_mouseClickable && RightCellClickCommand != null)
                {
                    Point pos = e.GetPosition(this);
                    var cellPoint = GetCellFromPoint(e, pos.X, pos.Y);
                    if (RightCellClickCommand.CanExecute(cellPoint))
                    {
                        RightCellClickCommand.Execute(cellPoint);
                    }
                }
                _isRightMouseDown = false;
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

        private void HandleDragMouse(MouseEventArgs e)
        {
            if (!_isLeftMouseDown && !_isRightMouseDown)
            {
                return;
            }

            Point pos = e.GetPosition(this);
            var cellPoint = GetCellFromPoint(e, pos.X, pos.Y);
            if (!_draggedCells.Contains(cellPoint))
            {
                if(_isLeftMouseDown && LeftCellDragCommand != null && LeftCellDragCommand.CanExecute(cellPoint))
                {
                    LeftCellDragCommand.Execute(cellPoint);
                    _draggedCells.Add(cellPoint);
                }
                else if(_isRightMouseDown && RightCellDragCommand != null && RightCellDragCommand.CanExecute(cellPoint))
                {
                    RightCellDragCommand.Execute(cellPoint);
                    _draggedCells.Add(cellPoint);
                }
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
