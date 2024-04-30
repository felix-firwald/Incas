using Incubator_2.Models;
using Incubator_2.ViewModels.VMAdmin;
using Incubator_2.Windows.AdminWindows;
using System.Windows;
using System.Windows.Controls;

namespace Incubator_2.Forms.AdminPanel
{
    /// <summary>
    /// Логика взаимодействия для WorkspaceManager.xaml
    /// </summary>
    public partial class WorkspaceManager : UserControl
    {
        private VM_WorkspaceParameters vm;
        public WorkspaceManager()
        {
            InitializeComponent();
            this.vm = new VM_WorkspaceParameters();
            this.DataContext = this.vm;
            this.LoadSectors();
        }

        private void LoadSectors()
        {
            this.Sectors.Children.Clear();
            using Sector s = new();
            s.GetSectors().ForEach(sector =>
            {
                SectorElement se = new(sector);
                this.Sectors.Children.Add(se);
            });
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            this.vm.SaveParameters();
        }

        private void AddSectorClick(object sender, RoutedEventArgs e)
        {
            SectorEditor se = new(new Sector());
            se.ShowDialog();
            this.LoadSectors();
        }
    }
}
