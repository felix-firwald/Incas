using Incas.Core.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Incas.Core.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для ContainerWindow.xaml
    /// </summary>
    public partial class ContainerWindow : Window
    {
        public ContainerWindow(UserControl control, string title)
        {
            this.InitializeComponent();
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
    }
}
