using GameOfLifeLibrary.Grids;
using GameOfLifeLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolExplorerWPF.Models.GameOfLife
{
    public class ObservableGrid : Grid
    {
        protected override Type CellTemplate => typeof(ObservableCell);
        public ObservableGrid(int width, int height) : base(width, height)
        {
        }
    }
}
