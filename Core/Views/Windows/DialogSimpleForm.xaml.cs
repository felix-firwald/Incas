using DIaLOGIKa.b2xtranslator.Tools;
using Incas.Core.Classes;
using Incas.Core.Views.Controls;
using Microsoft.Scripting.Metadata;


//using Microsoft.Scripting.Utils;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X509.Qualified;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Incas.Core.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для DialogSimpleForm.xaml
    /// </summary>
    public partial class DialogSimpleForm : Window
    {
        public object Result;
        private List<TextBox> textBoxes = [];
        public DialogSimpleForm(object values, string title)
        {
            InitializeComponent();
            this.Result = values;
            this.TitleText.Content = title;

            foreach (FieldInfo field in values.GetType().GetFields())
            {
                this.AddField(field);
            }
        }
        private void AddField(FieldInfo field)
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
            
            switch (field.FieldType.Name)
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
            DatePicker control = new()
            {
                Tag = description,
                SelectedDate = value,
                Style = this.FindResource("DatePickerMain") as Style
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

        private string GetFieldDescription(FieldInfo field)
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
            foreach (FieldInfo field in this.Result.GetType().GetFields())
            {
                foreach (Control control in this.Fields.Children)
                {
                    string descript = this.GetFieldDescription(field);
                    if (control.Tag?.ToString() == descript)
                    {
                        
                        switch (field.FieldType.Name)
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
