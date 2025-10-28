using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;

namespace ToolExplorerWPF.Views.Controls.GameOfLife
{
    public class LifeGridPattern : AMouseLifeGrid
    {
        private const double PaddingRatio = 0.1; // 10% padding

        // Always readonly
        public override bool IsReadOnly
        {
            get => true;
            set { }
        }


        protected override void OnAliveCellsChanged()
        {
            base.OnAliveCellsChanged();
            Zoom = 1.0;
            OriginX = 0.0;
            OriginY = 0.0;
        }

        protected override LifeGridOptions GetGridOptions()
        {
            // If there are no alive cells, fallback to default grid
            if (AliveCells == null || AliveCells.Count == 0)
            {
                return new LifeGridOptions(1, 1, Math.Min(ActualWidth, ActualHeight), 0, 0);
            }

            // Find pattern bounds in a single pass for performance
            int minX = int.MaxValue, maxX = int.MinValue, minY = int.MaxValue, maxY = int.MinValue;
            foreach (var (x, y) in AliveCells)
            {
                if (x < minX) minX = x;
                if (x > maxX) maxX = x;
                if (y < minY) minY = y;
                if (y > maxY) maxY = y;
            }

            int patternColumns = maxX - minX + 1;
            int patternRows = maxY - minY + 1;

            // Calculate cell size to fit all cells and keep them square, with padding
            double cellWidth = ActualWidth / (patternColumns + patternColumns * PaddingRatio);
            double cellHeight = ActualHeight / (patternRows + patternRows * PaddingRatio);
            double cellSize = Math.Min(cellWidth, cellHeight) * Zoom;

            // Calculate grid size in cells, with padding
            int width = (int)(ActualWidth / cellSize);
            int height = (int)(ActualHeight / cellSize);

            // Calculate offset to center the pattern
            double offsetX = OriginX + (width - patternColumns) / 2.0;
            double offsetY = OriginY + (height - patternRows) / 2.0;

            return new LifeGridOptions(
                width,
                height,
                cellSize,
                offsetX,
                offsetY
            );
        }
    }
}