using Incas.Core.Models;
using Incas.ViewModels.VMAdmin;
using System.Windows;

namespace Incubator_2.Windows.AdminWindows
{
    /// <summary>
    /// Логика взаимодействия для SectorEditor.xaml
    /// </summary>
    public partial class SectorEditor : Window
    {
        private VM_SectorElement vm;
        public SectorEditor(Sector s)
        {
            this.InitializeComponent();
            this.vm = new(s);
            this.DataContext = this.vm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            this.vm.Save();
        }
    }
}
