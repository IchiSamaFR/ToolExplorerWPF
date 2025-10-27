using GameOfLifeLibrary.Engines;
using GameOfLifeLibrary.Models;
using GameOfLifeLibrary.Models.Rules;
using GameOfLifeLibrary.Patterns;
using Wpf.Ui.Controls;

namespace ToolExplorerWPF.ViewModels.Pages
{
    public partial class GameOfLifeVM : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;
        private int _inputDelay = 200;
        private CancellationTokenSource? _cancellationWidthToken;
        private CancellationTokenSource? _cancellationHeightToken;

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
        [ObservableProperty]
        private Pattern? _selectedPattern;
        [ObservableProperty]
        private List<Pattern> _patterns = new();

        // Generation
        [ObservableProperty]
        private bool _isStarted = false;
        [ObservableProperty]
        private bool _isRunning = false;

        public ICollection<(int x, int y)> AliveCells
        {
            get
            {
                return GameEngine?.Grid?.AliveCells.ToList() ?? new List<(int x, int y)>();
            }
        }

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


        // Gestion des changements des valeurs avec délai
        partial void OnZoomChanged(double value)
        {
            return;

            if (_cancellationWidthToken != null)
            {
                _cancellationWidthToken.Cancel();
            }
            _cancellationWidthToken = new CancellationTokenSource();
            _ = Task.Factory.StartNew(async () =>
            {
                try
                {
                    await Task.Delay(_inputDelay, _cancellationWidthToken.Token);
                    // Set zoom level
                }
                catch (TaskCanceledException)
                {
                    // Task was cancelled, do nothing
                }
            });
        }
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
            ImportPatterns();
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
            if (SelectedPattern != null)
            {
                PatternLoader.ApplyPattern(GameEngine.Grid, SelectedPattern, node.x, node.y);
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
        public void ImportPatterns()
        {
            Patterns = RlePatternImporter.FetchPatterns();
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
        public void Render()
        {
            OnPropertyChanged(nameof(AliveCells));
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