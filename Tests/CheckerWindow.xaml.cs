using System.Windows;
using System.Windows.Controls;

namespace Incas.Tests
{
    /// <summary>
    /// Логика взаимодействия для CheckerWindow.xaml
    /// </summary>
    public partial class CheckerWindow : Window
    {
        public CheckerWindow()
        {
            this.InitializeComponent();
        }
        public CheckerWindow(Control c)
        {
            this.InitializeComponent();
            this.MainGrid.Children.Clear();
            this.MainGrid.Children.Add(c);
        }
    }
}
