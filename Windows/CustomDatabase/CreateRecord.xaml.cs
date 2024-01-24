using Incubator_2.Common;
using Incubator_2.Forms;
using Incubator_2.Models;
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
    /// Логика взаимодействия для CreateRecord.xaml
    /// </summary>
    public partial class CreateRecord : Window
    {
        public readonly string Table;
        public CreateRecord(string tableName, List<FieldCreator> fields)
        {
            InitializeComponent();
            Table = tableName;
            foreach (FieldCreator field in fields)
            {
                if (!field.IsPK)
                {
                    this.ContentPanel.Children.Add(new UC_TagFiller(field));
                }
            }
        }
        private void Save()
        {
            Dictionary<string, string> pairs = new();
            foreach (UC_TagFiller tf in this.ContentPanel.Children)
            {
                pairs.Add(tf.GetTagName(), tf.GetValue());
            }
            CustomTable c = new();
            c.InsertInTable(Table, pairs);
        }

        private void SaveAndCloseClick(object sender, RoutedEventArgs e)
        {
            Save();
            this.Close();
        }
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            Save();
        }
    }
}
