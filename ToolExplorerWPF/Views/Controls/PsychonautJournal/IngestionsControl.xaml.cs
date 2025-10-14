using PsychonautJournalLibrary.Entities.Interfaces;
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

namespace ToolExplorerWPF.Views.Controls.PsychonautJournal
{
    /// <summary>
    /// Logique d'interaction pour IngestionsControl.xaml
    /// </summary>
    public partial class IngestionsControl : UserControl
    {
        public IngestionsControl()
        {
            InitializeComponent();
        }

        public IEnumerable<IIngestion> Ingestions
        {
            get => (IEnumerable<IIngestion>)GetValue(IngestionsProperty);
            set => SetValue(IngestionsProperty, value);
        }

        public static readonly DependencyProperty IngestionsProperty =
            DependencyProperty.Register(
                nameof(Ingestions),
                typeof(IEnumerable<IIngestion>),
                typeof(IngestionsControl),
                new PropertyMetadata(new List<IIngestion>()));
    }
}
