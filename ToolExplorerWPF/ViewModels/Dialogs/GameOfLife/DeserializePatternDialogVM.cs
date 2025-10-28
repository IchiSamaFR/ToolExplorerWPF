using GameOfLifeLibrary.Models;
using GameOfLifeLibrary.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolExplorerWPF.ViewModels.Dialogs.GameOfLife
{
    public partial class SerializePatternDialogVM : ObservableObject
    {
        [ObservableProperty]
        private string _patternName = string.Empty;

        [ObservableProperty]
        private ICollection<(int x, int y)> _patternCells = new List<(int x, int y)>();
    }
}
