using GameOfLifeLibrary.Models.Cells;
using System.ComponentModel;

namespace ToolExplorerWPF.Models.GameOfLife
{
    public partial class ObservableCell : ObservableObject, ICell
    {
        [ObservableProperty]
        private bool _isAlive;

        public int X { get; }
        public int Y { get; }

        public ObservableCell(int x, int y)
            : this(x, y, false)
        {
        }

        public ObservableCell(int x, int y, bool isAlive)
        {
            X = x;
            Y = y;
            IsAlive = isAlive;
        }

        public void Test()
        {

            OnPropertyChanged(string.Empty);
        }
    }
}