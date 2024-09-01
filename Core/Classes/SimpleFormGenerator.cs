using Incas.Core.Attributes;
using Incas.Core.AutoUI;
using Incas.Core.Views.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Incas.Core.Classes
{
    internal class SimpleFormGenerator
    {
        public StackPanel Container;
        public AutoUIBase Result;
        public SimpleFormGenerator(AutoUIBase values, string title)
        {
            this.SafetyCallMethod("Load");
            foreach (PropertyInfo field in values.GetType().GetProperties())
            {
                this.AddField(field);
            }
        }
        private string GetFieldDescription(PropertyInfo field)
        {
            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>(true);
            return attribute != null ? attribute.Description : "";
        }
        private void SafetyCallMethod(string name)
        {
            MethodInfo method = this.Result.GetType().GetMethod(name);
            if (method is not null)
            {
                try
                {
                    method.Invoke(this.Result, null);
                }
                catch (TargetInvocationException)
                {

                }
            }
        }
        private void AddField(PropertyInfo field)
        {
            string description = this.GetFieldDescription(field);
            Control control = new();
            Label label = new()
            {
                Content = description + ":",
                FontSize = 11,
                Style = this.Container.FindResource("LabelPrimary") as Style
            };
            switch (field.PropertyType.Name)
            {
                case "String":
                    Attribute attr = field.GetCustomAttribute(typeof(UrlRequired), false);
                    control = attr is not null
                        ? this.GeneratePathBox(description, (string)field.GetValue(this.Result))
                        : this.GenerateTextBox(description, (string)field.GetValue(this.Result));
                    break;
                case "Int32":
                    control = this.GenerateNumericBox(description, (int)field.GetValue(this.Result));
                    this.Container.Children.Add(label);
                    break;
                case "Boolean":
                    control = this.GenerateCheckBox(description, (bool)field.GetValue(this.Result));
                    break;
                case "DateTime":
                    control = this.GenerateDateBox(description, (DateTime)field.GetValue(this.Result));
                    this.Container.Children.Add(label);
                    break;
                case "DataTable":
                    control = this.GenerateDataGrid(description, (DataTable)field.GetValue(this.Result));
                    this.Container.Children.Add(label);
                    break;
                case "ComboSelector":
                    control = this.GenerateComboBox(description, (ComboSelector)field.GetValue(this.Result));
                    this.Container.Children.Add(label);
                    break;
                default:
                    if (field.PropertyType.Name.Contains("List"))
                    {
                        control = this.GenerateDataGrid(description, (List<string>)field.GetValue(this.Result));
                        this.Container.Children.Add(label);
                    }
                    else
                    {
                        return;
                    }
                    break;
            }
            control.Uid = field.Name;
            if (field.SetMethod.IsPrivate)
            {
                control.IsEnabled = false;
            }
            this.Container.Children.Add(control);
        }
        private Control GenerateTextBox(string description, string value)
        {
            TextBox control = new()
            {
                Tag = description,
                Text = value,
                Style = this.Container.FindResource("TextBoxMain") as Style
            };
            control.TextChanged += this.TextBox_TextChanged;
            return control;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private Control GeneratePathBox(string description, string value)
        {
            PathSelector control = new()
            {
                Value = value,
                Tag = description,
            };
            return control;
        }
        private Control GenerateComboBox(string description, ComboSelector selector)
        {
            ComboBox control = new()
            {
                Tag = description,
                ItemsSource = selector.VisibleItems,
                SelectedValue = selector.SelectedValue,
                Style = this.Container.FindResource("ComboBoxMain") as Style
            };
            return control;
        }
        private Control GenerateNumericBox(string description, int value)
        {
            NumericBox control = new()
            {
                Tag = description,
                Value = value,
            };
            return control;
        }
        private Control GenerateDateBox(string description, DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                value = DateTime.Today;
            }
            DatePicker control = new()
            {
                Tag = description,
                SelectedDate = value,
                Style = this.Container.FindResource("DatePickerMain") as Style
            };
            return control;
        }
        private Control GenerateDataGrid(string description, DataTable value)
        {
            DataGrid control = new()
            {
                Tag = description,
                ItemsSource = value?.DefaultView,
                MinHeight = 80,
                Style = this.Container.FindResource("DataGridMain") as Style
            };
            return control;
        }
        private Control GenerateDataGrid(string description, List<string> value)
        {
            DataTable dt = new();
            dt.Columns.Add("Значение");
            foreach (string item in value)
            {
                dt.Rows.Add(item);
            }
            DataGrid control = new()
            {
                Tag = description,
                MinHeight = 80,
                ItemsSource = dt.DefaultView,
                Style = this.Container.FindResource("DataGridMain") as Style
            };

            return control;
        }
        private Control GenerateDataGrid(string description, Dictionary<string, string> value)
        {
            DataTable dt = new();
            dt.Columns.Add("Ключ");
            dt.Columns.Add("Значение");
            foreach (KeyValuePair<string, string> item in value)
            {
                dt.Rows.Add(item.Key, item.Value);
            }
            DataGrid control = new()
            {
                Tag = description,
                MinHeight = 80,
                ItemsSource = dt.DefaultView,
                Style = this.Container.FindResource("DataGridMain") as Style
            };

            return control;
        }
        private Control GenerateCheckBox(string description, bool value)
        {
            CheckBox control = new()
            {
                Content = description,
                Tag = description,
                IsChecked = value,
                Style = this.Container.FindResource("Toggle") as Style
            };
            return control;
        }
        public bool Save()
        {
            foreach (PropertyInfo field in this.Result.GetType().GetProperties())
            {
                foreach (Control control in this.Container.Children)
                {
                    string descript = this.GetFieldDescription(field);
                    if (control.Uid == field.Name)
                    {
                        switch (field.PropertyType.Name)
                        {
                            case "String":
                                string resultString = "";
                                Attribute attr = field.GetCustomAttribute(typeof(UrlRequired), false);
                                resultString = attr is not null ? ((PathSelector)control).Value : ((TextBox)control).Text;
                                if (string.IsNullOrEmpty(resultString))
                                {
                                    DialogsManager.ShowExclamationDialog($"Поле \"{descript}\" не заполнено!", "Сохранение прервано");
                                    return false;
                                }
                                field.SetValue(this.Result, resultString);
                                break;
                            case "Int32":
                                field.SetValue(this.Result, ((NumericBox)control).Value);
                                break;
                            case "Boolean":
                                field.SetValue(this.Result, ((CheckBox)control).IsChecked);
                                break;
                            case "DateTime":
                                field.SetValue(this.Result, ((DatePicker)control).SelectedDate);
                                break;
                            case "DataTable":
                                field.SetValue(this.Result, ((DataView)((DataGrid)control).ItemsSource).ToTable());
                                break;
                            case "ComboSelector":
                                if (((ComboBox)control).SelectedValue is null)
                                {
                                    DialogsManager.ShowExclamationDialog("Одно из полей не заполнено.", "Сохранение прервано");
                                    return false;
                                }
                                ((ComboSelector)field.GetValue(this.Result)).SetSelection(((ComboBox)control).SelectedValue.ToString());
                                break;
                            default:
                                if (field.PropertyType.Name.Contains("List"))
                                {
                                    DataTable dt = ((DataView)((DataGrid)control).ItemsSource).ToTable();
                                    List<string> values = [];
                                    foreach (DataRow row in dt.Rows)
                                    {
                                        values.Add(row[0].ToString());
                                    }
                                    field.SetValue(this.Result, values);
                                }
                                else if (field.PropertyType.Name.Contains("Dictionary"))
                                {
                                    DataTable dt = ((DataView)((DataGrid)control).ItemsSource).ToTable();
                                    Dictionary<string, string> values = [];
                                    foreach (DataRow row in dt.Rows)
                                    {
                                        values.Add(row[0].ToString(), row[1].ToString());
                                    }
                                    field.SetValue(this.Result, values);
                                }
                                break;
                        }
                        break;
                    }
                }
            }
            this.SafetyCallMethod("Save");
        }
    }
}
