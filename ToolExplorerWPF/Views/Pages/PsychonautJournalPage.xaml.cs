using ToolExplorerWPF.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace ToolExplorerWPF.Views.Pages
{
    /// <summary>
    /// Logique d'interaction pour PsychonautJournalPage.xaml
    /// </summary>
    public partial class PsychonautJournalPage : INavigableView<PsychonautJournalVM>
    {
        public PsychonautJournalVM ViewModel { get; }

        public PsychonautJournalPage(PsychonautJournalVM viewModel)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;

            InitializeComponent();
        }
    }
}
