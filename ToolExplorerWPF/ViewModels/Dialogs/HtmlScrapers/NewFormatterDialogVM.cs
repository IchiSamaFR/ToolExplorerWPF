using HtmlScraperLibrary.Entities;
using HtmlScraperLibrary.Entities.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolExplorerWPF.ViewModels.Dialogs.HtmlScrapers
{
    public partial class NewFormatterDialogVM : ObservableObject
    {
        [ObservableProperty]
        private ATextFormatter _selectedFormatterType;

        [ObservableProperty]
        private List<ATextFormatter> _formatterTypes = new List<ATextFormatter>()
        {
            new PrefixFormatter(),
            new SuffixFormatter(),
            new ReplaceFormatter()
        };

        public NewFormatterDialogVM()
        {
            SelectedFormatterType = FormatterTypes.First();
        }
    }
}
