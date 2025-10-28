using GameOfLifeLibrary.Models;
using GameOfLifeLibrary.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolExplorerWPF.ViewModels.Dialogs.GameOfLife
{
    public partial class DeserializePatternDialogVM : ObservableObject
    {
        private Pattern? _pattern;

        [ObservableProperty]
        private string _patternText = string.Empty;

        [ObservableProperty]
        private ICollection<(int x, int y)> _patternCells = new List<(int x, int y)>();

        partial void OnPatternTextChanged(string value)
        {
            try
            {
                _pattern = PatternImporter.Deserialize(value);
                PatternCells = _pattern.AliveCells.ToList();
            }
            catch (Exception)
            {
                PatternCells = new List<(int x, int y)>();
            }
        }
    }
}
