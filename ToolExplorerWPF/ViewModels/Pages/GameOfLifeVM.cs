using AstarLibrary;
using GameOfLifeLibrary.Engines;
using GameOfLifeLibrary.Helpers;
using GameOfLifeLibrary.Models;
using GameOfLifeLibrary.Rules;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml.Linq;
using ToolExplorerWPF.Models.GameOfLife;
using ToolExplorerWPF.Models.PathFinder;
using ToolExplorerWPF.Services;
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
                if (GameEngine == null)
                {
                    return;
                }
                GameEngine.GoToGeneration(Math.Min(Math.Max(value, 0), TotalGenerations));
                OnPropertyChanged(nameof(GameEngine));
            }
        }
        public int TotalGenerations
        {
            get
            {
                return GameEngine?.TotalGenerations ?? 0;
            }
        }

        // Collection observable pour le binding
        [ObservableProperty]
        private ICell[,] _cells = new ICell[0, 0];

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
            if(_cancellationHeightToken != null)
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
        }

        [RelayCommand]
        public void Generate()
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
            node.IsAlive = !node.IsAlive;
        }

        [RelayCommand]
        public void ApplyRules()
        {
            var rules = new ModulableRules(MinSurvivalNeighbors, MaxSurvivalNeighbors, BirthNeighbors);
            GameEngine.SetRules(rules);
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

        [RelayCommand]
        public void Reset()
        {
            IsRunning = false;
            IsStarted = false;
            GameEngine.Reset();
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
            IsRunning = false;
            GameEngine.Reset();
            OnPropertyChanged(nameof(Generation));
            OnPropertyChanged(nameof(TotalGenerations));
            OnPropertyChanged(nameof(Cells));
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
                GameEngine.StartGeneration();
                IsStarted = true;
            }
            GameEngine.NextGeneration();
            OnPropertyChanged(nameof(Generation));
            OnPropertyChanged(nameof(TotalGenerations));
        }
    }
}