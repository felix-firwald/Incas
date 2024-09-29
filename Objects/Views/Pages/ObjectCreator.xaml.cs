using DocumentFormat.OpenXml.Wordprocessing;
using Incas.Core.Classes;
using Incas.Objects.Components;
using Incas.Objects.Models;
using Incas.Objects.Views.Windows;
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
        public delegate void FieldCopyAction(Guid id, string text);
        public event FieldCopyAction OnInsertRequested;
        public event ObjectCreatorData OnSaveRequested;
        public event ObjectCreatorData OnRemoveRequested;
        private bool Locked = false;
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
            this.ExpanderButton.IsChecked = true;
            this.ApplyAuthorConstraint();
        }
        private void ApplyAuthorConstraint()
        {
            if (this.Object.Id != Guid.Empty && this.ClassData.EditByAuthorOnly == true && this.Object.AuthorId != ProgramState.CurrentUser.id)
            {
                this.Locked = true;
                this.ContentPanel.IsEnabled = false;
                Label label = new()
                {
                    Content = "Вы не можете редактировать этот объект, поскольку не являетесь его автором.",
                    Margin = new Thickness(5),
                    Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0))
                };
                this.ContentPanel.Children.Insert(0, label);
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
                    tf.OnDatabaseObjectCopyRequested += this.Tf_OnDatabaseObjectCopyRequested;
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

        private void Tf_OnDatabaseObjectCopyRequested(TagFiller sender)
        {
            BindingData bd = new()
            {
                Class = this.Class.identifier,
                Field = sender.field.Id
            };
            DatabaseSelection ds = new(bd);
            ds.ShowDialog();
            foreach (Components.FieldData field in ds.SelectedObject?.Fields)
            {
                if (field.ClassField.Id == sender.field.Id)
                {
                        
                    sender.SetValue(field.Value);
                    return;
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
                    if (tagfiller.field.Id == data.ClassField.Id)
                    {
                        tagfiller.SetValue(data.Value);
                        break;
                    }
                }
                foreach (TableFiller table in this.Tables)
                {
                    if (table.field.Id == data.ClassField.Id)
                    {
                        table.SetData(data.Value);
                        break;
                    }
                }
            }
        }
        public Components.Object PullObject()
        {
            if (this.Locked == true)
            {
                throw new Exceptions.AuthorFailed($"Объект с именем \"{this.Object.Name}\" не может быть модифицирован, поскольку не вы являетесь его автором.");
            }
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
                    ClassField = tf.field,
                    Value = tf.GetData()
                };
                this.Object.Fields.Add(data);

            }
            foreach (TableFiller table in this.Tables)
            {
                Components.FieldData data = new()
                {
                    ClassField = table.field,
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
            this.OnInsertRequested?.Invoke(tag, text);
        }
        public void InsertToField(Guid id, string data)
        {
            foreach (TagFiller tf in this.TagFillers)
            {
                if (tf.field.Id == id)
                {
                    tf.SetValue(data);
                    return;
                }
            }
        }

        public void Maximize()
        {
            this.ExpanderButton.IsChecked = true;
        }
        public void Minimize()
        {
            this.ExpanderButton.IsChecked = false;
        }
        private void MaximizeClick(object sender, RoutedEventArgs e)
        {
            this.MainBorder.Height = this.ContentPanel.Height + 40;
        }
        private void MinimizeClick(object sender, RoutedEventArgs e)
        {
            this.MainBorder.Height = 40;
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
    }
}
