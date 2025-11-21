using HtmlScraperLibrary.Entities;
using HtmlScraperLibrary.Entities.Formatters;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ToolExplorerWPF.Views.Controls.HtmlScraper
{
    public class FormatterTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? PrefixFormatterTemplate { get; set; }
        public DataTemplate? SuffixFormatterTemplate { get; set; }
        public DataTemplate? ReplaceFormatterTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
            {
                return null;
            }

            if (item is PrefixFormatter)
            {
                return PrefixFormatterTemplate;
            }
            if (item is SuffixFormatter)
            {
                return SuffixFormatterTemplate;
            }
            if (item is ReplaceFormatter)
            {
                return ReplaceFormatterTemplate;
            }

            return null;
        }
    }
}