using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Windows.Graphics.Display;

namespace Incas.Core.Styles
{
    internal static class LocalExtensions
    {
        public static void ForWindowFromTemplate(this object templateFrameworkElement, Action<Window> action)
        {
            if (((FrameworkElement)templateFrameworkElement).TemplatedParent is Window window)
            {
                action(window);
            }
        }

        public static IntPtr GetWindowHandle(this Window window)
        {
            WindowInteropHelper helper = new(window);
            return helper.Handle;
        }
    }

    public partial class IncasWindowStyle
    {
        private void IconMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                sender.ForWindowFromTemplate(w => SystemCommands.CloseWindow(w));
            }
        }

        private void IconMouseUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            Point point = element.PointToScreen(new Point(element.ActualWidth / 2, element.ActualHeight));
            sender.ForWindowFromTemplate(w => SystemCommands.ShowSystemMenu(w, point));
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            ((Window)sender).StateChanged += this.WindowStateChanged;
        }

        private void WindowStateChanged(object sender, EventArgs e)
        {
            Window w = (Window)sender;
            nint handle = w.GetWindowHandle();
            Border containerBorder = (Border)w.Template.FindName("PART_Container", w);
            //DisplayInformation.GetForCurrentView();
            if (w.WindowState == WindowState.Maximized)
            {
                // Make sure window doesn't overlap with the taskbar.
                System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromHandle(handle);
                if (screen.Primary)
                {
                    containerBorder.Padding = new Thickness(
                        SystemParameters.WorkArea.Left + 6,
                        SystemParameters.WorkArea.Top + 6,
                        SystemParameters.PrimaryScreenWidth - SystemParameters.WorkArea.Right + 6,
                        SystemParameters.PrimaryScreenHeight - SystemParameters.WorkArea.Bottom + 6);
                }
                else
                {
                    containerBorder.Padding = new Thickness(
                        SystemParameters.WorkArea.Left + 6,
                        SystemParameters.WorkArea.Top + 6,
                        SystemParameters.PrimaryScreenWidth - SystemParameters.WorkArea.Right + 6,
                        SystemParameters.PrimaryScreenHeight - SystemParameters.WorkArea.Bottom + 6);
                }
            }
            else
            {
                containerBorder.Padding = new Thickness(7, 7, 7, 5);
            }
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            sender.ForWindowFromTemplate(w => SystemCommands.CloseWindow(w));
        }

        private void MinButtonClick(object sender, RoutedEventArgs e)
        {
            sender.ForWindowFromTemplate(w => SystemCommands.MinimizeWindow(w));
        }

        private void MaxButtonClick(object sender, RoutedEventArgs e)
        {
            sender.ForWindowFromTemplate(w =>
            {
                if (w.WindowState == WindowState.Maximized)
                {
                    SystemCommands.RestoreWindow(w);
                }
                else
                {
                    SystemCommands.MaximizeWindow(w);
                }
            });
        }
    }
}
