using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ToolExplorerWPF.Views.Controls
{
    public class BindableRichTextBox : RichTextBox
    {
        public static readonly DependencyProperty BoundTextProperty =
            DependencyProperty.Register(
                "BoundText",
                typeof(string),
                typeof(BindableRichTextBox),
                new PropertyMetadata(string.Empty, OnBoundTextChanged));

        public string BoundText
        {
            get { return (string)GetValue(BoundTextProperty); }
            set { SetValue(BoundTextProperty, value); }
        }

        public BindableRichTextBox()
        {
            TextChanged += BindableRichTextBox_TextChanged;
        }

        private void BindableRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string newText = new TextRange(Document.ContentStart, Document.ContentEnd).Text;
            newText = newText.TrimEnd('\r', '\n');
            if (BoundText != newText)
            {
                BoundText = newText;
            }
        }

        private static void OnBoundTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var richTextBox = d as BindableRichTextBox;
            if (richTextBox != null)
            {
                string newText = e.NewValue as string ?? string.Empty;
                string currentText = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text.TrimEnd('\r', '\n');
                if (currentText != newText)
                {
                    richTextBox.Document.Blocks.Clear();
                    if (!string.IsNullOrEmpty(newText))
                    {
                        richTextBox.Document.Blocks.Add(new Paragraph(new Run(newText)));
                    }
                }
            }
        }
    }
}