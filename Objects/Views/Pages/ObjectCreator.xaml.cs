using DocumentFormat.OpenXml.Wordprocessing;
using Incas.Objects.Components;
using Incas.Objects.Models;
using Incas.Templates.Components;
using Incas.Templates.Views.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incas.Objects.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ObjectCreator.xaml
    /// </summary>
    public partial class ObjectCreator : UserControl
    {
        public Components.Object Object { get; set; }
        public Class Class { get; set; }
        public ClassData ClassData { get; set; }
        public List<TagFiller> TagFillers = new();
        public List<TableFiller> Tables = new();
        public delegate void ObjectCreatorData(ObjectCreator creator);
        public event ObjectCreatorData OnSaveRequested;
        public event ObjectCreatorData OnRemoveRequested;

        public ObjectCreator(Class source, ClassData data, Components.Object obj = null)
        {
            this.InitializeComponent();
            this.Class = source;
            this.ClassData = data;
            this.FillContentPanel();
            if (obj != null)
            {
                this.ApplyObject(obj);
            }
            else
            {
                this.Object = new();
            }           
            if (data.ClassType != ClassType.Document)
            {
                this.RenderArea.Visibility = Visibility.Collapsed;
            }
        }
        private void FillContentPanel()
        {
            foreach (Objects.Models.Field f in this.ClassData.fields)
            {
                if (f.Type != TagType.Table)
                {
                    TagFiller tf = new(f);
                    tf.Uid = f.Id.ToString();
                    tf.OnInsert += this.Tf_OnInsert;
                    tf.OnRename += this.Tf_OnRename;
                    tf.OnFieldUpdate += this.Tf_OnFieldUpdate;
                    //tf.OnScriptRequested += this.OnScriptRequested;
                    this.ContentPanel.Children.Add(tf);
                    this.TagFillers.Add(tf);
                }
                else
                {
                    TableFiller tf = new(f);
                    tf.Uid = f.Id.ToString();
                    this.ContentPanel.Children.Add(tf);
                    this.Tables.Add(tf);
                }
            }
        }

        private void Tf_OnFieldUpdate(TagFiller sender)
        {
            
        }

        public void ApplyObject(Components.Object obj)
        {
            this.Object = obj;
            this.ObjectName.Text = obj.Name;
            foreach (Components.FieldData data in obj.Fields)
            {
                foreach (TagFiller tagfiller in this.TagFillers)
                {
                    //DialogsManager.ShowInfoDialog(tagfiller);
                    if (tagfiller.field.Id == data.ClassFieldId)
                    {
                        tagfiller.SetValue(data.Value);
                        break;
                    }
                }
                foreach (TableFiller table in this.Tables)
                {
                    if (table.field.Id == data.ClassFieldId)
                    {
                        table.SetData(data.Value);
                        break;
                    }
                }
            }
        }
        public Components.Object PullObject()
        {
            this.UpdateName();
            this.Object.Name = this.ObjectName.Text;
            if (this.Object.Fields == null)
            {
                this.Object.Fields = new();
            }
            this.Object.Fields.Clear();
            foreach (TagFiller tf in this.TagFillers)
            {
                Components.FieldData data = new()
                {
                    ClassFieldId = tf.field.Id,
                    Value = tf.GetData()
                };
                this.Object.Fields.Add(data);

            }
            foreach (TableFiller table in this.Tables)
            {
                Components.FieldData data = new()
                {
                    ClassFieldId = table.field.Id,
                    Value = table.GetData()
                };
                this.Object.Fields.Add(data);
            }
            return this.Object;
        }
        private string UpdateName()
        {
            string name = "";
            if (this.ClassData.NameTemplate is not null)
            {
                name = this.ClassData.NameTemplate;
            }
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrEmpty(this.ObjectName.Text))
            {
                name = "Объект от " + DateTime.Now;
                this.ObjectName.Text = name;
                return name;
            }
            foreach (TagFiller tf in this.TagFillers)
            {
                string val = tf.GetValue();
                if (val != null)
                {
                    name = name.Replace("[" + tf.GetTagName() + "]", val);
                }               
            }
            this.ObjectName.Text = name;
            return name;
        }

        private void Tf_OnRename(string tag)
        {
            
        }

        private void Tf_OnInsert(Guid tag, string text)
        {
            
        }

        private void MaximizeClick(object sender, RoutedEventArgs e)
        {

        }

        private void SaveClick(object sender, MouseButtonEventArgs e)
        {
            this.OnSaveRequested?.Invoke(this);
        }

        private void PreviewCLick(object sender, MouseButtonEventArgs e)
        {

        }

        private void RemoveClick(object sender, MouseButtonEventArgs e)
        {
            this.OnRemoveRequested?.Invoke(this);
        }

        private void MinimizeClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
