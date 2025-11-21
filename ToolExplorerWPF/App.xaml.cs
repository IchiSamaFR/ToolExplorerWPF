// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
using System.Windows.Threading;
using ToolExplorerWPF.Services;
using ToolExplorerWPF.ViewModels.Dialogs.HtmlScrapers;
using ToolExplorerWPF.ViewModels.Pages;
using ToolExplorerWPF.ViewModels.Windows;
using ToolExplorerWPF.Views.Dialogs.HtmlScrapers;
using ToolExplorerWPF.Views.Pages;
using ToolExplorerWPF.Views.Windows;
using Wpf.Ui;

namespace ToolExplorerWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
        // https://docs.microsoft.com/dotnet/core/extensions/generic-host
        // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
        // https://docs.microsoft.com/dotnet/core/extensions/configuration
        // https://docs.microsoft.com/dotnet/core/extensions/logging
        private static readonly IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)); })
            .ConfigureServices((context, services) =>
            {
                services.AddHostedService<ApplicationHostService>();

                // Page resolver service
                services.AddSingleton<IPageService, PageService>();

                // Theme manipulation
                services.AddSingleton<IThemeService, ThemeService>();

                // TaskBar manipulation
                services.AddSingleton<ITaskBarService, TaskBarService>();

                services.AddSingleton<IContentDialogService, ContentDialogService>();
                services.AddSingleton<ISnackbarService, SnackbarService>();

                // Service containing navigation, same as INavigationWindow... but without window
                services.AddSingleton<INavigationService, NavigationService>();

                // Dialogs
                services.AddTransient<NewEntityDialog>();
                services.AddTransient<NewEntityDialogVM>();

                services.AddTransient<NewFormatterDialog>();
                services.AddTransient<NewFormatterDialogVM>();

                // Main window with navigation
                services.AddSingleton<INavigationWindow, MainWindow>();
                services.AddSingleton<MainWindowVM>();


                // Main window with navigation
                services.AddSingleton<DashboardPage>();
                services.AddSingleton<DashboardVM>();
                services.AddSingleton<NoisePage>();
                services.AddSingleton<NoiseVM>();
                services.AddSingleton<PathFinderPage>();
                services.AddSingleton<PathFinderVM>();
                services.AddSingleton<HtmlScraperPage>();
                services.AddSingleton<HtmlScraperVM>();
                services.AddSingleton<PasswordPage>();
                services.AddSingleton<PasswordVM>();
                services.AddSingleton<PsychonautJournalPage>();
                services.AddSingleton<PsychonautJournalVM>();
                services.AddSingleton<GameOfLifePage>();
                services.AddSingleton<GameOfLifeVM>();

                services.AddSingleton<SettingsPage>();
                services.AddSingleton<SettingsVM>();
            }).Build();

        /// <summary>
        /// Gets registered service.
        /// </summary>
        /// <typeparam name="T">Type of the service to get.</typeparam>
        /// <returns>Instance of the service or <see langword="null"/>.</returns>
        public static T GetService<T>()
            where T : class
        {
            return _host.Services.GetService(typeof(T)) as T;
        }

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private void OnStartup(object sender, StartupEventArgs e)
        {
            _host.Start();
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();

            _host.Dispose();
        }

        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
        }
    }
}
