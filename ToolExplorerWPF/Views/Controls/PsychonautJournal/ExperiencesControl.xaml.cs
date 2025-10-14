using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using PsychonautJournalLibrary.Entities.Interfaces;

namespace ToolExplorerWPF.Views.Controls.PsychonautJournal
{
    /// <summary>
    /// Logique d'interaction pour ExperiencesControl.xaml
    /// </summary>
    public partial class ExperiencesControl : UserControl
    {
        public ExperiencesControl()
        {
            InitializeComponent();
        }

        public IEnumerable<IExperience> Experiences
        {
            get => (IEnumerable<IExperience>)GetValue(ExperiencesProperty);
            set => SetValue(ExperiencesProperty, value);
        }

        public static readonly DependencyProperty ExperiencesProperty =
            DependencyProperty.Register(
                nameof(Experiences),
                typeof(IEnumerable<IExperience>),
                typeof(ExperiencesControl),
                new PropertyMetadata(new List<IExperience>()));
    }
}