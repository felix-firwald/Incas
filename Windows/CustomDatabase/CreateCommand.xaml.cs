using Common;
using Incubator_2.Models;
using Incubator_2.ViewModels.VM_CustomDB;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Incubator_2.Windows.CustomDatabase
{

    /// <summary>
    /// Логика взаимодействия для CreateCommand.xaml
    /// </summary>
    public partial class CreateCommand : Window
    {
        public static List<string> specialWords = new()
        {
            "SELECT",
            "UPDATE",
            "DELETE",
            "FROM",
            "WHERE",
            "SET"
        };
        private VM_CreateCommand vm;
        public CreateCommand(string database, string table)
        {
            InitializeComponent();
            Command c = new();
            c.database = database;
            c.table = table;
            this.vm = new(c);
            this.DataContext = this.vm;
        }
        public CreateCommand(SCommand com)
        {
            InitializeComponent();
            Command c = com.AsModel();
            this.vm = new(c);
            this.DataContext = this.vm;
        }


        private void AddToRTBClick(object sender, RoutedEventArgs e)
        {
            MenuItem btn = (MenuItem)sender;
            switch (btn.Tag.ToString())
            {
                case "SELECT":
                    this.vm.Query = this.GetSelectQueryExample();
                    break;
                case "UPDATE":
                    this.vm.Query = this.GetUpdateExample();
                    break;
                case "DELETE":
                    this.vm.Query = $"DELETE *\nFROM [{this.vm.Table}]";
                    break;
                case "WHERE":
                    this.vm.Query += this.GetWhereExample();
                    break;
                case "WHERE NOT EQUAL":
                    this.vm.Query += this.GetWhereExample("<>");
                    break;
                case "WHERE LESS":
                    this.vm.Query += this.GetWhereExample("<", "");
                    break;
                case "WHERE MORE":
                    this.vm.Query += this.GetWhereExample(">", "");
                    break;
                case "WHEREPK":
                    this.vm.Query += this.GetWherePKExample();
                    break;
                case "PARAMFIELD":
                    this.vm.Query += this.GetParameterField();
                    break;
                case "PARAMTIME":
                    this.vm.Query += "\n{%TIME%}";
                    break;
                case "JOIN":
                    this.vm.Query = this.GetJoinExample();
                    break;
                case "ORDERASC":
                    this.vm.Query += this.GetOrderByExample("ASC");
                    break;
                case "ORDERDESC":
                    this.vm.Query += this.GetOrderByExample("DESC");
                    break;
            }
        }
        private List<string> GetFieldsDefinition(string table)
        {
            List<string> fields = this.vm.Requester.GetTableFieldsSimple(table, this.vm.Database);
            for (int f = 0; f < fields.Count; f++)
            {
                fields[f] = $"[{table}].[{fields[f]}]";
            }
            return fields;
        }

        private string GetWhereByContext()
        {
            if (!string.IsNullOrEmpty(this.vm.Query) && this.vm.Query.Contains("WHERE"))
            {
                return "\n  AND ";
            }
            return "\nWHERE ";
        }
        private string GetSelectQueryExample()
        {
            string result = "SELECT ";
            result += string.Join(",\n       ", this.GetFieldsDefinition(this.vm.Table));
            return result += $"\nFROM   [{this.vm.Table}]";
        }
        private string GetUpdateExample()
        {
            string result = "";
            if (!string.IsNullOrEmpty(this.vm.Query))
            {
                if (!this.vm.Query.Contains("UPDATE"))
                {
                    result = $"UPDATE [{this.vm.Table}]\nSET  ";
                }
                else
                {
                    result = this.vm.Query;
                }
            }
            else
            {
                result = $"UPDATE [{this.vm.Table}]\nSET  ";
            }
            BindingSelector bs = ProgramState.ShowBindingSelector(this.vm.Database, this.vm.Table, false, false);
            if (bs.Result == DialogStatus.Yes)
            {
                string value = ProgramState.ShowInputBox("Введите новое значение", $"Новое значение для поля '{bs.SelectedField}'");
                result += $",\n    [{bs.SelectedField}] = '{value}'";
                return result.Replace($"SET  ,\n", $"SET ");
            }
            return "";
        }
        private string GetWhereExample(string comparator = "=", string mark = "'")
        {
            BindingSelector bd = new(this.vm.Database, this.vm.Table, false);
            bd.ShowDialog();
            if (bd.Result == DialogStatus.Yes)
            {
                string value = ProgramState.ShowInputBox("Введите значение", $"Значение для условия к полю '{bd.SelectedField}'");
                return $"{this.GetWhereByContext()} [{bd.SelectedTable}].[{bd.SelectedField}] {comparator} {mark}{value}{mark}";
            }
            return "";
        }
        private string GetWherePKExample()
        {
            return $"{this.GetWhereByContext()} [{this.vm.Table}].[{this.vm.GetPKField()}] in ({{%SELECTED%}})";
        }
        private string GetParameterField()
        {
            BindingSelector bd = ProgramState.ShowBindingSelector(this.vm.Database, this.vm.Table, false, false);
            return $"\n{{%SELECTED#{bd.SelectedField}%}}";
        }

        private string GetJoinExample()
        {
            BindingSelector bs = new(this.vm.Database, false);
            bs.ShowDialog();
            string result;
            if (string.IsNullOrEmpty(this.vm.Query) || !this.vm.Query.Contains("JOIN"))
            {
                result = "SELECT ";
                List<string> fields = this.GetFieldsDefinition(this.vm.Table);
                List<string> joinFields = this.GetFieldsDefinition(bs.SelectedTable);
                fields.AddRange(joinFields);
                result += string.Join(",\n       ", fields);
                return result += $"\nFROM   [{this.vm.Table}]\nJOIN [{bs.SelectedTable}]\n    ON [{bs.SelectedTable}].[{bs.SelectedField}] = [{this.vm.Table}].[Поле]";
            }
            else
            {
                List<string> joinFields = this.GetFieldsDefinition(bs.SelectedTable);
                result = this.vm.Query.Replace("FROM", $"       {string.Join(",\n       ", joinFields)}\nFROM");
                return result += $"\nJOIN [{bs.SelectedTable}]\n    ON [{bs.SelectedTable}].[{bs.SelectedField}] = [{this.vm.Table}].[Поле]";
            }
        }
        private string GetOrderByExample(string type = "ASC")
        {
            BindingSelector bs = new(this.vm.Database, this.vm.Table, false, false);
            bs.ShowDialog();
            if (!this.vm.Query.Contains("ORDER BY"))
            {
                return $"\nORDER BY [{this.vm.Table}].[{bs.SelectedField}] {type}";
            }
            else
            {
                return $",\n [{this.vm.Table}].[{bs.SelectedField}] {type}";
            }
        }
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {

        }
        public static void AppendText(RichTextBox box, string text, string color)
        {
            BrushConverter bc = new BrushConverter();
            TextRange tr = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
            tr.Text = text;
            try
            {
                tr.ApplyPropertyValue(TextElement.ForegroundProperty,
                    bc.ConvertFromString(color));
            }
            catch (FormatException) { }
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (this.vm.ValidateQuery())
            {
                this.vm.Save();
                this.Close();
            }
        }
    }
}
