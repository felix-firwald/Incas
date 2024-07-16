using Incas.Core.Classes;
using Incas.Core.Views.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Incas.Core.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для DialogSimpleForm.xaml
    /// </summary>
    public partial class DialogSimpleForm : Window
    {
        public object Result;
        public DialogSimpleForm(object values, string title)
        {
            InitializeComponent();
            this.Result = values;
            this.TitleText.Content = title;

            foreach (PropertyInfo field in values.GetType().GetProperties())
            {
                this.AddField(field);
            }
        }
        private void AddField(PropertyInfo field)
        {
            string description = this.GetFieldDescription(field);
            //DialogsManager.ShowInfoDialog(field.FieldType.Name);
            Control control = new();
            Label label = new()
            {
                Content = description + ":",
                FontSize = 11,
                Style = this.FindResource("LabelPrimary") as Style
            };

            //DialogsManager.ShowInfoDialog(field.PropertyType.Name);
            switch (field.PropertyType.Name)
            {
                case "String":
                    control = this.GenerateTextBox(description, (string)field.GetValue(this.Result));
                    break;
                case "Int32":
                    control = this.GenerateNumericBox(description, (int)field.GetValue(this.Result));
                    this.Fields.Children.Add(label);
                    break;
                case "Boolean":
                    control = this.GenerateCheckBox(description, (bool)field.GetValue(this.Result));
                    break;
                case "DateTime":
                    control = this.GenerateDateBox(description, (DateTime)field.GetValue(this.Result));
                    this.Fields.Children.Add(label);
                    break;
                case "DataTable":
                    control = this.GenerateDataGrid(description, (DataTable)field.GetValue(this.Result));
                    this.Fields.Children.Add(label);
                    break;
                default:
                    if (field.PropertyType.Name.Contains("List"))
                    {
                        control = this.GenerateDataGrid(description, (List<string>)field.GetValue(this.Result));
                        this.Fields.Children.Add(label);
                    }
                    else
                    {
                        return;
                    }
                    break;
            }
            //if (field.FieldType.IsEnum)
            //{
            //    DialogsManager.ShowInfoDialog(field.Name);
            //    this.GetEnumDescriptions(field);
            //}
            if (field.SetMethod.IsPrivate)
            {
                control.IsEnabled = false;
            }
            this.Fields.Children.Add(control);
        }
        private Control GenerateTextBox(string description, string value)
        {
            TextBox control = new()
            {
                Tag = description,
                Text = value,
                Style = this.FindResource("TextBoxMain") as Style
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
                Style = this.FindResource("DatePickerMain") as Style
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
                Style = this.FindResource("DataGridMain") as Style
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
                Style = this.FindResource("DataGridMain") as Style
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
                Style = this.FindResource("DataGridMain") as Style
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
                Style = this.FindResource("Toggle") as Style
            };
            return control;
        }

        private void GetEnumDescriptions(FieldInfo f)
        {
            string GetEnumDescription(Enum enumValue)
            {
                var field = enumValue.GetType().GetField(enumValue.ToString());
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    return attribute.Description;
                }
                return "";
            }
            Array values = Enum.GetValues(f.ReflectedType);

            foreach (Enum value in values)
            {
                GetEnumDescription(value);
                //MemberInfo memberInfo = f.GetType().GetMember(value.ToString()).FirstOrDefault();
                //if (memberInfo == null)
                //{
                //    DescriptionAttribute descriptionAttribute = f.GetType().GetMember(f.ToString())[0]
                //        .GetCustomAttributes(typeof(DescriptionAttribute), inherit: false)[0] as DescriptionAttribute;
                //    //DescriptionAttribute attr = memberInfo.GetCustomAttribute<DescriptionAttribute>(false);
                //    DialogsManager.ShowInfoDialog($"{value} - {descriptionAttribute.Description}");
                //}
            }
        }
        private string GetFieldDescription(PropertyInfo field)
        {
            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>(true);
            if (attribute != null)
            {
                return attribute.Description;
            }
            return "";
        }

        private void FinishClick(object sender, RoutedEventArgs e)
        {
            foreach (PropertyInfo field in this.Result.GetType().GetProperties())
            {
                foreach (Control control in this.Fields.Children)
                {
                    string descript = this.GetFieldDescription(field);
                    if (control.Tag?.ToString() == descript)
                    {
                        
                        switch (field.PropertyType.Name)
                        {
                            case "String":
                                string resultString = ((TextBox)control).Text;
                                if (string.IsNullOrEmpty(resultString))
                                {
                                    DialogsManager.ShowExclamationDialog($"Поле \"{descript}\" не заполнено!", "Сохранение прервано");
                                    return;
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
                            default:
                                if (field.PropertyType.Name.Contains("List"))
                                {
                                    DataTable dt = ((DataView)((DataGrid)control).ItemsSource).ToTable();
                                    List<string> values = new();
                                    foreach (DataRow row in dt.Rows)
                                    {
                                        values.Add(row[0].ToString());
                                    }
                                    field.SetValue(this.Result, values);
                                }
                                else if (field.PropertyType.Name.Contains("Dictionary"))
                                {
                                    DataTable dt = ((DataView)((DataGrid)control).ItemsSource).ToTable();
                                    Dictionary<string,string> values = new();
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
            this.DialogResult = true;
            this.Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
