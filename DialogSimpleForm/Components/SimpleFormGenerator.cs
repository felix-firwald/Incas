using Incas.Core.Attributes;
using Incas.Core.AutoUI;
using Incas.Core.Classes;
using Incas.Core.Views.Controls;
using Incas.DialogSimpleForm.Attributes;
using Incas.DialogSimpleForm.Views.Controls;
using IncasEngine.AdditionalFunctionality;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace Incas.DialogSimpleForm.Components
{
    internal class SimpleFormGenerator
    {
        /// <summary>
        /// Container where the generated controls will be placed
        /// </summary>
        public StackPanel Container;

        /// <summary>
        /// Target object
        /// </summary>
        public StaticAutoUIBase Result;

        /// <summary>
        /// Target object
        /// </summary>
        public DynamicAutoUIForm DynamicResult;

        private ResourceStyleManager resourceInstance = new();
        public SimpleFormGenerator(StaticAutoUIBase values, StackPanel container)
        {
            this.Result = values;
            this.SafetyCallMethod("Load");
            this.Container = container;
            foreach (PropertyInfo field in values.GetType().GetProperties())
            {
                this.AddField(field);
            }
        }
        public SimpleFormGenerator(DynamicAutoUIForm form, StackPanel container)
        {
            this.DynamicResult = form;
            this.Container = container;
            foreach (DynamicAutoUIMember field in form.GetAllMembers())
            {
                this.AddField(field);
            }
        }
        /// <summary>
        /// Looking for DescriptionAttribute (Description) for set visible name of control
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private string GetFieldDescription(PropertyInfo field)
        {
            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>(true);
            return attribute != null ? attribute.Description : "";
        }
        /// <summary>
        /// Looking for StringLengthAttribute (StringLength) for set max length of TextBox control
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private int GetFieldTextMaxLength(PropertyInfo field)
        {
            StringLengthAttribute attribute = field.GetCustomAttribute<StringLengthAttribute>(true);
            return attribute != null ? attribute.MaximumLength : 120;
        }
        /// <summary>
        /// Calls the method by its name if it exists in the class
        /// </summary>
        /// <param name="name"></param>
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
        /// <summary>
        /// Gets a class field and turns it into a control
        /// </summary>
        /// <param name="field"></param>
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
                    control = this.SwitchOnAttribute(field, description);
                    this.Container.Children.Add(label);
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
                case "CheckedList":
                    control = this.GenerateCheckedListBox(description, (CheckedList)field.GetValue(this.Result));
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
            if (field.SetMethod is null || field.SetMethod.IsPrivate)
            {
                control.IsEnabled = false;
            }
            this.Container.Children.Add(control);
        }

        /// <summary>
        /// Gets a dynamic member and turns it into a control
        /// </summary>
        /// <param name="field"></param>
        private void AddField(DynamicAutoUIMember field)
        {
            string description = field.Description;
            Control control = new();
            Label label = new()
            {
                Content = field.VisibleName + ":",
                FontSize = 11,
                Style = this.Container.FindResource("LabelPrimary") as Style
            };
            switch (field.Type)
            {
                case DynamicAutoUIMemberType.String:
                    control = this.GenerateTextBox(description, (string)this.DynamicResult[field], 120);
                    this.Container.Children.Add(label);
                    break;
                case DynamicAutoUIMemberType.Text:
                    control = this.GenerateTextBox(description, (string)this.DynamicResult[field], 1200);
                    this.Container.Children.Add(label);
                    break;
                case DynamicAutoUIMemberType.Int:
                    control = this.GenerateNumericBox(description, (int)this.DynamicResult[field]);
                    this.Container.Children.Add(label);
                    break;
                case DynamicAutoUIMemberType.Bool:
                    control = this.GenerateCheckBox(description, (bool)this.DynamicResult[field]);
                    break;
                case DynamicAutoUIMemberType.DateTime:
                    control = this.GenerateDateBox(description, (DateTime)this.DynamicResult[field]);
                    this.Container.Children.Add(label);
                    break;
                //case "DataTable":
                //    control = this.GenerateDataGrid(description, (DataTable)this.DynamicResult[field]);
                //    this.Container.Children.Add(label);
                //    break;
                case DynamicAutoUIMemberType.Enumeration:
                    Selector s = new(field.Values);
                    s.SetSelection(this.DynamicResult[field]);
                    control = this.GenerateComboBox(description, s);
                    this.Container.Children.Add(label);
                    break;
                default:
                    return;
            }
            control.Uid = field.Name;
            if (field.IsReadOnly)
            {
                control.IsEnabled = false;
            }
            this.Container.Children.Add(control);
        }

        private Control SwitchOnAttribute(PropertyInfo field, string description)
        {
            Attribute attrUrl = field.GetCustomAttribute(typeof(UrlRequired), false);
            Attribute attrPwd = field.GetCustomAttribute(typeof(PasswordAttribute), false);
            if (attrUrl != null)
            {
                return this.GeneratePathBox(description, (string)field.GetValue(this.Result));
            }
            if (attrPwd != null)
            {
                return this.GeneratePasswordBox(description, (string)field.GetValue(this.Result), 16);
            }
            return this.GenerateTextBox(description, (string)field.GetValue(this.Result), this.GetFieldTextMaxLength(field));
        }
        #region Updates
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void Numeric_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
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
        private Control GeneratePasswordBox(string description, string value, int maxlength)
        {
            PasswordBox control = new()
            {
                Tag = description,
                Password = value,
                MaxLength = maxlength
            };
            return control;
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
            if (selector is null)
            {
                DialogsManager.ShowExclamationDialog("При отрисовке combobox возникла ошибка: selector был null", "Ошибка отрисовки");
                return new ComboBox();
            }
            ComboBox control = new()
            {
                Tag = description,
                ItemsSource = selector.VisibleItems,
                SelectedValue = selector.SelectedValue,
                Style = this.Container.FindResource("ComboBoxMain") as Style
            };
            control.SelectionChanged += this.ComboBox_SelectionChanged;
            return control;
        }

        private Control GenerateNumericBox(string description, int value)
        {
            IntegerUpDown numeric = new()
            {
                Style = this.resourceInstance.FindResource("NumericUpDown") as Style,
                Watermark = description,
                Value = value,
                ToolTip = description
            };
            numeric.ValueChanged += this.Numeric_ValueChanged;
            return numeric;
        }        

        private Control GenerateDateBox(string description, DateTime value)
        {
            //if (value == DateTime.MinValue)
            //{
            //    value = DateTime.Today;
            //}
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
                IsChecked = value
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
        private Control GenerateCheckedListBox(string description, CheckedList value)
        {
            CheckedListBox control = new(value)
            {
                Tag = description
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
        /// <summary>
        /// Save Form and returns boolean result of this operation (if validation failed it returns false)
        /// </summary>
        /// <returns></returns>
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
                                Attribute attrUrl = field.GetCustomAttribute(typeof(UrlRequired), false);
                                Attribute attrPwd = field.GetCustomAttribute(typeof(PasswordAttribute), false);
                                if (attrUrl != null)
                                {
                                    resultString = ((PathSelector)control).Value;
                                }
                                else if (attrPwd != null)
                                {
                                    resultString = ((PasswordBox)control).Password;
                                }
                                else
                                {
                                    resultString = ((TextBox)control).Text;
                                }
                                
                                if (string.IsNullOrEmpty(resultString) && field.GetCustomAttribute(typeof(CanBeNull), false) is null)
                                {
                                    DialogsManager.ShowExclamationDialog($"Поле \"{descript}\" не заполнено!", "Сохранение прервано");
                                    return false;
                                }
                                if (field.SetMethod is not null)
                                {
                                    field.SetValue(this.Result, resultString);
                                }                             
                                break;
                            case "Int32":
                                field.SetValue(this.Result, ((IntegerUpDown)control).Value);
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
                            case "CheckedList":
                                field.SetValue(this.Result, ((CheckedListBox)control).GetResult());
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
        public bool SaveDynamic()
        {
            this.DynamicResult.Result = false;
            try
            {
                foreach (DynamicAutoUIMember field in this.DynamicResult.GetAllMembers())
                {
                    foreach (Control control in this.Container.Children)
                    {
                        string descript = field.VisibleName;
                        if (control.Uid == field.Name)
                        {
                            switch (field.Type)
                            {
                                case DynamicAutoUIMemberType.String:
                                case DynamicAutoUIMemberType.Text:
                                    string resultString = "";
                                    resultString = ((TextBox)control).Text;
                                    if (string.IsNullOrEmpty(resultString) && field.IsCanBeNull == false)
                                    {
                                        DialogsManager.ShowExclamationDialog($"Поле \"{descript}\" не заполнено!", "Сохранение прервано");
                                        return false;
                                    }
                                    if (!field.IsReadOnly)
                                    {
                                        this.DynamicResult[field.Name] = resultString;
                                    }
                                    break;
                                case DynamicAutoUIMemberType.Int:
                                    this.DynamicResult[field.Name] = ((IntegerUpDown)control).Value;
                                    break;
                                case DynamicAutoUIMemberType.Bool:
                                    this.DynamicResult[field.Name] = (bool)((CheckBox)control).IsChecked;
                                    break;
                                case DynamicAutoUIMemberType.DateTime:
                                    this.DynamicResult[field.Name] = ((DatePicker)control).SelectedDate;
                                    break;
                                //case "DataTable":
                                //    field.SetValue(this.Result, ((DataGridWithButtons)control).DataTable);
                                //    break;
                                case DynamicAutoUIMemberType.Enumeration:
                                    if (((ComboBox)control).SelectedValue is null)
                                    {
                                        DialogsManager.ShowExclamationDialog("Одно из полей не заполнено.", "Сохранение прервано");
                                        return false;
                                    }
                                    int index = ((ComboBox)control).SelectedIndex;
                                    this.DynamicResult[field.Name] = field.Values.Keys.ToList()[index];
                                    break;
                                default:
                                    break;
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DialogsManager.ShowExclamationDialog(ex.InnerException.Message, "Сохранение прервано");
                return false;
            }
            this.DynamicResult.Result = true;
            return true;
        }
    }
}
