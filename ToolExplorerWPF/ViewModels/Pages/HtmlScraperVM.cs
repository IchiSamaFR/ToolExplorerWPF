using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using HtmlScraperLibrary;
using HtmlScraperLibrary.Components;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json.Nodes;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;

namespace ToolExplorerWPF.ViewModels.Pages
{
    public partial class HtmlScraperVM : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private WebView2 _webView = new WebView2();

        [ObservableProperty]
        private string _url;
        [ObservableProperty]
        private string _urlSource;

        [ObservableProperty]
        private string _xamlConfig;

        [ObservableProperty]
        private ObservableCollection<JsonArray> _jsonArrays = new ObservableCollection<JsonArray>();

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
            Url = "https://www.google.com/search?q=.net";
            InitWebView2();
        }

        [RelayCommand]
        public void LoadPage()
        {
            if(string.IsNullOrEmpty(Url))
            {
                return;
            }
            WebView.Source = new Uri(Url);
        }
        [RelayCommand]
        public void LoadLocalHtml()
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

        string test = "";
        [RelayCommand]
        public void LoadXMLConfigHtml()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Html source";
            dialog.DefaultExt = ".html";
            dialog.Filter = "Config files (*.xml)|*.xml";

            if (dialog.ShowDialog() == true)
            {
                XamlConfig = string.Empty;
                XamlConfig = File.ReadAllText(dialog.FileName);
                test = dialog.FileName;
            }
        }
        [RelayCommand]
        public async Task ApplyXMLConfigLocal()
        {
            JsonArrays.Clear();
            var document = new HtmlDocument();
            var html = await GetDocumentHtmlAsync();
            document.LoadHtml(html);

            var context = ComponentConfig.LoadFromXML(test);
            var scrapper = Scraper.Build(context);
            foreach (var item in context.Scrapers)
            {
                JsonArrays.Add(scrapper.ActionLocal(item, document));
                Debug.WriteLine(JsonArrays.First().ToString());
            }
        }

        #region WebView2 commands
        private async void InitWebView2()
        {
            await WebView.EnsureCoreWebView2Async(await CoreWebView2Environment.CreateAsync(null, null, new CoreWebView2EnvironmentOptions("")));
            LoadPage();
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
    }
}
