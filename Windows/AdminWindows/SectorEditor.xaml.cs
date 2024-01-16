using Incubator_2.Models;
using Incubator_2.ViewModels.VMAdmin;
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

namespace Incubator_2.Windows.AdminWindows
{
    /// <summary>
    /// Логика взаимодействия для SectorEditor.xaml
    /// </summary>
    public partial class SectorEditor : Window
    {
        VM_SectorElement vm;
        public SectorEditor(Sector s)
        {
            InitializeComponent();
            vm = new(s);
            this.DataContext = vm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            vm.Save();
        }
    }
}
