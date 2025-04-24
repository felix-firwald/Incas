using DocumentFormat.OpenXml.Office2010.Excel;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using Incas.Objects.Interfaces;
using Incas.Objects.Views.Pages;
using IncasEngine.Core;
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
        private UIElement placedElement;
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
                switch (data.ClassField.Type)
                {
                    case FieldType.Object:
                        try
                        {
                            Guid id = Guid.Parse(data.Value.ToString());
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
                        break;
                    case FieldType.Date:
                        this.FieldValue.Text = data.Value.ToString();
                        this.GenerateDateTimeField();
                        break;
                    case FieldType.Boolean:
                        CheckBox box = new();
                        box.Style = this.FindResource("CheckBoxDataGridUsual") as Style;
                        box.HorizontalAlignment = HorizontalAlignment.Left;
                        box.IsChecked = this.Data.Value.ToString() == bool.TrueString;
                        box.Margin = new Thickness(5);
                        this.MainGrid.Children.Add(box);
                        Grid.SetColumn(box, 1);
                        break;
                    case FieldType.LocalEnumeration:
                    case FieldType.GlobalEnumeration:
                        this.FieldValue.Text = data.Value.ToString();
                        this.ColorizeField(255, 251, 0);
                        break;
                    default:
                        this.FieldValue.Text = data.Value.ToString();
                        break;
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
            this.FieldValue.Text = date.ToString();
            this.GenerateDateTimeField();
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
            this.relationClass = EngineGlobals.GetClass(bd.BindingClass);
            this.relationObject = id;
            this.FieldValue.Text = Processor.GetObjectFieldValue(this.relationClass, this.relationObject, bd.BindingField.ToString());
            this.ColorizeField(130, 113, 239);
            this.FieldValue.Cursor = System.Windows.Input.Cursors.Hand;
            this.FieldValue.MouseDown += this.FieldValue_MouseDown;
            this.FieldValue.ToolTip = "Кликнуть для просмотра объекта";
        }
        private void GenerateDateTimeField()
        {           
            this.ColorizeField(74, 243, 170);
            this.FieldValue.Cursor = System.Windows.Input.Cursors.Hand;
            this.FieldValue.MouseDown += this.FieldValue_MouseDownToDateTime;
            this.FieldValue.ToolTip = "Кликнуть для просмотра календаря";
        }

        private void FieldValue_MouseDownToDateTime(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.isNestedObjectShowed == false)
            {
                this.isNestedObjectShowed = true;
                try
                {
                    Calendar c = new();
                    DateTime dt = DateTime.Parse(this.FieldValue.Text);
                    c.SelectedDate = dt;
                    c.DisplayDate = dt;
                    c.Style = ResourceStyleManager.FindStyle("CalendarMain");
                    this.placedElement = c;
                    this.MainGrid.Children.Add(c);
                    Grid.SetRow(c, 1);
                    Grid.SetColumnSpan(c, 3);
                }
                catch
                {
                    ContentControl cc = new();
                    cc.Style = ResourceStyleManager.FindStyle("BoxError");
                    cc.Content = "Не удалось распознать дату.";
                    this.placedElement = cc;
                    this.MainGrid.Children.Add(cc);
                    Grid.SetRow(cc, 1);
                    Grid.SetColumnSpan(cc, 3);
                }
            }
            else
            {
                this.isNestedObjectShowed = false;
                this.MainGrid.Children.Remove(this.placedElement);
            }
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
                Value = this.Data.Value,
                ClassField = this.Data.ClassField
            };
            if (this.Data.ClassField.Type == FieldType.Object)
            {
                data.Value = this.FieldValue.Text;
            }
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
                            Clipboard.SetText(this.Data.Value.ToString());
                            break;
                        case FieldType.Object:
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
