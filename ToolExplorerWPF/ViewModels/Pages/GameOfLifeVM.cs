using GameOfLifeLibrary.Engines;
using GameOfLifeLibrary.Helpers;
using GameOfLifeLibrary.Models;
using GameOfLifeLibrary.Models.Cells;
using GameOfLifeLibrary.Models.Rules;
using GameOfLifeLibrary.Patterns;
using ToolExplorerWPF.Models.GameOfLife;
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
        private int _width = 30;
        [ObservableProperty]
        private int _height = 30;
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
        [ObservableProperty]
        private ICell[,] _cells = new ICell[0, 0];

        public int Generation
        {
            get
            {
                return GameEngine?.Generation ?? 0;
            }
            set
            {
                if (GameEngine == null)
                {
                    return;
                }
                GameEngine.GoToGeneration(Math.Min(Math.Max(value, 0), TotalGenerations));
                OnPropertyChanged(nameof(Generation));
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
        partial void OnWidthChanged(int value)
        {
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
                    GameEngine.Grid.SetWidth(value);
                    Cells = GameEngine.Grid.Cells;
                }
                catch (TaskCanceledException)
                {
                    // Task was cancelled, do nothing
                }
            });
        }
        partial void OnHeightChanged(int value)
        {
            if (_cancellationHeightToken != null)
            {
                _cancellationHeightToken.Cancel();
            }
            _cancellationHeightToken = new CancellationTokenSource();
            _ = Task.Factory.StartNew(async () =>
            {
                try
                {
                    await Task.Delay(_inputDelay, _cancellationHeightToken.Token);
                    GameEngine.Grid.SetHeight(value);
                    Cells = GameEngine.Grid.Cells;
                }
                catch (TaskCanceledException)
                {
                    // La tâche a été annulée, ne rien faire
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

            var grid = new ObservableGrid(Width, Height);
            var rules = new ModulableRules(MinSurvivalNeighbors, MaxSurvivalNeighbors, BirthNeighbors);
            GameEngine = GameEngineFactory.CreateCustomGameEngine(grid, rules);

            // Met à jour la collection observable
            Cells = GameEngine.Grid.Cells;
        }

        [RelayCommand]
        public void ApplyLife(ICell node)
        {
            if (SelectedPattern != null)
            {
                PatternLoader.ApplyPattern(GameEngine.Grid, SelectedPattern, node.X, node.Y);
                return;
            }

            node.IsAlive = !node.IsAlive;
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
            Patterns = RlePatternImporter.LoadAllRlePatterns();
        }

        // Commandes de contrôle du jeu
        [RelayCommand]
        public void Reset()
        {
            IsRunning = false;
            IsStarted = false;
            GameEngine.Reset();
            OnPropertyChanged(nameof(Generation));
            OnPropertyChanged(nameof(TotalGenerations));
            OnPropertyChanged(nameof(Cells));
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
                GameEngine.StartGeneration();
                IsStarted = true;
            }
            GameEngine.NextGeneration();
            OnPropertyChanged(nameof(Generation));
            OnPropertyChanged(nameof(TotalGenerations));
        }
    }
}