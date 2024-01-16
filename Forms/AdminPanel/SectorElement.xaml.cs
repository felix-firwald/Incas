using Incubator_2.Models;
using Incubator_2.ViewModels.VMAdmin;
using Incubator_2.Windows.AdminWindows;
using System.Windows.Controls;


namespace Incubator_2.Forms.AdminPanel
{
    /// <summary>
    /// Логика взаимодействия для SectorElement.xaml
    /// </summary>
    public partial class SectorElement : UserControl
    {
        VM_SectorElement vm;
        public SectorElement(Sector s)
        {
            InitializeComponent();
            vm = new(s);
            this.DataContext = vm;
        }

        private void EditClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SectorEditor se = new(vm.sector);
            se.ShowDialog();
        }
        private void DeleteClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
