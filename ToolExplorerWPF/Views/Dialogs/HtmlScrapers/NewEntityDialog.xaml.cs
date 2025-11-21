using ToolExplorerWPF.ViewModels.Dialogs.HtmlScrapers;
using Wpf.Ui.Controls;

namespace ToolExplorerWPF.Views.Dialogs.HtmlScrapers
{
    public partial class NewEntityDialog : ContentDialog
    {
        public NewEntityDialogVM ViewModel { get; }

        public NewEntityDialog(NewEntityDialogVM viewModel)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;

            InitializeComponent();
        }
    }
}
