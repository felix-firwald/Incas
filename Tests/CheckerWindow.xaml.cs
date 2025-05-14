using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
