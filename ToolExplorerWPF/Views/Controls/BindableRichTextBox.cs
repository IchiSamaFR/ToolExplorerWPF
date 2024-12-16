using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using Wpf.Ui.Controls;

namespace ToolExplorerWPF.Views.Controls
{
    public class BindableRichTextBox : RichTextBox
    {
        public static readonly DependencyProperty BoundTextProperty =
            DependencyProperty.Register("BoundText", typeof(string), typeof(BindableRichTextBox), new PropertyMetadata(string.Empty, OnBoundTextChanged));

        public string BoundText
        {
            get { return (string)GetValue(BoundTextProperty); }
            set { SetValue(BoundTextProperty, value); }
        }

        public BindableRichTextBox()
        {
            var binding = new Binding("BoundText")
            {
                Source = this,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            SetBinding(BoundTextProperty, binding);
        }

        private static void OnBoundTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var richTextBox = d as BindableRichTextBox;
            if (richTextBox != null)
            {
                richTextBox.Document.Blocks.Clear();
                if (e.NewValue != null)
                {
                    richTextBox.Document.Blocks.Add(new Paragraph(new Run(e.NewValue.ToString())));
                }
            }
        }
    }
}
