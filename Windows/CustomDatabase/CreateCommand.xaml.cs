using Common;
using Incubator_2.Models;
using Incubator_2.ViewModels.VM_CustomDB;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

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
        VM_CreateCommand vm;
        public CreateCommand(string database, string table)
        {
            InitializeComponent();
            Command c = new();
            c.database = database;
            c.table = table;
            vm = new(c);
            this.DataContext = vm;
        }
        public CreateCommand(SCommand com)
        {
            InitializeComponent();
            Command c = com.AsModel();
            vm = new(c);
            this.DataContext = vm;
        }


        private void AddToRTBClick(object sender, RoutedEventArgs e)
        {
            MenuItem btn = (MenuItem)sender;
            switch (btn.Tag.ToString())
            {
                case "SELECT":
                    this.vm.Query = GetSelectQueryExample();
                    break;
                case "UPDATE":
                    this.vm.Query = GetUpdateExample();
                    break;
                case "DELETE":
                    this.vm.Query = $"DELETE *\nFROM [{vm.Table}]";
                    break;
                case "WHERE":
                    this.vm.Query += GetWhereExample();
                    break;
                case "WHERE NOT EQUAL":
                    this.vm.Query += GetWhereExample("<>");
                    break;
                case "WHERE LESS":
                    this.vm.Query += GetWhereExample("<", "");
                    break;
                case "WHERE MORE":
                    this.vm.Query += GetWhereExample(">", "");
                    break;
                case "WHEREPK":
                    this.vm.Query += GetWherePKExample();
                    break;
                case "JOIN":
                    this.vm.Query = GetJoinExample();
                    break;
                case "ORDERASC":
                    this.vm.Query += GetOrderByExample("ASC");
                    break;
                case "ORDERDESC":
                    this.vm.Query += GetOrderByExample("DESC");
                    break;
            }
        }
        private List<string> GetFieldsDefinition(string table)
        {
            List<string> fields = vm.Requester.GetTableFieldsSimple(table, vm.Database);
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
            result += string.Join(",\n       ", GetFieldsDefinition(vm.Table));
            return result += $"\nFROM   [{vm.Table}]";
        }
        private string GetUpdateExample()
        {
            string result = "";
            if (!string.IsNullOrEmpty(this.vm.Query))
            {
                if (!this.vm.Query.Contains("UPDATE"))
                {
                    result = $"UPDATE [{vm.Table}]\nSET  ";
                }
                else
                {
                    result = this.vm.Query;
                }
            }
            else
            {
                result = $"UPDATE [{vm.Table}]\nSET  ";
            }
            BindingSelector bs = ProgramState.ShowBindingSelector(vm.Database, vm.Table, false, false);
            if (bs.Result == DialogStatus.Yes)
            {
                string value = ProgramState.ShowInputBox("Введите новое значение", $"Новое значение для поля '{bs.SelectedField}'");
                result += $",\n    [{bs.SelectedField}] = '{value}'";
                return result.Replace($"SET  ,\n", $"SET  ");
            }
            return "";
        }
        private string GetWhereExample(string comparator = "=", string mark = "'")
        {
            BindingSelector bd = new(vm.Database, vm.Table, false);
            bd.ShowDialog();
            if (bd.Result == DialogStatus.Yes)
            {
                string value = ProgramState.ShowInputBox("Введите значение", $"Значение для условия к полю '{bd.SelectedField}'");
                return $"{GetWhereByContext()} [{bd.SelectedTable}].[{bd.SelectedField}] {comparator} {mark}{value}{mark}";
            }
            return "";
        }
        private string GetWherePKExample()
        {
            return $"{GetWhereByContext()} [{vm.Table}].[{vm.GetPKField()}] in (%SELECTED%)";
        }

        private string GetJoinExample()
        {
            BindingSelector bs = new(vm.Database, false);
            bs.ShowDialog();
            string result;
            if (string.IsNullOrEmpty(vm.Query) || !vm.Query.Contains("JOIN"))
            {
                result = "SELECT ";
                List<string> fields = GetFieldsDefinition(vm.Table);
                List<string> joinFields = GetFieldsDefinition(bs.SelectedTable);
                fields.AddRange(joinFields);
                result += string.Join(",\n       ", fields);
                return result += $"\nFROM   [{vm.Table}]\nJOIN [{bs.SelectedTable}]\n    ON [{bs.SelectedTable}].[{bs.SelectedField}] = [{vm.Table}].[Поле]";
            }
            else
            {
                List<string> joinFields = GetFieldsDefinition(bs.SelectedTable);
                result = vm.Query.Replace("FROM", $"       {string.Join(",\n       ", joinFields)}\nFROM");
                return result += $"\nJOIN [{bs.SelectedTable}]\n    ON [{bs.SelectedTable}].[{bs.SelectedField}] = [{vm.Table}].[Поле]";
            }
        }
        private string GetOrderByExample(string type = "ASC")
        {
            BindingSelector bs = new(vm.Database, vm.Table, false, false);
            bs.ShowDialog();
            string result;
            result = $"\nORDER BY [{vm.Table}].[{bs.SelectedField}] {type}";
            return result;
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
            if (vm.ValidateQuery())
            {
                vm.Save();
                this.Close();
            }
        }
    }
}
