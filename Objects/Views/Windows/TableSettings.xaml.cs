using Incas.Core.Classes;
using Incas.Objects.Components;
using Incas.Objects.Exceptions;
using Incas.Objects.Views.Controls;
using Incas.Templates.Components;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;

namespace Incas.Objects.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для TableSettings.xaml
    /// </summary>
    public partial class TableSettings : Window
    {
        private string TableName { get; set; }
        public TableFieldData Data { get; set; }
        public TableSettings(string value, string tableName)
        {
            this.InitializeComponent();
            this.TableName = tableName;
            this.Title = $"Редактирование таблицы ({tableName})";
            try
            {
                this.Data = JsonConvert.DeserializeObject<TableFieldData>(value);
                this.FillPanel(this.Data.Columns);
            }
            catch
            {
                this.Data = new();
                this.Data.Columns = new();
            }
        }
        public void FillPanel(List<TableFieldColumnData> columns)
        {
            this.ContentPanel.Children.Clear();
            foreach (TableFieldColumnData column in columns)
            {
                this.AddColumnCreator(column);
            }
        }
        private void AddColumnCreator(TableFieldColumnData col)
        {
            Views.Controls.TableColumnCreator creator = new(col);
            creator.OnRemove += this.Creator_OnRemove;
            creator.OnMoveDownRequested += this.Creator_OnMoveDownRequested;
            creator.OnMoveUpRequested += this.Creator_OnMoveUpRequested;
            this.ContentPanel.Children.Add(creator);
        }

        private int Creator_OnMoveUpRequested(Views.Controls.TableColumnCreator t)
        {
            int position = this.ContentPanel.Children.IndexOf(t);
            if (position < this.ContentPanel.Children.Count - 1)
            {
                position += 1;
            }
            this.ContentPanel.Children.Remove(t);
            this.ContentPanel.Children.Insert(position, t);
            return position;
        }

        private int Creator_OnMoveDownRequested(Views.Controls.TableColumnCreator t)
        {
            int position = this.ContentPanel.Children.IndexOf(t);
            if (position > 0)
            {
                position -= 1;
            }

            this.ContentPanel.Children.Remove(t);
            this.ContentPanel.Children.Insert(position, t);
            return position;
        }

        private bool Creator_OnRemove(Views.Controls.TableColumnCreator t)
        {
            this.ContentPanel.Children.Remove(t);
            return true;
        }

        private void FinishClick(object sender, RoutedEventArgs e)
        {
            try
            {
                List<TableFieldColumnData> list = new();
                foreach (Views.Controls.TableColumnCreator creator in this.ContentPanel.Children)
                {
                    list.Add(creator.GetField());
                }
                this.Data.Columns = list;
                this.DialogResult = true;
            }
            catch (FieldDataFailed fail)
            {
                DialogsManager.ShowExclamationDialog(fail.Message, "Сохранение прервано");
            }
        }

        private void AddFieldClick(object sender, MouseButtonEventArgs e)
        {
            this.AddColumnCreator(new());
        }

        private void MinimizeAllClick(object sender, MouseButtonEventArgs e)
        {
            foreach (TableColumnCreator tcc in this.ContentPanel.Children)
            {
                tcc.Minimize();
            }
        }

        private void MaximizeAllClick(object sender, MouseButtonEventArgs e)
        {
            foreach (TableColumnCreator tcc in this.ContentPanel.Children)
            {
                tcc.Maximize();
            }
        }

        private void FindColumnsInFile(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog fd = new()
            {
                Filter = "Word и Excel|*.docx;*.xlsx",
                InitialDirectory = ProgramState.CurrentWorkspace.GetSourcesTemplatesFolder(),
            };
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    ITemplator templ;
                    if (fd.FileName.EndsWith(".docx"))
                    {
                        templ = new WordTemplator(fd.FileName);
                        this.ApplyColumnsFromTemplator(templ.FindTableTags(this.TableName));
                    }
                    else
                    {
                        templ = new ExcelTemplator(fd.FileName);
                        this.ApplyColumnsFromTemplator(templ.FindTableTags(this.TableName));
                    }
                }
                catch (IOException ioex)
                {
                    DialogsManager.ShowErrorDialog("При доступе к файл возникла ошибка. Описание ошибки: " + ioex.Message);
                }
                catch { }
            }
        }
        private void ApplyColumnsFromTemplator(List<string> columns)
        {
            foreach (string column in columns)
            {
                TableFieldColumnData tf = new();
                tf.Name = column;
                tf.VisibleName = column.Replace("_", " ");
                this.AddColumnCreator(tf);
            }
        }
    }
}
