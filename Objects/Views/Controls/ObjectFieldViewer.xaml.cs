using DocumentFormat.OpenXml.Office2010.Excel;
using Incas.Core.Classes;
using Incas.Objects.Interfaces;
using Incas.Objects.Views.Pages;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Incas.Objects.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для ObjectFieldViewer.xaml
    /// </summary>
    public partial class ObjectFieldViewer : UserControl, IObjectFieldViewer
    {
        private IClass relationClass;
        private Guid relationObject;
        private bool isNestedObjectShowed = false;
        private ObjectCard card;
        public delegate void FieldDataAction(FieldData data);
        public event FieldDataAction OnFilterRequested;
        public FieldData Data { get; private set; }
        public ObjectFieldViewer(FieldData data, bool first = true)
        {
            this.InitializeComponent();
            this.Data = data;
            this.FieldName.Text = data.ClassField.VisibleName + ":";
            if (!first)
            {
                this.FilterButton.Visibility = Visibility.Collapsed;
            }
            if (data.ClassField.Confidential)
            {
                this.ConfidentialButton.Visibility = Visibility.Visible;
                this.FieldValue.Visibility = Visibility.Collapsed;
                this.FilterButton.IsEnabled = false;
            }
            else
            {
                if (data.ClassField.Type == FieldType.Relation)
                {
                    try
                    {
                        Guid id = Guid.Parse(data.Value);
                        if (id != Guid.Empty)
                        {
                            BindingData bd = data.ClassField.GetBindingData();
                            this.GenerateRelatedField(id, bd);
                        }
                    }
                    catch
                    {
                        this.FieldValue.Text = "(объект не выбран)";
                        this.ColorizeField(200, 0, 90);
                        this.FilterButton.IsEnabled = false;
                    }
                }
                else if (data.ClassField.Type == FieldType.Table)
                {
                    this.FieldValue.Text = "(таблица)";
                    this.FilterButton.IsEnabled = false;
                    this.ColorizeField(67, 70, 80);
                }
                else
                {
                    this.FieldValue.Text = data.Value;
                }
            }           
            if (!data.ClassField.ListVisibility)
            {
                this.FilterButton.IsEnabled = false;
            }
        }
        public ObjectFieldViewer(ServiceClass cl, Guid data, string label) // all service fields with links (author, group, etc.)
        {
            this.InitializeComponent();
            this.FieldName.Text = label + ":";
            this.relationClass = cl;
            this.relationObject = data;
            this.FilterButton.Visibility = Visibility.Collapsed;
            string value = Processor.GetObjectFieldValue(cl, data, Helpers.NameField);
            if (string.IsNullOrEmpty(value))
            {
                this.FieldValue.Text = "(объект не определен)";
                this.ColorizeField(200, 0, 90);
            }
            else
            {
                this.FieldValue.Text = value;
                this.FieldValue.Cursor = System.Windows.Input.Cursors.Hand;
                this.FieldValue.MouseDown += this.FieldValue_MouseDown;
                this.FieldValue.ToolTip = "Кликнуть для просмотра объекта";
                this.ColorizeField(243, 74, 147);
            }           
        }
        public ObjectFieldViewer(DateTime date, string name) // date
        {
            this.InitializeComponent();
            this.FieldName.Text = name + ":";
            this.FieldValue.Text = date.ToString("f");
            this.ColorizeField(74, 243, 170);
            this.FilterButton.Visibility = Visibility.Collapsed;
        }
        public ObjectFieldViewer(string value, byte r, byte g, byte b) // custom
        {
            this.InitializeComponent();
            this.FieldName.Text = value;
            this.FieldValue.Text = "";
            Grid.SetColumnSpan(this.FieldName, 2);
            this.ColorizeName(r, g, b);
            this.FilterButton.Visibility = Visibility.Collapsed;
        }

        private void GenerateRelatedField(Guid id, BindingData bd)
        {
            this.relationClass = new Class(bd.Class);
            this.relationObject = id;
            this.FieldValue.Text = Processor.GetObjectFieldValue(this.relationClass, this.relationObject, bd.Field.ToString());
            this.ColorizeField(130, 113, 239);
            this.FieldValue.Cursor = System.Windows.Input.Cursors.Hand;
            this.FieldValue.MouseDown += this.FieldValue_MouseDown;
            this.FieldValue.ToolTip = "Кликнуть для просмотра объекта";
        }

        private void FieldValue_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.isNestedObjectShowed == false)
            {
                this.isNestedObjectShowed = true;
                this.card = new(this.relationClass, false);             
                this.card.UpdateFor(Processor.GetObject(this.relationClass, this.relationObject));
                this.MainGrid.Children.Add(this.card);
                Grid.SetRow(this.card, 1);
                Grid.SetColumnSpan(this.card, 3);
            }
            else
            {
                this.isNestedObjectShowed = false;
                this.MainGrid.Children.Remove(this.card);
            }
        }

        
        public void HideSeparator()
        {
            this.Separator.Visibility = Visibility.Collapsed;
        }
        private void ColorizeField(byte r, byte g, byte b)
        {
            this.FieldValue.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(r, g, b));
        }
        private void ColorizeName(byte r, byte g, byte b)
        {
            this.FieldName.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(r, g, b));
        }

        private void FilterClick(object sender, RoutedEventArgs e)
        {
            FieldData data = new()
            {
                Value = this.FieldValue.Text,
                ClassField = this.Data.ClassField
            };
            this.OnFilterRequested?.Invoke(data);
        }

        private void CopyClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (this.Data.ClassField is not null)
                {
                    switch (this.Data.ClassField.Type)
                    {
                        default:
                            Clipboard.SetText(this.Data.Value);
                            break;
                        case FieldType.Table:
                            break;
                        case FieldType.Relation:
                            Clipboard.SetText(this.FieldValue.Text);
                            break;
                    }
                }              
            }
            catch { }
        }

        private void ConfidentialButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
