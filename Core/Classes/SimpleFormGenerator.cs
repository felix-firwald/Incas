using Incas.Core.Attributes;
using Incas.Core.AutoUI;
using Incas.Core.Views.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Incas.Core.Classes
{
    internal class SimpleFormGenerator
    {
        public StackPanel Container;
        public AutoUIBase Result;
        public SimpleFormGenerator(AutoUIBase values, StackPanel container)
        {
            this.Result = values;
            this.SafetyCallMethod("Load");
            this.Container = container;
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
        private int GetFieldTextMaxLength(PropertyInfo field)
        {
            StringLengthAttribute attribute = field.GetCustomAttribute<StringLengthAttribute>(true);
            return attribute != null ? attribute.MaximumLength : 120;
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
                        : this.GenerateTextBox(description, (string)field.GetValue(this.Result), this.GetFieldTextMaxLength(field));
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
                case "Selector":
                    control = this.GenerateComboBox(description, (Selector)field.GetValue(this.Result));
                    this.Container.Children.Add(label);
                    break;
                case "Color":
                    control = this.GenerateColorBox(description, (Color)field.GetValue(this.Result));
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
        #region Updates
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void NumericBox_OnNumberChanged(object sender)
        {

        }
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void CheckBox_OnChanged(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region Generate
        private Control GenerateTextBox(string description, string value, int maxlength)
        {
            TextBox control = new()
            {
                Tag = description,
                Text = value,
                MaxLength = maxlength,
                Style = maxlength > 120 ? this.Container.FindResource("TextBoxBig") as Style : this.Container.FindResource("TextBoxMain") as Style
            };

            control.TextChanged += this.TextBox_TextChanged;
            return control;
        }

        private void Mi_Click(object sender, RoutedEventArgs e)
        {
            string text = DialogsManager.ShowClipboardManager(true);
            string tag = ((MenuItem)sender).Tag.ToString();
            foreach (Control c in this.Container.Children)
            {
                if (c.Tag.ToString() == tag)
                {
                    ((TextBox)c).Text = text;
                    break;
                }
            }
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
        private Control GenerateComboBox(string description, Selector selector)
        {
            //if (selector.SelectedValue is not null)
            //{
            //    DialogsManager.ShowInfoDialog(selector.SelectedValue);
            //}
            ComboBox control = new()
            {
                Tag = description,
                ItemsSource = selector.VisibleItems,
                SelectedValue = selector.SelectedValue,
                Style = this.Container.FindResource("ComboBoxMain") as Style
            };
            //DialogsManager.ShowInfoDialog(selector.SelectedObject);
            control.SelectionChanged += this.ComboBox_SelectionChanged;
            return control;
        }

        private Control GenerateNumericBox(string description, int value)
        {
            NumericBox control = new()
            {
                Tag = description,
                Value = value,
            };
            control.OnNumberChanged += this.NumericBox_OnNumberChanged;
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
            control.SelectedDateChanged += this.DatePicker_SelectedDateChanged;
            return control;
        }

        private Control GenerateDataGrid(string description, DataTable value)
        {
            DataGridWithButtons control = new(value)
            {
                Tag = description,
                MinHeight = 80
            };
            return control;
        }
        private Control GenerateDataGrid(string description, List<string> value)
        {
            DataTable dt = new();
            dt.Columns.Add("Значение");
            if (value is not null)
            {
                foreach (string item in value)
                {
                    dt.Rows.Add(item);
                }
            }
            DataGridWithButtons control = new(dt)
            {
                Tag = description,
                MinHeight = 80
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
            DataGridWithButtons control = new(dt)
            {
                Tag = description,
                MinHeight = 80
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
            control.Checked += this.CheckBox_OnChanged;
            control.Unchecked += this.CheckBox_OnChanged;
            return control;
        }
        private Control GenerateColorBox(string description, Color value)
        {
            ColorBox control = new()
            {
                Tag = description,
                SelectedColor = value,
            };
            return control;
        }
        #endregion
        private void GetEnumDescriptions(FieldInfo f)
        {
            string GetEnumDescription(Enum enumValue)
            {
                FieldInfo field = enumValue.GetType().GetField(enumValue.ToString());
                return Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute
                    ? attribute.Description
                    : "";
            }
            Array values = Enum.GetValues(f.ReflectedType);

            foreach (Enum value in values)
            {
                GetEnumDescription(value);
            }
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
                                if (string.IsNullOrEmpty(resultString) && field.GetCustomAttribute(typeof(CanBeNull), false) is null)
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
                                field.SetValue(this.Result, ((DataGridWithButtons)control).DataTable);
                                break;
                            case "Selector":
                                if (((ComboBox)control).SelectedValue is null)
                                {
                                    DialogsManager.ShowExclamationDialog("Одно из полей не заполнено.", "Сохранение прервано");
                                    return false;
                                }
                                ((Selector)field.GetValue(this.Result)).SetSelectionByIndex(((ComboBox)control).SelectedIndex);
                                break;
                            case "Color":
                                field.SetValue(this.Result, ((ColorBox)control).SelectedColor);
                                break;
                            default:
                                if (field.PropertyType.Name.Contains("List"))
                                {
                                    DataTable dt = ((DataGridWithButtons)control).DataTable;
                                    List<string> values = [];
                                    foreach (DataRow row in dt.Rows)
                                    {
                                        values.Add(row[0].ToString());
                                    }
                                    field.SetValue(this.Result, values);
                                }
                                else if (field.PropertyType.Name.Contains("Dictionary"))
                                {
                                    DataTable dt = ((DataGridWithButtons)control).DataTable;
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
            try
            {
                MethodInfo method = this.Result.GetType().GetMethod("Validate");
                if (method is not null)
                {
                    method.Invoke(this.Result, null);
                }
            }
            catch (Exception ex)
            {
                DialogsManager.ShowExclamationDialog(ex.InnerException.Message, "Сохранение прервано");
                return false;
            }
            this.SafetyCallMethod("Save");
            return true;
        }
    }
}
