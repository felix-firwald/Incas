using ClosedXML.Excel;
using Incas.Core.Classes;
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

namespace Incas.Objects.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для FieldTableFiller.xaml
    /// </summary>
    public partial class FieldTableFiller : UserControl
    {
        private TableFillerViewModel vm;
        public Objects.Models.Field field;
        public DataTable DataTable => this.vm.Grid;

        public FieldTableFiller(Objects.Models.Field f)
        {
            this.InitializeComponent();
            this.field = f;
            this.vm = new TableFillerViewModel(f);
            this.DataContext = this.vm;
            this.MakeFields();
        }
        private void MakeFields()
        {
            void MakeField(string name)
            {
                TextBox tb = new()
                {
                    Tag = name,
                    Style = this.FindResource("TextBoxMain") as Style
                };
                this.Fields.Children.Add(tb);
            }
            foreach (DataColumn dc in this.vm.Grid.Columns)
            {
                MakeField(dc.ColumnName);
            }
        }
        public void SetData(DataTable dt)
        {
            this.vm.Grid = dt;
        }
        public void SetData(string data)
        {
            this.vm.Grid = JsonConvert.DeserializeObject<DataTable>(data);
        }
        public string GetData()
        {
            return JsonConvert.SerializeObject(this.vm.Grid);
        }
        public SGeneratedTag GetAsGeneratedTag()
        {
            SGeneratedTag result = new()
            {
                tag = this.field.Id,
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
            this.vm.Grid.Rows.Add();
        }

        private void RemoveClick(object sender, RoutedEventArgs e)
        {
            try
            {
                this.vm.Grid.Rows.RemoveAt(this.Table.SelectedIndex);
            }
            catch { }
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
                this.vm.Grid = output;
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
