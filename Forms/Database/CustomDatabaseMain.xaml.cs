using ClosedXML.Excel;
using Common;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Incubator_2.Models;
using Incubator_2.ViewModels.VM_CustomDB;
using Incubator_2.Windows.CustomDatabase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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
    public partial class CustomDatabaseMain : System.Windows.Controls.UserControl
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
            vm.UpdateTable();
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
            vm.UpdateTable();
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
            if (!string.IsNullOrEmpty(vm.SearchText))
            {
                char[] letters = vm.SearchText.ToCharArray();
                letters[0] = char.ToUpper(letters[0]);
                vm.CustomViewRequest = $"SELECT * FROM [{vm.SelectedTable}] " +
                    $"WHERE [{vm.ColumnFilter}] = '{vm.SearchText}' OR " +
                    $"[{vm.ColumnFilter}] LIKE '%{vm.SearchText}%' OR " +
                    $"[{vm.ColumnFilter}] LIKE '%{vm.SearchText.ToUpper()}%' OR " +
                    $"[{vm.ColumnFilter}] LIKE '%{new string(letters)}%'";
                vm.UpdateTable();
            }
        }

        private void ClearCustomClick(object sender, MouseButtonEventArgs e)
        {
            vm.ClearTableFromCustomView();
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы", Justification = "<Ожидание>")]
        public void ExportToExcel()
        {
            FolderBrowserDialog fb = new();
            fb.ShowDialog();
            if (!string.IsNullOrEmpty(fb.SelectedPath))
            {
                string fileName = ProgramState.ShowInputBox("Имя файла", "Введите имя файла для вывода");
                ProgramState.ShowWaitCursor();
                XLWorkbook wb = new XLWorkbook();
                IXLWorksheet ws = wb.AddWorksheet(vm.SelectedTable);
                for (int c = 0; c < vm.Table.Columns.Count; c++) // columns
                {
                    IXLCell cell = ws.Cell(1, c + 1).SetValue(vm.Columns[c]);
                    cell.Style.Font.Bold = true;
                    for (int r = 0; r < vm.Table.Rows.Count; r++) // rows
                    {
                        ws.Cell(r+2, c+1).SetValue(vm.Table.Rows[r][c].ToString());
                    }
                }
                ws.SetAutoFilter();
                try
                {
                    wb.SaveAs(fb.SelectedPath + $"\\{fileName}.xlsx");
                }    
                catch (IOException)
                {
                    ProgramState.ShowWaitCursor(false);
                    ProgramState.ShowErrorDialog("При попытке записать файл возникла ошибка,\nвозможно файл уже открыт." +
                        "\nЗакройте его и попробуйте снова", "Сохранение прервано");
                }
                ProgramState.ShowWaitCursor(false);
                ProgramState.OpenFolder(fb.SelectedPath);
            }
            
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы", Justification = "<Ожидание>")]
        public void ImportFromExcel(bool deleteOld = false)
        {
            OpenFileDialog of = new();
            of.Filter = "Файлы Excel (*.xlsx)|*.xlsx";
            if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ProgramState.ShowWaitCursor();
                IXLWorksheet ws;
                try
                {
                    XLWorkbook wb = new XLWorkbook(of.FileName);
                    ws = wb.Worksheet(1);
                }
                catch (IOException)
                {
                    ProgramState.ShowErrorDialog("Файл занят другим процессом. Его использование невозможно.");
                    return;
                }
                string pkfield = vm.GetPK();
                DataTable output = new();
                foreach (string col in vm.GetFieldsSimple())
                {
                    if (col == pkfield)
                    {
                        continue;
                    }
                    IXLCell colCell;
                    try
                    {
                        if (ws.Search(col, CompareOptions.IgnoreCase).FirstOrDefault() == null)
                        {
                            ProgramState.ShowWaitCursor(false);
                            ProgramState.ShowExclamationDialog($"Лист Excel не содержит столбца \"{col}\".\n" +
                                $"Импорт из листа, не содержащего хотя бы одно из описанных в таблице полей, запрещен в целях обеспечения полноты данных.", "Действие прервано");
                            continue;
                        }
                        
                        colCell = ws.Search(col, CompareOptions.IgnoreCase).FirstOrDefault();   // ищем заголовок столбца с именем аналогичным тегу
                        int columnNumber = colCell.WorksheetColumn().ColumnNumber();    // номер столбца в листе Excel
                        int rowNumber = colCell.WorksheetRow().RowNumber() + 1; // номер строки в листе Excel
                        output.Columns.Add(col);
                        for (int i = rowNumber; i <= ws.LastRowUsed().RowNumber(); i++)
                        {
                            if (i > output.Rows.Count)
                            {
                                output.Rows.Add();
                            }
                            
                            output.Rows[i-2][col] = ws.Cell(i, columnNumber).Value.ToString();
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                ProgramState.ShowWaitCursor(false);
                DataImporter di = new(output);
                di.ShowDialog();
                if (di.Result == Windows.DialogStatus.Yes)
                {
                    ProgramState.ShowWaitCursor();
                    string result = "BEGIN TRANSACTION;\n";
                    if (deleteOld)
                    {
                        result += $"DELETE FROM [{vm.Table}];\n";
                    }
                    List<string> columns = new();
                    foreach (DataColumn col in di.ResultTable.Columns)
                    {
                        columns.Add(col.ColumnName);
                    }
                    foreach (DataRow dr in di.ResultTable.Rows)
                    {
                        List<string> cells = new();
                        foreach (string cell in columns)
                        {
                            cells.Add(dr[cell].ToString());
                        }
                        result += $"REPLACE INTO [{vm.Table}] ([{string.Join("], [", columns)}]) " +
                            $"VALUES ('{string.Join("', '", cells)}');\n";
                    }
                    result += "\nEND TRANSACTION;";
                    vm.CustomUpdateRequest(result);
                    ProgramState.ShowWaitCursor(false);
                }
            }
        }

        private void ExportClick(object sender, RoutedEventArgs e)
        {
            switch (((MenuItem)sender).Tag.ToString())
            {
                case "Excel":
                    ExportToExcel();
                    break;
                case "Word":
                    break;
            }
        }

        private void ImportClick(object sender, RoutedEventArgs e)
        {
            switch (((MenuItem)sender).Tag.ToString())
            {
                case "Import":
                    ImportFromExcel();
                    break;
                case "FullImport":
                    if (Permission.CurrentUserPermission == PermissionGroup.Admin)
                    {
                        if (ProgramState.ShowQuestionDialog("Старые данные будут стерты без возможности восстановления.", "Вы уверены?") == Windows.DialogStatus.Yes)
                        {
                            ImportFromExcel(true);
                        }    
                    }
                    else
                    {
                        ProgramState.ShowExclamationDialog("Данное действие может совершать только администратор рабочего пространства", "Нет доступа");
                    }
                    break;
            }
        }
    }
}
