using Common;
using Incubator_2.Common;
using Incubator_2.Forms;
using Incubator_2.Models;
using System;
using System.Collections.Generic;
using System.Data;
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
        public string PK;
        public string PKValue;
        private string path;
        public CreateRecord(string tableName, List<FieldCreator> fields, string pathDb) // add new
        {
            InitializeComponent();
            Table = tableName;
            path = pathDb;
            foreach (FieldCreator field in fields)
            {
                if (!field.IsPK)
                {
                    this.ContentPanel.Children.Add(new UC_TagFiller(field, pathDb));
                }
            }
        }
        public CreateRecord(string tableName, string pk, string pkValue, List<FieldCreator> fields, string pathDb) // update
        {
            InitializeComponent();
            Table = tableName;
            PK = pk;
            PKValue = pkValue;
            path = pathDb;
            CustomTable ct = new();
            DataRow dr = ct.GetOneFromTable(Table, pk, pkValue, pathDb);
            foreach (FieldCreator field in fields)
            {
                if (!field.IsPK)
                {
                    UC_TagFiller tf = new(field, pathDb);
                    tf.SetValue(dr[field.Name].ToString());
                    this.ContentPanel.Children.Add(tf);
                }
            }
            this.SaveButton.IsEnabled = false;
        }
        private bool Save()
        {
            Dictionary<string, string> pairs = new();
            foreach (UC_TagFiller tf in this.ContentPanel.Children)
            {
                if (tf.ValidateContent())
                {
                    pairs.Add(tf.GetTagName(), tf.GetValue());
                }
                else
                {
                    return false;
                }
            }
            CustomTable c = new();
            if (PK == null)
            {
                c.InsertInTable(Table, path, pairs);
            }
            else
            {
                c.UpdateInTable(Table, PK, PKValue, path, pairs);
            }
            return true;
        }

        private void SaveAndCloseClick(object sender, RoutedEventArgs e)
        {
            if (Save())
            {
                this.Close();
            }
        }
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            Save();
        }
    }
}
