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
            Button btn = (Button)sender;
            switch (btn.Tag.ToString())
            {
                case "SELECT":
                    this.vm.Query = GetSelectQueryExample();
                    break;
                case "UPDATE":
                    this.vm.Query = $"UPDATE [{vm.Table}]\nSET [Поле] = 'Новое значение'";
                    break;
                case "DELETE":
                    this.vm.Query = $"DELETE *\nFROM [{vm.Table}]";
                    break;
                case "WHERE":
                    this.vm.Query += GetWhereExample();
                    break;
                case "WHEREPK":
                    this.vm.Query += GetWherePKExample();
                    break;
                case "ORDERBY":
                    this.vm.Query += $"\nORDER BY [{vm.Table}].[Поле] ASC";
                    break;
            }
        }
        private List<string> GetFieldsDefinition()
        {
            List<string> fields = vm.Requester.GetTableFieldsSimple(vm.Table, vm.Database);
            for (int f = 0; f < fields.Count; f++)
            {
                fields[f] = $"[{vm.Table}].[{fields[f]}]";
            }
            return fields;
        }

        private string GetWhereByContext()
        {
            if (this.vm.Query.Contains("WHERE"))
            {
                return "\n  AND ";
            }
            return "\nWHERE ";
        }
        private string GetSelectQueryExample()
        {
            string result = "SELECT ";
            result += string.Join(",\n       ", GetFieldsDefinition());
            return result += $"\nFROM   [{vm.Table}]";
        }
        private string GetWhereExample()
        {
            return $"{GetWhereByContext()} [{vm.Table}].[Поле] = 'значение'";
        }
        private string GetWherePKExample()
        {
            return $"{GetWhereByContext()} [{vm.Table}].[{vm.GetPKField()}] in (%SELECTED%)";
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
