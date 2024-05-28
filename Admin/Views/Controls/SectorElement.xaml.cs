using Incas.Admin.ViewModels;
using Incas.Core.Models;
using Incubator_2.Windows.AdminWindows;
using System.Windows.Controls;

namespace Incas.Admin.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для SectorElement.xaml
    /// </summary>
    public partial class SectorElement : UserControl
    {
        private SectorElementViewModel vm;
        public SectorElement(Sector s)
        {
            this.InitializeComponent();
            this.vm = new(s);
            this.DataContext = this.vm;
        }

        private void EditClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SectorEditor se = new(this.vm.sector);
            se.ShowDialog();
        }
        private void DeleteClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
