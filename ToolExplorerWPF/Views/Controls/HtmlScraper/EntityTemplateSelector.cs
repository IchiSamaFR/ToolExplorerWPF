using HtmlScraperLibrary.Entities;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ToolExplorerWPF.Views.Controls.HtmlScraper
{
    public class EntityTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? RootEntityTemplate { get; set; }
        public DataTemplate? LoopEntityTemplate { get; set; }
        public DataTemplate? SelectEntityTemplate { get; set; }
        public DataTemplate? TextEntityTemplate { get; set; }
        public DataTemplate? AttributEntityTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
            {
                return null;
            }

            if (item is AttributeEntity)
            {
                return AttributEntityTemplate;
            }
            if (item is TextEntity)
            {
                return TextEntityTemplate;
            }
            if (item is SelectEntity)
            {
                return SelectEntityTemplate;
            }
            if (item is LoopEntity)
            {
                return LoopEntityTemplate;
            }
            if (item is RootEntity)
            {
                return RootEntityTemplate;
            }

            return null;
        }
    }
}