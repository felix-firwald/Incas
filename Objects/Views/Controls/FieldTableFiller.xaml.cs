using ClosedXML.Excel;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using Incas.Objects.AutoUI;
using Incas.Objects.Components;
using Incas.Objects.Interfaces;
using Incas.Rendering.Components;
using Incas.Rendering.ViewModels;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Exceptions;
using IncasEngine.ObjectiveEngine.Models;
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
using System.Windows.Media;
using Xceed.Wpf.Toolkit;
using static Incas.Objects.Interfaces.IFillerBase;
using static IncasEngine.ObjectiveEngine.FieldComponents.TableFieldData;

namespace Incas.Objects.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для FieldTableFiller.xaml
    /// </summary>
    public partial class FieldTableFiller : System.Windows.Controls.UserControl, ITableFiller
    {
        private TableFillerViewModel vm;
        public Field Field { get; set; }
        public event FillerUpdate OnFillerUpdate;
        public event StringAction OnInsert;
        public event FillerUpdate OnDatabaseObjectCopyRequested;
        public FieldTableFiller(Field f)
        {
            this.InitializeComponent();
            this.Field = f;
            this.vm = new TableFillerViewModel(f);

            this.vm.AddRow();
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
        public void SetValue(string data)
        {
            try
            {
                this.SetData(JsonConvert.DeserializeObject<DataTable>(data));
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// Internal using
        /// </summary>
        /// <returns></returns>
        public string GetData()
        {
            this.CheckData();
            return JsonConvert.SerializeObject(this.vm.Grid);
        }
        public object GetDataForScript()
        {
            return this.vm.Grid;
        }
        public void MarkAsNotValidated()
        {
            this.Main.Background = new SolidColorBrush(Color.FromRgb(68, 40, 45));
        }
        public void MarkAsValidated()
        {
            this.Main.Background = null;
        }
        public void CheckData()
        {
            foreach (TableFieldColumnData tf in this.vm.TableDefinition.Columns)
            {
                if (tf.NotNull == true)
                {
                    int row = 1;
                    foreach (DataRow dr in this.vm.Grid.Rows)
                    {
                        if (string.IsNullOrWhiteSpace(dr[tf.Name].ToString()))
                        {
                            this.MarkAsNotValidated();
                            throw new NotNullFailed($"Колонка \"{tf.VisibleName}\" у таблицы \"{this.vm.TableName}\" является обязательной, однако в ряду под номером {row} значение отсутствует.");
                        }
                        row++;
                    }
                }
            }
        }
        /// <summary>
        /// Template & forms using
        /// </summary>
        /// <returns></returns>
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
        }

        private void ColumnGenerating(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            foreach (TableFieldColumnData col in this.vm.TableDefinition.Columns)
            {
                if (col.Name == e.Column.Header.ToString() || col.Id.ToString() == e.Column.Header.ToString())
                {
                    switch (col.FieldType)
                    {
                        case FieldType.String:
                        case FieldType.Text:
                            DataGridTextColumn dgt1 = new()
                            {
                                Header = col.VisibleName,
                                Binding = new System.Windows.Data.Binding(e.Column.Header.ToString()),
                                EditingElementStyle = this.FindResource("TextBoxGrid") as Style
                            };
                            e.Column = dgt1;
                            break;
                        case FieldType.LocalEnumeration:
                        case FieldType.GlobalEnumeration:
                            DataGridComboBoxColumn dgc = new()
                            {
                                Header = col.VisibleName,
                                TextBinding = new System.Windows.Data.Binding(e.Column.Header.ToString()),

                                EditingElementStyle = ResourceStyleManager.FindStyle(ResourceStyleManager.ComboboxGridStyle)
                            };
                            dgc.ItemsSource = col.FieldType == FieldType.LocalEnumeration
                                ? JsonConvert.DeserializeObject<List<string>>(col.Value)
                                : (System.Collections.IEnumerable)ProgramState.GetEnumeration(Guid.Parse(col.Value));
                            e.Column = dgc;
                            break;
                        case FieldType.Integer:
                            this.GenerateTemplateColumnInteger(col, e);
                            break;
                        case FieldType.Date:
                            this.GenerateTemplateColumnDateTime(col, e);
                            break;
                        case FieldType.Boolean:
                            DataGridCheckBoxColumn cbc = new()
                            {
                                Header = col.VisibleName,
                                Binding = new System.Windows.Data.Binding(e.Column.Header.ToString()),
                                EditingElementStyle = ResourceStyleManager.FindStyle(ResourceStyleManager.CheckboxEditingGridStyle),               
                                ElementStyle = ResourceStyleManager.FindStyle(ResourceStyleManager.CheckboxNotEditableGridStyle)
                            };
                            e.Column = cbc;
                            break;
                        default:
                            DialogsManager.ShowErrorDialog($"Не удалось распознать тип данных столбца \"{col.VisibleName}\"");
                            e.Column.IsReadOnly = true;
                            break;
                    }
                    break;
                }
            }
        }
        private void GenerateTemplateColumnInteger(TableFieldColumnData col, DataGridAutoGeneratingColumnEventArgs e)
        {
            DataGridTemplateColumn templateInteger = new();
            templateInteger.Header = col.VisibleName;
            
            DataTemplate cellEdit = new();
            DataTemplate cellUsual = new();

            #region Usual View
            FrameworkElementFactory textBlock1Factory = new(typeof(TextBlock));
            cellUsual.VisualTree = textBlock1Factory;
            textBlock1Factory.SetBinding(TextBlock.TextProperty, new Binding(e.Column.Header.ToString()));
            #endregion
            
            #region Edit View
            FrameworkElementFactory editFactory = new(typeof(IntegerUpDown));
            editFactory.SetValue(IntegerUpDown.StyleProperty, this.FindResource(ResourceStyleManager.IntegerUpDownGridStyle) as Style);
            editFactory.SetBinding(IntegerUpDown.ValueProperty, new Binding(e.Column.Header.ToString()));
            cellEdit.VisualTree = editFactory;
            #endregion

            templateInteger.CellTemplate = cellUsual;
            templateInteger.CellEditingTemplate = cellEdit;
            e.Column = templateInteger;
        }
        private void GenerateTemplateColumnDateTime(TableFieldColumnData col, DataGridAutoGeneratingColumnEventArgs e)
        {
            DataGridTemplateColumn templateInteger = new();
            templateInteger.Header = col.VisibleName;

            DataTemplate cellEdit = new();
            DataTemplate cellUsual = new();

            #region Usual View
            FrameworkElementFactory textBlock1Factory = new(typeof(TextBlock));
            cellUsual.VisualTree = textBlock1Factory;
            textBlock1Factory.SetBinding(TextBlock.TextProperty, new Binding(e.Column.Header.ToString()));
            #endregion

            #region Edit View
            FrameworkElementFactory editFactory = new(typeof(DatePicker));
            editFactory.SetValue(IntegerUpDown.StyleProperty, this.FindResource(ResourceStyleManager.DatePickerGridStyle) as Style);
            editFactory.SetBinding(DatePicker.SelectedDateProperty, new Binding(e.Column.Header.ToString()));
            cellEdit.VisualTree = editFactory;
            #endregion

            templateInteger.CellTemplate = cellUsual;
            templateInteger.CellEditingTemplate = cellEdit;
            e.Column = templateInteger;
        }
        private MenuItem AddMenuItem(string header)
        {
            MenuItem mi = new()
            {
                Header = header
            };
            return mi;
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

        private void MoveUpClick(object sender, RoutedEventArgs e)
        {
            this.vm.MoveUpSelectedRow();
        }

        private void MoveDownClick(object sender, RoutedEventArgs e)
        {
            this.vm.MoveDownSelectedRow();
        }

        private void Table_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            this.MarkAsValidated();
            this.OnFillerUpdate?.Invoke(this);
        }
        private void ObjectCopyRequestClick(object sender, RoutedEventArgs e)
        {
            this.OnDatabaseObjectCopyRequested?.Invoke(this);
        }
        private void InsertToOther(object sender, RoutedEventArgs e)
        {
            try
            {
                OnInsert?.Invoke(this.Field.Id, this.GetData());
            }
            catch (NotNullFailed)
            {
                DialogsManager.ShowExclamationDialog("Поле является обязательным, необходимо сначала присвоить ему значение.", "Переназначение прервано");
            }
        }
        private void ColumnHeaderClick(object sender, RoutedEventArgs e)
        {
            string columnname = ((System.Windows.Controls.Primitives.DataGridColumnHeader)sender).Content.ToString();
            this.vm.SortByColumn(columnname);
        }

        private void CopyColumnClick(object sender, RoutedEventArgs e)
        {
            TableCopyingToColumn tc = new(this.vm.TableDefinition.Columns);
            if (tc.ShowDialog("Копирование колонки"))
            {
                this.vm.CopyColumnValuesToAnother(tc.GetSourceColumnName(), tc.GetTargetColumnName());
            }
        }

        private void CopyValueToAllRowsClick(object sender, RoutedEventArgs e)
        {
            ColumnSelector selector = new(this.vm.TableDefinition.Columns);
            if (selector.ShowDialog("Копирование значений"))
            {
                this.vm.CopyValueToAllRows(selector.GetTargetColumnName(), selector.OnlyEmptyOnes);
            }
        }
    }
}
