using Incas.CustomDatabases.Models;
using Incubator_2.Common;
using Incubator_2.Forms;
using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace Incas.CustomDatabases.Views.Windows
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
            this.InitializeComponent();
            this.Table = tableName;
            this.path = pathDb;
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
            this.InitializeComponent();
            this.Table = tableName;
            this.PK = pk;
            this.PKValue = pkValue;
            this.path = pathDb;
            CustomTable ct = new();
            DataRow dr = ct.GetOneFromTable(this.Table, pk, pkValue, pathDb);
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
        public CreateRecord(string tableName, List<FieldCreator> fields, string pathDb, DataRow dr) // copy
        {
            this.InitializeComponent();
            this.Table = tableName;
            this.path = pathDb;
            CustomTable ct = new();
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
            Dictionary<string, string> pairs = [];
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
            if (this.PK == null)
            {
                c.InsertInTable(this.Table, this.path, pairs);
            }
            else
            {
                c.UpdateInTable(this.Table, this.PK, this.PKValue, this.path, pairs);
            }
            return true;
        }

        private void SaveAndCloseClick(object sender, RoutedEventArgs e)
        {
            if (this.Save())
            {
                this.Close();
            }
        }
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            this.Save();
        }
    }
}
