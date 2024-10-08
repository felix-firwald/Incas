using ClosedXML.Excel;
using Incas.Core.Classes;
using Incas.Objects.Components;
using Incas.Templates.Components;
using Incas.Templates.ViewModels;
using Microsoft.Scripting.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Incas.Objects.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для FieldTableFiller.xaml
    /// </summary>
    public partial class FieldTableFiller : UserControl
    {
        private TableFillerViewModel vm;
        public Objects.Models.Field Field;

        public FieldTableFiller(Objects.Models.Field f)
        {
            this.InitializeComponent();
            this.Field = f;
            this.vm = new TableFillerViewModel(f);
            this.vm.AddRow();
            this.TableName.Content = f.VisibleName;
            this.DataContext = this.vm;
        }
        public void SetData(DataTable dt)
        {
            if (dt == null)
            {
                return;
            }
            this.vm.ApplyData(dt);
        }
        public void SetData(string data)
        {
            this.SetData(JsonConvert.DeserializeObject<DataTable>(data));
        }
        public string GetData()
        {
            return JsonConvert.SerializeObject(this.vm.Grid);
        }
        public DataTable GetValue()
        {
            return this.vm.Grid;
        }
        public SGeneratedTag GetAsGeneratedTag()
        {
            SGeneratedTag result = new()
            {
                tag = this.Field.Id,
                value = this.GetData()
            };
            return result;
        }
        private void RunScript()
        {
            try
            {
                string json = JsonConvert.SerializeObject(this.vm.Grid);
                List<Dictionary<string, string>> data = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);
                ScriptScope scope = ScriptManager.GetEngine().CreateScope();
                scope.SetVariable("input_data", data);
                //ScriptManager.Execute(this.command.Script, scope);
                List<Dictionary<string, string>> result = scope.GetVariable("output");
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(result));
                this.SetData(dt);
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog("При обработке скрипта произошла ошибка:\n" + ex.Message);
            }
        }

        private void CommandClick(object sender, RoutedEventArgs e)
        {
            this.RunScript();
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            this.vm.AddRow();
        }

        private void RemoveClick(object sender, RoutedEventArgs e)
        {
            this.vm.RemoveSelectedRow();
            //try
            //{
            //    this.Data.Rows.RemoveAt(this.Table.SelectedIndex);
            //}
            //catch { }
        }

        private void ColumnGenerating(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            foreach (TableFieldColumnData col in this.vm.TableDefinition.Columns)
            {
                if (col.Name == e.Column.Header.ToString())
                {
                    switch (col.FieldType)
                    {
                        case FieldType.Variable:
                            DataGridTextColumn dgt1 = new();
                            dgt1.Header = col.VisibleName;
                            dgt1.Binding = new Binding(e.Column.Header.ToString());
                            dgt1.EditingElementStyle = this.FindResource("TextBoxMain") as Style;
                            e.Column = dgt1;
                            break;
                        case FieldType.Text:
                            DataGridTextColumn dgt2 = new();
                            dgt2.Header = col.VisibleName;
                            dgt2.Binding = new Binding(e.Column.Header.ToString());
                            dgt2.EditingElementStyle = this.FindResource("TextBoxBig") as Style;
                            e.Column = dgt2;
                            break;
                        case FieldType.LocalEnumeration:
                        case FieldType.GlobalEnumeration:
                            DataGridComboBoxColumn dgc = new();
                            dgc.Header = col.VisibleName;
                            dgc.TextBinding = new Binding(e.Column.Header.ToString());
                            
                            dgc.EditingElementStyle = this.FindResource("ComboBoxMain") as Style;
                            if (col.FieldType == FieldType.LocalEnumeration)
                            {
                                dgc.ItemsSource = JsonConvert.DeserializeObject<List<string>>(col.Value);
                            }
                            else
                            {
                                dgc.ItemsSource = ProgramState.GetEnumeration(Guid.Parse(col.Value));
                            }
                            e.Column = dgc;
                            break;
                    }
                    break;
                }
            }
        }

        private void ExcelClick(object sender, RoutedEventArgs e)
        {
            string file = "";
            if (DialogsManager.ShowOpenFileDialog(ref file, "MS Excel|*.xlsx"))
            {
                IXLWorksheet ws;
                try
                {
                    XLWorkbook wb = new(file);
                    ws = wb.Worksheet(1);
                }
                catch (IOException)
                {
                    DialogsManager.ShowErrorDialog("Файл занят другим процессом. Его использование невозможно.", "Ошибка доступа");
                    return;
                }
                catch (Exception ex)
                {
                    DialogsManager.ShowErrorDialog(ex.Message);
                    return;
                }
                DataTable output = new();

                foreach (DataColumn dc in this.vm.Grid.Columns)
                {
                    IXLCell colCell;
                    string col = dc.ColumnName;
                    output.Columns.Add(col);
                    try
                    {
                        colCell = ws.Search(dc.ColumnName, CompareOptions.IgnoreCase).First();   // ищем заголовок столбца с именем аналогичным тегу
                        int columnNumber = colCell.WorksheetColumn().ColumnNumber();    // номер столбца в листе Excel
                        int rowNumber = colCell.WorksheetRow().RowNumber() + 1; // номер строки в листе Excel          
                        for (int i = rowNumber; i <= ws.LastRowUsed().RowNumber(); i++)
                        {
                            if (i > output.Rows.Count)
                            {
                                output.Rows.Add();
                            }

                            output.Rows[i - 2][col] = ws.Cell(i, columnNumber).Value.ToString();
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                this.vm.ApplyData(output);
            }
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            string file = "";
            if (DialogsManager.ShowSaveFileDialog(ref file, "MS Excel|*.xlsx"))
            {
                XLWorkbook wb = new();
                IXLWorksheet ws = wb.AddWorksheet("Из INCAS");
                IXLTable table = ws.FirstCell().InsertTable(this.vm.Grid);
                table.Theme = new XLTableTheme("None");
                try
                {
                    wb.SaveAs(file);
                    ProgramState.OpenFolder(Path.GetDirectoryName(file));
                }
                catch (IOException)
                {
                    DialogsManager.ShowErrorDialog("При попытке записать файл возникла ошибка,\nвозможно файл уже открыт." +
                        "\nЗакройте его и попробуйте снова", "Сохранение прервано");
                }
            }
        }
    }
}
