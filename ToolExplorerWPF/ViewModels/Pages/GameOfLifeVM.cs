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

        [ObservableProperty]
        private GameEngine _gameEngine;

        // View
        [ObservableProperty]
        private int _rows = 20;
        [ObservableProperty]
        private int _columns = 20;
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

        // Collection observable pour le binding
        [ObservableProperty]
        private ICell[,] _cells = new ICell[0, 0];

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

            var grid = new ObservableGrid(Columns, Rows);
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
        public void NextAge()
        {
            GameEngine.NextGeneration();
            OnPropertyChanged(nameof(GameEngine));
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
        public void Reset()
        {
            IsRunning = false;
            Generate();
        }

        private async Task RunGenerationsAsync()
        {
            IsStarted = true;
            while (IsRunning)
            {
                NextAge();
                await Task.Delay(SpeedDelay);
            }
        }
    }
}