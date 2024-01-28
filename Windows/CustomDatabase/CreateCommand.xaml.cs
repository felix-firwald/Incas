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
            vm = new(database, table);
            this.DataContext = vm;
            BindingSelector bs = new();
            bs.ShowDialog();
        }

        private TextRange GetTextRange()
        {
            return new TextRange(RTB.Document.ContentStart, RTB.Document.ContentEnd);
        }

        private void AddToRTBClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            switch (btn.Tag.ToString())
            {
                case "SELECT":
                    this.RTB.Document.Blocks.Clear();
                    this.RTB.AppendText(GetSelectQueryExample());
                    break;
                case "UPDATE":
                    this.RTB.Document.Blocks.Clear();
                    this.RTB.AppendText($"UPDATE [{vm.Table}]\nSET [Поле] = 'Новое значение'");
                    break;
                case "DELETE":
                    this.RTB.Document.Blocks.Clear();
                    this.RTB.AppendText($"DELETE *\nFROM [{vm.Table}]");
                    break;
                case "WHERE":
                    this.RTB.AppendText($"\nWHERE [{vm.Table}].[Поле] = 'Нужное значение'");
                    break;
                case "ORDERBY":
                    this.RTB.AppendText($"\nORDER BY [{vm.Table}].[Поле] ASC");
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
        private string GetSelectQueryExample()
        {
            string result = "SELECT ";
            result += string.Join(",\n       ", GetFieldsDefinition());
            return result += $"\nFROM   [{vm.Table}]";
        }
        private string GetSelectWhereExample()
        {
            string result = "WHERE ";
            result += string.Join(",\n       ", GetFieldsDefinition());
            return result += $"\nFROM   [{vm.Table}]";
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
        private Dictionary<int, int> GetEntriesOfSubstring(string substring, string text)
        {
            int startIndex = 0;
            Dictionary<int, int> entries = new();
            while (startIndex < text.Length)
            {
                int start = text.IndexOf(text, startIndex);

                if (start < 0)
                {
                    return entries;
                }
                else
                {
                    startIndex = start + text.Length + 1;
                    
                }
            }
            return entries;
        }
        private void Colorize()
        {
            //string[] words = GetTextRange().Text.Split(new string[] { "\r\n", "\r", "\n", " " }, StringSplitOptions.None);
            //this.RTB.Document.Blocks.Clear();
            //foreach (string word in words)
            //{
            //    if (specialWords.Contains(word.Trim()))
            //    {
            //        AppendText(this.RTB, word, "#0cfb00");
            //    }
            //    else
            //    {
            //        AppendText(this.RTB, word, "White");
            //    }
            //    this.RTB.AppendText(" ");
            //}
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            Colorize();
        }
    }
}
