using System.Windows.Controls;
using ToolExplorerWPF.ViewModels.Dialogs.GameOfLife;
using Wpf.Ui.Controls;

namespace ToolExplorerWPF.Views.Dialogs.GameOfLife
{
    /// <summary>
    /// Logique d'interaction pour DeserializePatternDialog.xaml
    /// </summary>
    public partial class DeserializePatternDialog : ContentDialog
    {
        public DeserializePatternDialogVM ViewModel { get; } = new ();

        public DeserializePatternDialog()
        {
            DataContext = ViewModel;
            InitializeComponent();
        }
    }
}
