using Incubator_2.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для ContainerWindow.xaml
    /// </summary>
    public partial class ContainerWindow : Window
    {
        public ContainerWindow(UserControl control, string title)
        {
            InitializeComponent();
            this.DataContext = new ContainerWindowViewModel(this);
            this.MinHeight = control.MinHeight + 40;
            this.MinWidth = control.MinWidth;
            this.Title = title;
            this.ContentPanel.Child = control;
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        //private void MinimizeClick(object sender, RoutedEventArgs e)
        //{
        //    WindowState = WindowState.Minimized;
        //}
        //private void MaximizeClick(object sender, RoutedEventArgs e)
        //{
        //    WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        //}
        //private void CloseClick(object sender, RoutedEventArgs e)
        //{
        //    this.Close();
        //}

        //private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    this.DragMove();
        //}
    }
}
