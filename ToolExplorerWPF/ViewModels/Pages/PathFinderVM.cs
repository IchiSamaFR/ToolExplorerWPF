using AstarLibrary.Modules;
using Wpf.Ui.Controls;

namespace ToolExplorerWPF.ViewModels.Pages
{
    public partial class PathFinderVM : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        private AstarFinder _pathFinder = new AstarFinder();
        private CancellationTokenSource _cancellationTokenSource;

        [ObservableProperty]
        private bool _isRunning = false;

        [ObservableProperty]
        private int _rows = 5;
        [ObservableProperty]
        private int _columns = 5;
        [ObservableProperty]
        private int _speedDelay = 10;
        [ObservableProperty]
        private bool _isDiagonal = false;


        public ICollection<(int x, int y)> WallCells
        {
            get
            {
                return _pathFinder?.NodesList?.Where(n => n.IsWall).Select(n => n.Pos).ToList() ?? new List<(int x, int y)>();
            }
        }
        public ICollection<(int x, int y)> CheckCells
        {
            get
            {
                return _pathFinder?.NodesList?.Where(n => n.IsChecked).Select(n => n.Pos).ToList() ?? new List<(int x, int y)>();
            }
        }
        public ICollection<(int x, int y)> PathCells
        {
            get
            {
                return _pathFinder?.NodesList?.Where(n => n.PathFound).Select(n => n.Pos).ToList() ?? new List<(int x, int y)>();
            }
        }
        public (int x, int y) StartCell
        {
            get
            {
                return _pathFinder?.Starting?.Pos ?? new(0, 0);
            }
        }
        public (int x, int y) EndCell
        {
            get
            {
                return _pathFinder?.Ending?.Pos ?? new(0, 0);
            }
        }

        partial void OnColumnsChanged(int value)
        {
            _pathFinder.SetGridSize(Columns, Rows);
            _pathFinder.SetEndPos((Columns - 1, Rows - 1));
            NotifyAllProperties();
        }
        partial void OnRowsChanged(int value)
        {
            _pathFinder.SetGridSize(Columns, Rows);
            _pathFinder.SetEndPos((Columns - 1, Rows - 1));
            NotifyAllProperties();
        }
        partial void OnIsDiagonalChanged(bool value)
        {
            _pathFinder.IsDiagonal = value;
        }

        public void OnNavigatedTo()
        {
            _pathFinder.SetGridSize(Columns, Rows);
            _pathFinder.SetEndPos((Columns - 1, Rows - 1));
            NotifyAllProperties();
        }
        public void OnNavigatedFrom()
        {
        }

        private void NotifyAllProperties()
        {
            OnPropertyChanged(nameof(WallCells));
            OnPropertyChanged(nameof(CheckCells));
            OnPropertyChanged(nameof(PathCells));
            OnPropertyChanged(nameof(StartCell));
            OnPropertyChanged(nameof(EndCell));
        }

        [RelayCommand]
        private async Task Generate()
        {
            await Reset();

            _cancellationTokenSource = new CancellationTokenSource();
            _ = Generate(_cancellationTokenSource.Token);
        }
        private async Task Generate(CancellationToken token)
        {
            if (IsRunning)
            {
                return;
            }
            IsRunning = true;
            while (!_pathFinder.PathFinished)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                await Task.Delay(SpeedDelay);

                _pathFinder.SelectNextNode();

                OnPropertyChanged(nameof(CheckCells));
            }
            IsRunning = false;
            OnPropertyChanged(nameof(CheckCells));
            OnPropertyChanged(nameof(PathCells));
        }

        [RelayCommand]
        private async Task ApplyWall((int x, int y) node)
        {
            _pathFinder.ToggleWall(node.x, node.y);
            await Reset();
        }

        [RelayCommand]
        private async Task Reset()
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.Token.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
                await Task.Delay(SpeedDelay);
            }
            _pathFinder.Reset();
            IsRunning = false;
            NotifyAllProperties();
        }

        [RelayCommand]
        private async Task Clear()
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.Token.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
                await Task.Delay(SpeedDelay);
            }
            _pathFinder.Clear();
            IsRunning = false;
            NotifyAllProperties();
        }
    }
}
