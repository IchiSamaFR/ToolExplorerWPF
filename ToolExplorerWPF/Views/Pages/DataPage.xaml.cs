// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using ToolExplorerWPF.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace ToolExplorerWPF.Views.Pages
{
    public partial class DataPage : INavigableView<DataVM>
    {
        public DataVM ViewModel { get; }

        public DataPage(DataVM viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
