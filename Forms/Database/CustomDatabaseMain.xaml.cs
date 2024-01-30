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
                if (string.IsNullOrEmpty(vm.SelectedTable))
                {
                    ProgramState.ShowExclamationDialog("Таблица для записи не выбрана!", "Действие невозможно");
                    return;
                }
                CreateRecord cr = new(vm.SelectedTable, vm.GetTableDefinition(), vm.SelectedDatabase.path);
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
            try
            {
                if (this.TableGrid.SelectedItems.Count == 0)
                {
                    ProgramState.ShowExclamationDialog("Не выбрано ни одной записи для удаления!", "Действие невозможно");
                    return;
                }
                CustomTable ct = new();
                List<string> selection = new();
                string pk = vm.GetPK();
                for (int i = 0; i < this.TableGrid.SelectedItems.Count; i++)
                {
                    selection.Add(((DataRowView)this.TableGrid.SelectedItems[i]).Row[pk].ToString());
                }
                ct.DeleteInTable(vm.SelectedTable, pk, vm.SelectedDatabase.path, selection);
            }
            catch (ArgumentException)
            {
                ProgramState.ShowErrorDialog("Неправильная конфигурация таблицы (вероятно у таблицы отсутствует первичный ключ).", "Ошибка идентификации записи");
            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog("При попытке удаления записей возникла ошибка неизвестного характера:\n" + ex.Message);
            }
            vm.RefreshTable();
        }

        private void EditRecordClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.TableGrid.SelectedItems.Count == 0)
                {
                    ProgramState.ShowExclamationDialog("Не выбрано ни одной записи для редактирования!", "Действие невозможно");
                    return;
                }
                string pk = vm.GetPK();
                string record = ((DataRowView)this.TableGrid.SelectedItems[0]).Row[pk].ToString();
                CreateRecord cr = new CreateRecord(vm.SelectedTable, pk, record, vm.GetTableDefinition(), vm.SelectedDatabase.path);
                cr.ShowDialog();
                vm.UpdateTable();
            }
            catch (ArgumentException)
            {
                ProgramState.ShowErrorDialog("Неправильная конфигурация таблицы (вероятно у таблицы отсутствует первичный ключ).", "Ошибка идентификации записи");
            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog("При попытке открыть окно редактирования записи возникла ошибка неизвестного характера:\n" + ex.Message);
            }
        }

        private void SwitchSelectionUnitClick(object sender, MouseButtonEventArgs e)
        {
            vm.SwitchSelectionUnit();
        }

        private void ReadCommandClick(object sender, RoutedEventArgs e)
        {
            vm.CustomViewRequest = ReplaceParametersInQuery(((MenuItem)sender).Tag.ToString());
            vm.UpdateTable();
            
        }
        private string GetParameterFormat(string parameter)
        {
            return "{%" + parameter + "%}";
        }

        private List<string> GetPKSelection()
        {
            List<string> selection = new();
            string pk = vm.GetPK();
            for (int i = 0; i < this.TableGrid.SelectedItems.Count; i++)
            {
                selection.Add(((DataRowView)this.TableGrid.SelectedItems[i]).Row[pk].ToString());
            }
            return selection;
        }

        private string ReplaceParametersInQuery(string request)
        {
            if (request.Contains("{%"))
            {
                request = request.Replace(GetParameterFormat("TIME"), DateTime.Now.ToString("G"));
                if (request.Contains(GetParameterFormat("INPUT")))
                {
                    request = request.Replace(GetParameterFormat("INPUT"), ProgramState.ShowInputBox("Введите значение", "Для выполнения функции ожидается ввод"));
                }
                if (this.TableGrid.SelectedItems.Count > 0 && request.Contains("SELECTED"))
                {
                    request = request.Replace(GetParameterFormat("SELECTED"), string.Join(", ", GetPKSelection()));
                    foreach (string col in vm.Columns)
                    {
                        request = request.Replace(GetParameterFormat("SELECTED#" + col), ((DataRowView)this.TableGrid.SelectedItems[0]).Row[col].ToString());
                    }
                }
            }
            return request;
        }

        private void UpdateCommandClick(object sender, RoutedEventArgs e)
        {
            string request = ((MenuItem)sender).Tag.ToString();
            
            vm.CustomUpdateRequest(ReplaceParametersInQuery(request));
            
        }
        

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NewCommandClick(object sender, RoutedEventArgs e)
        {
            if (vm.SelectedDatabase.path != null && vm.SelectedTable != null)
            {
                CreateCommand cc = new(vm.SelectedDatabase.path, vm.SelectedTable);
                cc.ShowDialog();
                vm.RefreshCommands();
            }
            else
            {
                ProgramState.ShowExclamationDialog("База данных или таблица не выбрана!", "Действие невозможно");
            }
        }

        private void EditCommand(object sender, RoutedEventArgs e)
        {
            SCommand com = vm.GetCommand(int.Parse(((MenuItem)sender).Tag.ToString()));
            CreateCommand cc = new(com);
            cc.ShowDialog();
            vm.RefreshCommands();
        }

        private void RemoveCommand(object sender, RoutedEventArgs e)
        {
            using (Command c = new())
            {
                c.id = int.Parse(((MenuItem)sender).Tag.ToString());
                c.DeleteCommand();
            }
            vm.RefreshCommands();
        }

        private void SearchTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SearchClick(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(vm.ColumnFilter) && !string.IsNullOrEmpty(vm.SearchText))
            {
                vm.CustomViewRequest = $"SELECT * FROM [{vm.SelectedTable}] WHERE [{vm.ColumnFilter}] LIKE '%{vm.SearchText}%'";
                vm.UpdateTable();
            }
        }

        private void ClearCustomClick(object sender, MouseButtonEventArgs e)
        {
            vm.ClearTableFromCustomView();
        }
    }
}
