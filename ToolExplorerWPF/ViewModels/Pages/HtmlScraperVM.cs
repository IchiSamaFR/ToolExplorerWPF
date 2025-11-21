using HtmlAgilityPack;
using HtmlScraperLibrary;
using HtmlScraperLibrary.Builders;
using HtmlScraperLibrary.Components;
using HtmlScraperLibrary.Entities;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using PasswordLibrary;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows.Documents;
using ToolExplorerWPF.Models;
using ToolExplorerWPF.ViewModels.Dialogs.Passwords;
using ToolExplorerWPF.Views.Dialogs.HtmlScrapers;
using ToolExplorerWPF.Views.Dialogs.Passwords;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace ToolExplorerWPF.ViewModels.Pages
{
    public partial class HtmlScraperVM(IServiceProvider _serviceProvider, IContentDialogService _contentDialogService) : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private WebView2 _webView = new WebView2();
        [ObservableProperty]
        private RichTextBox _richTextBox = new RichTextBox();

        [ObservableProperty]
        private bool _isDialogOpened;

        [ObservableProperty]
        private bool _isModuleConstructor = true;
        [ObservableProperty]
        private string _url;

        [ObservableProperty]
        private RootEntity _rootEntity = new RootEntity();

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
            RootEntity.Children.Add(new SelectEntity());

            _isInitialized = true;
            Url = "https://www.google.com/search?q=.net";
            InitWebView2();
        }

        [RelayCommand]
        public void LoadUrl()
        {
            if(string.IsNullOrEmpty(Url))
            {
                return;
            }
            WebView.Source = new Uri(Url);
        }

        [RelayCommand]
        public void ImportHtml()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Html source";
            dialog.DefaultExt = ".html";
            dialog.Filter = "Html files (*.html;*.htm)|*.html;*.htm|Txt files (*.txt)|*.txt|All files |*.*";

            if (dialog.ShowDialog() == true)
            {
                Url = $"file:///{dialog.FileName}";
                WebView.Source = new Uri(Url ?? string.Empty);
            }
        }

        [RelayCommand]
        public void ImportConfigFile()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Config";
            dialog.DefaultExt = ".xml";
            dialog.Filter = "Config files (*.xml)|*.xml";

            if (dialog.ShowDialog() == true)
            {
                Url = $"file:///{dialog.FileName}";
                RootEntity = EntityBuilder.BuildFromFilePath(dialog.FileName) as RootEntity ?? new RootEntity();
                //SetDocumentText(File.ReadAllText(dialog.FileName));
            }
        }

        [RelayCommand]
        public async Task ExecuteConfig()
        {
            var document = new HtmlDocument();
            document.LoadHtml(await GetDocumentHtmlAsync());

            var json = await RootEntity.Extract(document);
            var jsonTxt = JsonSerializer.Serialize(json, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });
        }

        [RelayCommand]
        public async Task OpenNewEntityDialog(AParentEntity entity)
        {
            IsDialogOpened = true;
            var content = _serviceProvider.GetService(typeof(NewEntityDialog)) as NewEntityDialog;

            var result = await _contentDialogService.ShowSimpleDialogAsync(
                new SimpleContentDialogCreateOptions()
                {
                    Title = "Create new entity",
                    Content = content,
                    PrimaryButtonText = "Create",
                    CloseButtonText = "Cancel",
                }
            );

            switch (result)
            {
                case ContentDialogResult.Primary:
                    entity.Children.Add(content.ViewModel.SelectedEntityType);
                    // TO DO
                    break;
                case ContentDialogResult.Secondary:
                case ContentDialogResult.None:
                default:
                    break;
            }
            IsDialogOpened = false;
        }

        [RelayCommand]
        public async Task OpenNewFormatterDialog()
        {
            IsDialogOpened = true;
            var content = _serviceProvider.GetService(typeof(NewFormatterDialog)) as NewFormatterDialog;

            var result = await _contentDialogService.ShowSimpleDialogAsync(
                new SimpleContentDialogCreateOptions()
                {
                    Title = "Create new formatter",
                    Content = content,
                    PrimaryButtonText = "Create",
                    CloseButtonText = "Cancel",
                }
            );

            switch (result)
            {
                case ContentDialogResult.Primary:

                    break;
                case ContentDialogResult.Secondary:
                case ContentDialogResult.None:
                default:
                    break;
            }
            IsDialogOpened = false;
        }

        #region WebView2 commands
        private async void InitWebView2()
        {
            await WebView.EnsureCoreWebView2Async(await CoreWebView2Environment.CreateAsync(null, null, new CoreWebView2EnvironmentOptions("")));
            LoadUrl();
        }
        public async Task<string> GetDocumentHtmlAsync()
        {
            string script =
                @"(function() {
                    return document.documentElement.innerHTML;
                })()";

            string docHtml = await WebView.CoreWebView2.ExecuteScriptAsync(script);
            docHtml = docHtml?.Trim('"').Replace("\\u003C", "<").Replace("\\u003E", ">").Replace("\\u0026", "&").Replace("\\n", "").Replace("\\t", "").Replace("\\\\", "\\").Replace("\\\"", "\"");

            return docHtml;
        }
        #endregion


        #region RichTextBox commands
        private string GetDocumentText()
        {
            TextRange textRange = new TextRange(RichTextBox.Document.ContentStart, RichTextBox.Document.ContentEnd);
            return textRange.Text;
        }
        private void SetDocumentText(string text)
        {
            var textRange = new TextRange(RichTextBox.Document.ContentStart, RichTextBox.Document.ContentEnd);
            textRange.Text = "";
            textRange.Text = text;
        }
        #endregion
    }
}
