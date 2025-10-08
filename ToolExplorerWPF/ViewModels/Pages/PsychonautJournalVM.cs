using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PsychonautJournalLibrary.Entities.Interfaces;
using PsychonautJournalLibrary.Services;
using System.IO;
using ToolExplorerWPF.Helpers.Extensions;
using Wpf.Ui.Controls;

namespace ToolExplorerWPF.ViewModels.Pages
{
    public partial class PsychonautJournalVM : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(InputOrderedExperiences))]
        private IRoot? _input;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(OutputOrderedExperiences))]
        private IRoot? _output;

        [ObservableProperty]
        public Dictionary<string, Type> _versions = new Dictionary<string, Type>
        {
            { "v11", typeof(PsychonautJournalLibrary.Entities.v11.Root) },
            { "v12", typeof(PsychonautJournalLibrary.Entities.v12.Root) }
        };

        [ObservableProperty]
        private KeyValuePair<string, Type>? _selectedVersion;

        public IEnumerable<IExperience> InputOrderedExperiences
        {
            get
            {
                if (Input?.IExperiences == null)
                    return Enumerable.Empty<IExperience>();
                return Input.IExperiences.OrderByDescending(e => e.CreationDate);
            }
        }

        public IEnumerable<IExperience> OutputOrderedExperiences
        {
            get
            {
                if (Output?.IExperiences == null)
                    return Enumerable.Empty<IExperience>();
                return Output.IExperiences.OrderByDescending(e => e.CreationDate);
            }
        }

        [ObservableProperty]
        private string _selectedFilePath = string.Empty;

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom()
        {
        }

        private void InitializeViewModel()
        {
            _isInitialized = true;
        }


        partial void OnSelectedVersionChanged(KeyValuePair<string, Type>? value)
        {
            if (value == null || Input == null)
                return;

            if (value.GetType() == Input.GetType())
            {
                Output = Input.Clone();
                return;
            }

            switch (Input.Version)
            {
                case 11:
                    Output = JournalConverter.ConvertToV12((PsychonautJournalLibrary.Entities.v11.Root)Input);
                    break;
                case 12:
                    Output = JournalConverter.ConvertToV11((PsychonautJournalLibrary.Entities.v12.Root)Input);
                    break;
            }
        }

        [RelayCommand]
        public void ImportPsychonautDatas()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Importer un export Psychonaut Journal",
                Filter = "Fichiers JSON (*.json)|*.json|Tous les fichiers (*.*)|*.*",
                CheckFileExists = true,
                Multiselect = false
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                SelectedFilePath = Path.GetFileName(dialog.FileName);
                var json = File.ReadAllText(dialog.FileName);

                // Analyse rapide pour déterminer la version
                var jToken = JToken.Parse(json);
                if (jToken.Type != JTokenType.Object)
                    throw new InvalidDataException("Le fichier JSON racine n'est pas un objet.");

                var obj = (JObject)jToken;

                // Heuristiques:
                // v12 : presence "customUnits" (liste) ou "exportSource"
                // v11 : presence "customSubstances" et absence de "customUnits"
                bool hasCustomUnits = obj.TryGetValue("customUnits", StringComparison.OrdinalIgnoreCase, out _);
                bool hasExportSource = obj.TryGetValue("exportSource", StringComparison.OrdinalIgnoreCase, out _);
                bool hasCustomSubstances = obj.TryGetValue("customSubstances", StringComparison.OrdinalIgnoreCase, out _);

                IRoot parsed;

                if (hasCustomUnits || hasExportSource)
                {
                    // v12
                    parsed = JsonConvert.DeserializeObject<PsychonautJournalLibrary.Entities.v12.Root>(json)
                             ?? throw new InvalidDataException("Impossible de désérialiser en v12.Root.");
                }
                else if (hasCustomSubstances)
                {
                    // v11
                    parsed = JsonConvert.DeserializeObject<PsychonautJournalLibrary.Entities.v11.Root>(json)
                             ?? throw new InvalidDataException("Impossible de désérialiser en v11.Root.");
                }
                else
                {
                    throw new InvalidDataException("Format indéterminé: ni customUnits/exportSource (v12) ni customSubstances (v11) détecté.");
                }

                Input = parsed;
            }
            catch (Exception ex)
            {
                // Logging possible ici (trace, snackbar, etc.)
                System.Diagnostics.Debug.WriteLine($"Erreur import Psychonaut: {ex}");
            }
        }

        [RelayCommand]
        public void ExportPsychonautDatas()
        {
            if (Output == null)
                return;

            try
            {
                // Nom de fichier proposé
                var suggestedName = string.Empty;
                if (!string.IsNullOrWhiteSpace(SelectedFilePath))
                {
                    var noExt = Path.GetFileNameWithoutExtension(SelectedFilePath);
                    suggestedName = $"{noExt}_v{Output.Version}_converted_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                }
                else
                {
                    suggestedName = $"psychonaut_export_v{Output.Version}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                }

                var dialog = new SaveFileDialog
                {
                    Title = "Exporter Psychonaut Journal",
                    Filter = "Fichiers JSON (*.json)|*.json|Tous les fichiers (*.*)|*.*",
                    FileName = suggestedName,
                    OverwritePrompt = true,
                    AddExtension = true,
                    DefaultExt = ".json"
                };

                if (dialog.ShowDialog() != true)
                    return;

                // Sérialisation (Indented pour lisibilité)
                var json = JsonConvert.SerializeObject(
                    Output,
                    Formatting.Indented,
                    new JsonSerializerSettings
                    {
                    });

                File.WriteAllText(dialog.FileName, json);

                // (Optionnel) notifier succès (selon infra UI existante)
                System.Diagnostics.Debug.WriteLine($"Export Psychonaut OK: {dialog.FileName}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur export Psychonaut: {ex}");
            }
        }

    }
}
