using Common;
using Incubator_2.Models;
using Incubator_2.ViewModels.VM_CustomDB;
using Incubator_2.Windows.CustomDatabase;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Incubator_2.Forms.Database
{
    /// <summary>
    /// Логика взаимодействия для CustomDatabaseMain.xaml
    /// </summary>
    public partial class CustomDatabaseMain : UserControl
    {
        private VM_CustomDatabase vm = new();
        public CustomDatabaseMain()
        {
            InitializeComponent();
            this.DataContext = vm;
        }

        private void AddNewRecordClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                CreateRecord cr = new(vm.SelectedTable, vm.GetTableDefinition());
                cr.ShowDialog();
                vm.RefreshTable();
            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog(ex.Message);
            }
        }

        private void RefreshClick(object sender, MouseButtonEventArgs e)
        {
            vm.RefreshTable();
        }

        private void DeleteRecordsClick(object sender, MouseButtonEventArgs e)
        {
            if (this.TableGrid.SelectedItems.Count == 0)
            {
                ProgramState.ShowExlamationDialog("Не выбрано ни одной записи для удаления!", "Действие невозможно");
                return;
            }
            CustomTable ct = new();
            List<string> selection = new();
            string pk = vm.GetPK();
            for (int i = 0; i < this.TableGrid.SelectedItems.Count; i++)
            {
                selection.Add(((DataRowView)this.TableGrid.SelectedItems[i]).Row[pk].ToString());
            }
            ct.DeleteInTable(vm.SelectedTable, pk, selection);
            vm.RefreshTable();
        }
    }
}
