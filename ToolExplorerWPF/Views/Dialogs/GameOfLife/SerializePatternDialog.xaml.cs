using System.Windows.Controls;
using ToolExplorerWPF.ViewModels.Dialogs.GameOfLife;
using Wpf.Ui.Controls;

namespace ToolExplorerWPF.Views.Dialogs.GameOfLife
{
    /// <summary>
    /// Logique d'interaction pour DeserializePatternDialog.xaml
    /// </summary>
    public partial class SerializePatternDialog : ContentDialog
    {
        public SerializePatternDialogVM ViewModel { get; } = new ();

        public SerializePatternDialog()
        {
            DataContext = ViewModel;
            InitializeComponent();
        }
    }
}
