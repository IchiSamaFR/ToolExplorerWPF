using AstarLibrary;
using GameOfLifeLibrary.Engines;
using GameOfLifeLibrary.Models;
using GameOfLifeLibrary.Models.Rules;
using GameOfLifeLibrary.Patterns;
using PasswordLibrary;
using ToolExplorerWPF.Views.Dialogs.GameOfLife;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace ToolExplorerWPF.ViewModels.Pages
{
    public partial class GameOfLifeVM : ObservableObject, INavigationAware
    {
        private readonly IContentDialogService _contentDialogService = null!;
        private bool _isInitialized = false;

        [ObservableProperty]
        private GameEngine _gameEngine = null!;

        // View
        [ObservableProperty]
        private double _zoom = 1;
        [ObservableProperty]
        private int _speedDelay = 100;

        // Rules
        [ObservableProperty]
        private int _minSurvivalNeighbors = 2;
        [ObservableProperty]
        private int _maxSurvivalNeighbors = 3;
        [ObservableProperty]
        private int _birthNeighbors = 3;

        // Patterns
        private Pattern _emptyPattern = new Pattern(string.Empty, 0, 0, Enumerable.Empty<(int x, int y)>());

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PreviewCells))]
        private Pattern? _selectedPattern;

        public List<Pattern?> Patterns
        {
            get
            {
                var lst = PatternImporter.LoadExistingPatterns();
                lst.Insert(0, _emptyPattern);
                return lst;
            }
        }

        public ICollection<(int x, int y)> AliveCells
        {
            get
            {
                return GameEngine?.Grid?.AliveCells.ToList() ?? new List<(int x, int y)>();
            }
        }
        public ICollection<(int x, int y)> PreviewCells
        {
            get
            {
                return SelectedPattern?.AliveCells.ToList() ?? new List<(int x, int y)>();
            }
        }

        // Generation
        [ObservableProperty]
        private bool _isStarted = false;
        [ObservableProperty]
        private bool _isRunning = false;

        public int Generation
        {
            get
            {
                return GameEngine?.Generation ?? 0;
            }
            set
            {
                SetGeneration(value);
            }
        }
        public int TotalGenerations
        {
            get
            {
                return GameEngine?.TotalGenerations ?? 0;
            }
        }

        public GameOfLifeVM(IContentDialogService contentDialogService)
        {
            _contentDialogService = contentDialogService;

            OnPropertyChanged(nameof(Patterns));
        }

        // Gestion des changements des valeurs avec délai
        partial void OnMinSurvivalNeighborsChanged(int value)
        {
            if (value < 0)
                MinSurvivalNeighbors = 0;
            else if (value > MaxSurvivalNeighbors)
                MinSurvivalNeighbors = MaxSurvivalNeighbors;
        }
        partial void OnMaxSurvivalNeighborsChanged(int value)
        {
            if (value > 4)
                MaxSurvivalNeighbors = 4;
            else if (value < MinSurvivalNeighbors)
                MaxSurvivalNeighbors = MinSurvivalNeighbors;
        }
        partial void OnBirthNeighborsChanged(int value)
        {
            if (value < 0)
                BirthNeighbors = 0;
            else if (value > 4)
                BirthNeighbors = 4;
        }

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
            Generate();
            LoadExistingPatterns();
        }

        private void Generate()
        {
            IsStarted = false;

            var rules = new ModulableRules(MinSurvivalNeighbors, MaxSurvivalNeighbors, BirthNeighbors);
            GameEngine = new GameEngine();
            GameEngine.SetRules(rules);
        }

        [RelayCommand]
        public void ApplyLife((int x, int y) node)
        {
            if (SelectedPattern != null && SelectedPattern != _emptyPattern)
            {
                PatternLoader.ApplyPattern(GameEngine.Grid, SelectedPattern, node.x, node.y);
                OnPropertyChanged(nameof(AliveCells));
                return;
            }

            GameEngine.Grid.SetCell(node.x, node.y);
            OnPropertyChanged(nameof(AliveCells));
        }

        [RelayCommand]
        public void ApplyDefaultRules()
        {
            var rules = new ConwayRules();
            MinSurvivalNeighbors = rules.MinSurvivalNeighbors;
            MaxSurvivalNeighbors = rules.MaxSurvivalNeighbors;
            BirthNeighbors = rules.BirthNeighbors;
            GameEngine.SetRules(rules);
        }

        // Patterns
        [RelayCommand]
        public void UnselectPattern()
        {
            SelectedPattern = null;
        }

        [RelayCommand]
        public void LoadExistingPatterns()
        {
            OnPropertyChanged(nameof(Patterns));
        }

        [RelayCommand]
        public async Task DeserializePattern()
        {
            var content = new DeserializePatternDialog();


            var result = await _contentDialogService.ShowSimpleDialogAsync(
                new SimpleContentDialogCreateOptions()
                {
                    Title = "Deserialize pattern",
                    Content = content,
                    PrimaryButtonText = "Create",
                    CloseButtonText = "Cancel",
                }
            );

            if (result != ContentDialogResult.Primary || string.IsNullOrWhiteSpace(content.ViewModel.PatternText))
            {
                return;
            }

            PatternImporter.InsertNewPattern(content.ViewModel.PatternText);
            LoadExistingPatterns();
        }

        [RelayCommand]
        public async Task SerializePattern()
        {
            var cells = AliveCells.ToList();
            if (cells == null || cells.Count == 0)
            {
                return;
            }

            // Find min and max coordinates
            int minX = cells.Min(c => c.x);
            int minY = cells.Min(c => c.y);

            // Shift all cells so the top-left is (0,0)
            var normalizedCells = cells
                .Select(c => (c.x - minX, c.y - minY))
                .ToList<(int x, int y)>();

            var content = new SerializePatternDialog();
            content.ViewModel.PatternName = "NewPattern";
            content.ViewModel.PatternCells = normalizedCells;

            var result = await _contentDialogService.ShowSimpleDialogAsync(
                new SimpleContentDialogCreateOptions()
                {
                    Title = "Serialize pattern",
                    Content = content,
                    PrimaryButtonText = "Create",
                    CloseButtonText = "Cancel",
                }
            );

            if (result != ContentDialogResult.Primary || string.IsNullOrWhiteSpace(content.ViewModel.PatternName))
            {
                return;
            }

            // Recalculate width and height from normalized cells
            int width = normalizedCells.Max(c => c.x) + 1;
            int height = normalizedCells.Max(c => c.y) + 1;

            var pattern = new Pattern(
                content.ViewModel.PatternName,
                width,
                height,
                content.ViewModel.PatternCells
            );
            PatternImporter.InsertNewPattern(pattern);
            LoadExistingPatterns();
        }

        [RelayCommand]
        public void Reset()
        {
            IsRunning = false;
            IsStarted = false;
            GameEngine.Reset();
            OnPropertyChanged(nameof(Generation));
            OnPropertyChanged(nameof(TotalGenerations));
            OnPropertyChanged(nameof(AliveCells));
        }

        [RelayCommand]
        public void StartPause()
        {
            IsRunning = !IsRunning;
            if (IsRunning)
            {
                _ = RunGenerationsAsync();
            }
        }

        [RelayCommand]
        public void Clear()
        {
            GameEngine.Clear();
            OnPropertyChanged(nameof(AliveCells));
        }

        private async Task RunGenerationsAsync()
        {
            while (IsRunning)
            {
                NextGeneration();
                await Task.Delay(SpeedDelay);
            }
        }

        [RelayCommand]
        public void NextGeneration()
        {
            if (!IsStarted)
            {
                GameEngine.SetRules(new ModulableRules(MinSurvivalNeighbors, MaxSurvivalNeighbors, BirthNeighbors));
                IsStarted = true;
            }
            GameEngine.NextGeneration();
            OnPropertyChanged(nameof(Generation));
            OnPropertyChanged(nameof(TotalGenerations));
            OnPropertyChanged(nameof(AliveCells));
        }

        public void SetGeneration(int generation)
        {
            if (GameEngine == null)
            {
                return;
            }
            GameEngine.GoToGeneration(Math.Min(Math.Max(generation, 0), TotalGenerations));
            OnPropertyChanged(nameof(Generation));
            OnPropertyChanged(nameof(AliveCells));
        }
    }
}