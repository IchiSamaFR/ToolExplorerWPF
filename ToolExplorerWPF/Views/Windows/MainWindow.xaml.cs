// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using ToolExplorerWPF.ViewModels.Windows;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace ToolExplorerWPF.Views.Windows
{
    public partial class MainWindow : INavigationWindow
    {
        public MainWindowVM ViewModel { get; }

        public MainWindow(
            MainWindowVM viewModel,
            IPageService pageService,
            ISnackbarService snackbarService,
            IContentDialogService contentDialogService,
            INavigationService navigationService
        )
        {
            ViewModel = viewModel;
            DataContext = ViewModel;

            SystemThemeWatcher.Watch(this);

            InitializeComponent();
            SetPageService(pageService);

            snackbarService.SetSnackbarPresenter(SnackbarPresenter);
            contentDialogService.SetContentPresenter(RootContentDialog);
            navigationService.SetNavigationControl(NavigationView);
        }

        #region INavigationWindow methods

        public INavigationView GetNavigation() => NavigationView;

        public bool Navigate(Type pageType) => NavigationView.Navigate(pageType);

        public void SetPageService(IPageService pageService) => NavigationView.SetPageService(pageService);

        public void ShowWindow() => Show();

        public void CloseWindow() => Close();

        #endregion INavigationWindow methods

        /// <summary>
        /// Raises the closed event.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Make sure that closing this window will begin the process of closing the application.
            Application.Current.Shutdown();
        }

        INavigationView INavigationWindow.GetNavigation()
        {
            throw new NotImplementedException();
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }
    }
}
