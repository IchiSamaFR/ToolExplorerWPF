using AstarLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ToolExplorerWPF.Models.PathFinder;
using ToolExplorerWPF.Services;
using Wpf.Ui.Controls;

namespace ToolExplorerWPF.ViewModels.Pages
{
    public partial class PathFinderVM : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        private PathFinder _pathFinder;
        private CancellationTokenSource _cancellationTokenSource;

        [ObservableProperty]
        private int _rows = 5;
        [ObservableProperty]
        private int _columns = 5;
        [ObservableProperty]
        private int _speedDelay = 10;
        [ObservableProperty]
        private bool _isDiagonal = false;

        [ObservableProperty]
        private List<NodeModel> _items = new List<NodeModel>();


        partial void OnColumnsChanged(int value)
        {
            SetItems();
            Reset();
        }
        partial void OnRowsChanged(int value)
        {
            SetItems();
            Reset();
        }
        partial void OnIsDiagonalChanged(bool value)
        {
            Reset();
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

            SetItems();
        }
        private void SetItems()
        {
            var tmp = new List<NodeModel>();
            _pathFinder = new PathFinder(Columns, Rows);

            for (int y = 0; y < _pathFinder.Height; y++)
            {
                for (int x = 0; x < _pathFinder.Width; x++)
                {
                    var n = _pathFinder.Nodes[x, y];
                    tmp.Add(new NodeModel
                    {
                        Node = n
                    });
                }
            }

            Items = tmp;
        }

        [RelayCommand]
        public async Task Generate()
        {
            await Reset();
            _pathFinder.IsDiagonal = IsDiagonal;

            _cancellationTokenSource = new CancellationTokenSource();
            await Generate(_cancellationTokenSource.Token);
        }
        private async Task Generate(CancellationToken token)
        {

            while (!_pathFinder.PathFinished)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                await Task.Delay(SpeedDelay);

                var node = _pathFinder.SelectNextNode();

                if(node == null)
                {
                    break;
                }
                Items.First(it => it.Node == node).Notify();
            }
            Items.ForEach(it => it.Notify());
        }

        [RelayCommand]
        public void ApplyWall(NodeModel node)
        {
            Reset();
            node.IsWall = !node.IsWall;
            node.Notify();
        }
        private async Task Reset()
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.Token.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
                await Task.Delay(SpeedDelay);
            }

            _pathFinder.Reset();
            Items.ForEach(it => it.Notify());
        }

    }
}
