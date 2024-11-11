using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ToolExplorerWPF.Views.Controls
{
    public class WebView2Ex : WebView2
    {
        #region -- PROPERTIES --

        #region ItemTemplate Property
        public string WebSource
        {
            get
            {
                return (string)GetValue(WebSourceProperty);
            }
            set
            {
                SetValue(WebSourceProperty, value);
            }
        }

        public static readonly DependencyProperty WebSourceProperty = DependencyProperty.RegisterAttached("WebSource", typeof(string), typeof(WebView2Ex), new PropertyMetadata(null, WebSourceChanged));
        public static void WebSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is WebView2Ex) || e.NewValue == null)
                return;

            var webView = (WebView2Ex)obj;
            webView.Source = new Uri(e.NewValue.ToString());
        }
        #endregion
        #endregion

        public WebView2Ex()
        {
            Init();
        }
        private async void Init()
        {
            await EnsureCoreWebView2Async(await CoreWebView2Environment.CreateAsync(null, null, new CoreWebView2EnvironmentOptions("")));
        }

        public async Task<string> GetDocumentHtmlAsync()
        {
            string script = 
                @"(function() {
                    return document.documentElement.innerHTML;
                })()";

            string docHtml = await CoreWebView2.ExecuteScriptAsync(script);
            docHtml = docHtml?.Trim('"');

            return docHtml;
        }
    }
}
