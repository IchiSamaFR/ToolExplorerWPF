using ToolExplorerWPF.ViewModels.Dialogs.HtmlScrapers;
using Wpf.Ui.Controls;

namespace ToolExplorerWPF.Views.Dialogs.HtmlScrapers
{
    public partial class NewFormatterDialog : ContentDialog
    {
        public NewFormatterDialogVM ViewModel { get; }

        public NewFormatterDialog(NewFormatterDialogVM viewModel)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;

            InitializeComponent();
        }
    }
}
