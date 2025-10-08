// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace ToolExplorerWPF.ViewModels.Windows
{
    public partial class MainWindowVM : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = "WPF UI - ToolExplorerWPF";

        [ObservableProperty]
        private ObservableCollection<object> _menuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "Home",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(Views.Pages.DashboardPage)
            },
            new NavigationViewItem()
            {
                Content = "Perlin noise",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Blur24 },
                TargetPageType = typeof(Views.Pages.NoisePage)
            },
            new NavigationViewItem()
            {
                Content = "Path finder",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Location24 },
                TargetPageType = typeof(Views.Pages.PathFinderPage)
            },
            new NavigationViewItem()
            {
                Content = "HTML Scraper",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Globe24 },
                TargetPageType = typeof(Views.Pages.HtmlScraperPage)
            },
            new NavigationViewItem()
            {
                Content = "Password",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Password24 },
                TargetPageType = typeof(Views.Pages.PasswordPage)
            },
            new NavigationViewItem()
            {
                Content = "Psychonaut Journal",
                Icon = new SymbolIcon { Symbol = SymbolRegular.BookOpen24 },
                TargetPageType = typeof(Views.Pages.PsychonautJournalPage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<object> _footerMenuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(Views.Pages.SettingsPage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new()
        {
            new MenuItem { Header = "Home", Tag = "tray_home" }
        };
    }
}
