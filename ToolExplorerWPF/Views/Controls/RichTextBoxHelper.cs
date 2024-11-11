using System;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using Wpf.Ui.Controls;

namespace ToolExplorerWPF.Views.Controls
{
    public static class RichTextBoxHelper
    {
        public static readonly DependencyProperty TextChProperty =
        DependencyProperty.RegisterAttached(
            "TextChanging",
            typeof(bool),
            typeof(RichTextBoxHelper),
            new PropertyMetadata(false, null));

        public static bool GetTextCh(DependencyObject obj)
        {
            return (bool)obj.GetValue(TextChProperty);
        }

        public static void SetTextCh(DependencyObject obj, bool value)
        {
            obj.SetValue(TextChProperty, value);
        }


        public static readonly DependencyProperty TextProperty =
        DependencyProperty.RegisterAttached(
            "Text",
            typeof(string),
            typeof(RichTextBoxHelper),
            new PropertyMetadata(string.Empty, OnTextChanged));

        public static string GetText(DependencyObject obj)
        {
            return (string)obj.GetValue(TextProperty);
        }

        public static void SetText(DependencyObject obj, string value)
        {
            obj.SetValue(TextProperty, value);
        }

        private static void OnTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is RichTextBox richTextBox)
            {
                if (GetTextCh(richTextBox))
                {
                    SetTextCh(richTextBox, false);
                    return;
                }
                richTextBox.TextChanged -= RichTextBox_TextChanged;
                SetDocumentText(richTextBox, (string)e.NewValue ?? string.Empty);
                richTextBox.TextChanged += RichTextBox_TextChanged;
            }
        }

        private static void RichTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (sender is RichTextBox richTextBox)
            {
                SetTextCh(richTextBox, true);
                SetText(richTextBox, GetDocumentText(richTextBox));
            }
        }

        private static string GetDocumentText(RichTextBox richTextBox)
        {
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            return textRange.Text;
        }

        private static void SetDocumentText(RichTextBox richTextBox, string text)
        {
            var textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            textRange.Text = text;
        }
    }
}
