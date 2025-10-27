using System.Windows.Input;
using System.Windows.Media;

namespace ToolExplorerWPF.Views.Controls.GameOfLife
{
    public class LifeGridPattern : ALifeGrid
    {
        protected override LifeGridOptions GetGridOptions()
        {
            int minX = AliveCells.Min(c => c.x);
            int maxX = AliveCells.Max(c => c.x);
            int minY = AliveCells.Min(c => c.y);
            int maxY = AliveCells.Max(c => c.y);

            int patternColumns = maxX - minX + 1;
            int patternRows = maxY - minY + 1;

            // Calculate cell size to fit all cells and keep them square
            double cellWidth = ActualWidth / patternColumns;
            double cellHeight = ActualHeight / patternRows;
            double cellSize = Math.Min(cellWidth, cellHeight);

            int width = (int)(ActualWidth / cellSize);
            width -= width / 10;
            int height = (int)(ActualHeight / cellSize);
            height -= height / 10;

            // Calculate total grid size in pixels
            double gridWidth = cellSize * patternColumns;
            double gridHeight = cellSize * patternRows;

            // Calculate offset to center the grid
            double offsetX = (width - patternColumns) / 2.0;
            double offsetY = (height - patternRows) / 2.0;

            // Set options for drawing
            return new LifeGridOptions(
                width,
                height,
                cellSize,
                Math.Abs((int)offsetX),
                Math.Abs((int)offsetY)
            );
        }
        protected override void OnRender(DrawingContext dc)
        {
            // Draw background if no cells
            if (AliveCells == null || AliveCells.Count == 0)
            {
                DrawBackground(dc);
                return;
            }

            var options = GetGridOptions();

            DrawBackground(dc);
            DrawAliveCells(dc, AliveCells, options);
            DrawGridLines(dc, options);
        }
    }
}