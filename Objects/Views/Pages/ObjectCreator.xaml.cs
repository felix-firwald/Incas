using Incas.Core.Classes;
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
        public List<TagFiller> TagFillers = new();
        public List<TableFiller> Tables = new();
        public delegate void ObjectCreatorData(ObjectCreator creator);
        public event ObjectCreatorData OnSaveRequested;
        public event ObjectCreatorData OnRemoveRequested;

        public ObjectCreator(Class source, Components.Object obj = null)
        {
            this.InitializeComponent();
            this.Class = source;
            this.FillContentPanel();
            if (obj != null)
            {
                this.ApplyObject(obj);
            }
            else
            {
                this.Object = new();
            }
            
        }
        private void FillContentPanel()
        {
            foreach (Objects.Models.Field f in this.Class.GetClassData().fields)
            {
                if (f.Type != TagType.Table)
                {
                    TagFiller tf = new(f);
                    tf.Uid = f.Id.ToString();
                    tf.OnInsert += this.Tf_OnInsert;
                    tf.OnRename += this.Tf_OnRename;
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
        public void ApplyObject(Components.Object obj)
        {
            this.Object = obj;
            foreach (FieldData data in obj.Fields)
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
            if (this.Object.Fields == null)
            {
                this.Object.Fields = new();
            }
            this.Object.Fields.Clear();
            foreach (TagFiller tf in this.TagFillers)
            {
                FieldData data = new()
                {
                    ClassFieldId = tf.field.Id,
                    Value = tf.GetData()
                };
                this.Object.Fields.Add(data);

            }
            foreach (TableFiller table in this.Tables)
            {
                FieldData data = new()
                {
                    ClassFieldId = table.field.Id,
                    Value = table.GetData()
                };
                this.Object.Fields.Add(data);
            }
            return this.Object;
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
