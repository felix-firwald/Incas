using Incubator_2.Models;
using Incubator_2.ViewModels.VMAdmin;
using System.Windows;

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
