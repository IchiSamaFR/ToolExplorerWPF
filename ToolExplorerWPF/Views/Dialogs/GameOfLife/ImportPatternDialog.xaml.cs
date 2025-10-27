using System.Windows.Controls;
using ToolExplorerWPF.ViewModels.Dialogs.GameOfLife;
using Wpf.Ui.Controls;

namespace ToolExplorerWPF.Views.Dialogs.GameOfLife
{
    /// <summary>
    /// Logique d'interaction pour ImportPatternDialog.xaml
    /// </summary>
    public partial class ImportPatternDialog : ContentDialog
    {
        public ImportPatternDialogVM ViewModel { get; } = new ();

        public ImportPatternDialog()
        {
            DataContext = ViewModel;
            InitializeComponent();
        }
    }
}
