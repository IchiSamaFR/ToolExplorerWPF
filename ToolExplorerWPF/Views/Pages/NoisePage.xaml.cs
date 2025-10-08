using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ToolExplorerWPF.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace ToolExplorerWPF.Views.Pages
{
    /// <summary>
    /// Logique d'interaction pour NoisePage.xaml
    /// </summary>
    public partial class NoisePage : INavigableView<NoiseVM>
    {
        public NoiseVM ViewModel { get; }

        public NoisePage(NoiseVM viewModel)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;

            InitializeComponent();
        }
    }
}
