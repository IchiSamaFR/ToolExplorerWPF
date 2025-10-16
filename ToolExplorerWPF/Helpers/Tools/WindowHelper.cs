using System.Windows;
using System.Windows.Controls;

namespace ToolExplorerWPF.Helpers.Tools
{
    public static class WindowHelper
    {
        public static Window CreateDetachedWindow(
            UserControl control,
            object dataContext,
            string title = "Fenêtre détachée",
            double width = 800,
            double height = 450,
            Action<Window>? onClosed = null)
        {
            var window = new Window
            {
                Title = title,
                Width = width,
                Height = height,
                Content = control,
                DataContext = dataContext,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            if (onClosed != null)
            {
                window.Closed += (sender, args) => onClosed(window);
            }

            return window;
        }

        public static Window CreateAndShowDetachedWindow(
            UserControl control,
            object dataContext,
            string title = "Fenêtre détachée",
            double width = 800,
            double height = 450,
            Action<Window>? onClosed = null)
        {
            var window = CreateDetachedWindow(control, dataContext, title, width, height, onClosed);
            window.Show();
            return window;
        }

        public static bool? CreateAndShowDialogWindow(
            UserControl control,
            object dataContext,
            string title = "Fenêtre détachée",
            double width = 800,
            double height = 450,
            Action<Window>? onClosed = null)
        {
            var window = CreateDetachedWindow(control, dataContext, title, width, height, onClosed);
            return window.ShowDialog();
        }
    }
}