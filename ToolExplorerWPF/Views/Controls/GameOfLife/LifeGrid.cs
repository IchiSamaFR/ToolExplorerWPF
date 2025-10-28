using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ToolExplorerWPF.Views.Controls.GameOfLife
{
    public class LifeGrid : AMouseLifeGrid
    {

        private const double BaseCellSize = 30.0;

        protected override LifeGridOptions GetGridOptions()
        {
            double cellSize = BaseCellSize * Zoom;
            double width = ActualWidth / cellSize;
            double height = ActualHeight / cellSize;
            double offsetX = OriginX + width / 2.0;
            double offsetY = OriginY + height / 2.0;
            return new LifeGridOptions(width, height, cellSize, offsetX, offsetY);
        }
    }
}