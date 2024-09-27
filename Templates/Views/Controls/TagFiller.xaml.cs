using Incas.Core.Classes;
using Incas.Core.Views.Controls;
using Incas.Objects.Components;
using Incas.Templates.Components;
using Incas.Users.Models;
using Microsoft.Scripting.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Incas.Templates.Views.Controls
{
    public static class CommandIcons
    {
        public static Geometry ConvertFromString(string input)
        {
            return (Geometry)TypeDescriptor.GetConverter(typeof(Geometry)).ConvertFrom(input);
        }
    }
    /// <summary>
    /// Логика взаимодействия для TagFiller.xaml
    /// </summary>
    public partial class TagFiller : UserControl
    {
        //public static Dictionary<TagType, >
        public readonly Objects.Models.Field field;

        private bool isRequired = false;
        private bool validated = true;
        private Control control;
        private CommandSettings command;
        public delegate void StringAction(Guid tag, string text);
        public event StringAction OnInsert;
        public delegate void CommandScript(string script);
        public event CommandScript OnScriptRequested;
        public delegate void StringActionRecalculate(string tag);
        public event StringActionRecalculate OnRename;
        public delegate void TagAction(TagFiller sender);
        public event TagAction OnFieldUpdate;

        public TagFiller(Objects.Models.Field f)
        {
            this.InitializeComponent();
            this.field = f;
            this.MainLabel.Text = string.IsNullOrWhiteSpace(this.field.VisibleName) ? this.field.Name + ":" : this.field.VisibleName + ":";
            try
            {
                this.GenerateUIControl(this.field.Type);
                //this.MakeButton();
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
            }
        }
        private TagType GetFillerType()
        {
            return this.field.Type;
        }
        private void GenerateUIControl(TagType type)
        {
            string value = this.field.Value;
            string description = this.field.Description;
            switch (type)
            {
                case TagType.Variable:
                case TagType.Text:
                    TextBox textBox = new()
                    {
                        Text = value,
                        Tag = description
                    };
                    textBox.TextChanged += this.Textbox_TextChanged;
                    if (type == TagType.Variable)
                    {
                        textBox.Style = this.FindResource("TextBoxMain") as Style;
                        textBox.MaxLength = 120;
                    }
                    else
                    {
                        textBox.Style = this.FindResource("TextBoxBig") as System.Windows.Style;
                        textBox.MaxLength = 1200;
                    }
                    this.PlaceUIControl(textBox);
                    break;
                case TagType.LocalEnumeration:
                case TagType.GlobalEnumeration:
                    if (value is null)
                    {
                        value = "";
                    }
                    ComboBox comboBox = new()
                    {                      
                        ItemsSource = type == TagType.LocalEnumeration? JsonConvert.DeserializeObject<List<string>>(value) : ProgramState.GetEnumeration(Guid.Parse(value)),
                        SelectedIndex = 0,
                        Style = this.FindResource("ComboBoxMain") as Style
                    };
                    comboBox.SelectionChanged += this.Combobox_SelectionChanged;
                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        comboBox.ToolTip = description;
                    }
                    this.PlaceUIControl(comboBox);
                    break;
                case TagType.Number:
                    NumericBox numeric = new();
                    numeric.OnNumberChanged += this.NumericBox_OnNumberChanged;
                    numeric.ApplyMinAndMax(Objects.Components.NumberFieldData.GetFromString(value));
                    this.PlaceUIControl(numeric);
                    break;
                case TagType.LocalConstant:              
                case TagType.HiddenField:
                    this.Visibility = Visibility.Collapsed;
                    break;
                case TagType.GlobalConstant:
                    this.Visibility = Visibility.Collapsed;
                    this.field.Value = ProgramState.GetConstant(value);           
                    break;
                case TagType.Relation:
                    SelectionBox selectionBox = new(BindingData.GetFromString(value));
                    selectionBox.OnValueChanged += this.SelectionBox_OnValueChanged;
                    this.PlaceUIControl(selectionBox);
                    break;
                case TagType.Date:
                    DatePicker picker = new()
                    {
                        ToolTip = description,
                        Style = this.FindResource("DatePickerMain") as Style
                    };
                    picker.SelectedDateChanged += this.DatePicker_SelectedDateChanged;
                    this.PlaceUIControl(picker);
                    break;
                case TagType.Generator:
                case TagType.Macrogenerator:
                    Guid num = Guid.Empty;
                    Guid.TryParse(value, out num);
                    Generator generator = new(type)
                    {
                        TemplateId = num
                    };
                    generator.OnValueChanged += this.Generator_OnValueChanged;
                    this.PlaceUIControl(generator);
                    break;
            }
        }

        private void PlaceUIControl(Control control)
        {
            this.control = control;
            this.Grid.Children.Add(control);
            Grid.SetRow(control, 0);
            Grid.SetColumn(control, 1);
        }

        private void MakeButton()
        {
            this.command = this.field.GetCommand();
            if (this.command.ScriptType != ScriptType.Button)
            {
                return;
            }
            this.CommandButtonIcon.Data = this.FindResource(this.command.Icon.ToString()) as PathGeometry;
            this.CommandButton.Visibility = Visibility.Visible;
            this.CommandButtonText.Content = this.command.Name;
        }
        public void MarkAsNotValidated()
        {
            this.validated = false;
            this.MainLabel.Foreground = this.FindResource("Error") as SolidColorBrush;
        }

        public void SetValue(string value)
        {
            switch (this.GetFillerType())
            {
                case TagType.Variable:
                case TagType.Text:
                default:
                    ((TextBox)this.control).Text = value;
                    break;
                case TagType.Number:
                    int digitValue;
                    if (int.TryParse(value, out digitValue))
                    {
                        ((NumericBox)this.control).Value = digitValue;
                    }
                    break;
                case TagType.Relation:
                    ((SelectionBox)this.control).SetObject(value);
                    break;
                case TagType.LocalConstant:
                case TagType.GlobalConstant:
                    return;
                case TagType.HiddenField:
                    this.field.Value = value;
                    break;
                case TagType.LocalEnumeration:
                case TagType.GlobalEnumeration:
                    ((ComboBox)this.control).SelectedValue = value;
                    break;
                case TagType.Date:
                    this.SetDateTimeValue(value);
                    break;
                case TagType.Generator:
                case TagType.Macrogenerator:
                    ((Generator)this.control).SetData(value);
                    break;
            }
        }

        private void SetDateTimeValue(string value)
        {
            List<string> formats = new()
            {
                "dd.MM.yyyy", "dd.MM.yyyy HH:mm:ss", "dd.MM.yy"
            };
            bool tryApply(string format)
            {
                DateTime parsedDate;
                bool success = DateTime.TryParseExact(value, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out parsedDate);
                if (success)
                {
                    ((DatePicker)this.control).SelectedDate = parsedDate;
                }
                return success;
            }
            foreach (string format in formats)
            {
                if (tryApply(format))
                {
                    return;
                }
            }            
        }

        public string GetTagName()
        {
            return this.field.Name;
        }
        public bool ValidateContent()
        {
            if (this.field.Type is TagType.Variable or TagType.Text)
            {
                if (this.isRequired && string.IsNullOrEmpty(((TextBox)this.control).Text))
                {
                    DialogsManager.ShowExclamationDialog($"Поле \"{this.field.Name}\" обязательно для заполнения!", "Действие прервано");
                    return false;
                }
            }
            return true;
        }
        private string GetDateInFormat()
        {
            DatePicker picker = ((DatePicker)this.control);
            if (picker.SelectedDate is null)
            {
                return "";
            }
            return string.IsNullOrWhiteSpace(this.field.Value)
                ? ((DateTime)picker.SelectedDate).ToString("dd.MM.yyyy")
                : picker.SelectedDate.HasValue ? ((DateTime)picker.SelectedDate).ToString(this.field.Value) : "";
        }
        public string GetValue()
        {
            switch (this.GetFillerType())
            {
                case TagType.Variable:
                default:
                    return ((TextBox)this.control).Text;
                case TagType.Number:
                    return ((NumericBox)this.control).Value.ToString();
                case TagType.LocalConstant:
                case TagType.HiddenField:
                case TagType.GlobalConstant:
                    return this.field.Value;
                case TagType.Relation:
                    return ((SelectionBox)this.control).Value;
                case TagType.LocalEnumeration:
                case TagType.GlobalEnumeration:
                    ComboBox cb = (ComboBox)this.control;
                    if (cb.SelectedIndex != -1)
                    {
                        return cb.Items.GetItemAt(cb.SelectedIndex).ToString();
                    }
                    return "";
                case TagType.Date:
                    return this.GetDateInFormat();
                case TagType.Generator:
                case TagType.Macrogenerator:
                    return ((Generator)this.control).GetText();
            }
        }
        public string GetData()
        {
            switch (this.GetFillerType())
            {
                case TagType.Generator:
                case TagType.Macrogenerator:
                    return ((Generator)this.control).GetData();
                case TagType.Date:
                    if (((DatePicker)this.control).SelectedDate.HasValue)
                    {
                        return ((DateTime)((DatePicker)this.control).SelectedDate).ToString("dd.MM.yyyy");
                    }
                    return "";
                case TagType.Relation:
                    return ((SelectionBox)this.control).SelectedObject?.Id.ToString();
                default: return this.GetValue();

            }
        }

        public Guid GetId()
        {
            return this.field.Id;
        }

        private void CopyAllClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(this.GetValue());
        }

        private void InsertToOther(object sender, RoutedEventArgs e)
        {
            OnInsert?.Invoke(this.field.Id, this.GetData());
        }
        private void RecalculateNamesClick(object sender, RoutedEventArgs e)
        {
            OnRename?.Invoke(this.field.Name);
        }

        private void TextRequest(object sender, RoutedEventArgs e)
        {

        }
        private void PlayScript()
        {
            try
            {
                if (this.command.Script.Contains("# [affects other]"))
                {
                    OnScriptRequested?.Invoke(this.command.Script);
                }
                else
                {
                    ScriptScope scope = ScriptManager.GetEngine().CreateScope();
                    scope.SetVariable("input_data", this.GetValue());
                    ScriptManager.Execute(this.command.Script, scope);

                    this.SetValue((string)scope.GetVariable("output"));
                }
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog("При обработке скрипта произошла ошибка:\n" + ex.Message);
            }
        }
        private void CommandClick(object sender, RoutedEventArgs e)
        {
            this.PlayScript();
        }
        private void CheckForScriptOnUpdate()
        {
            if (this.command.ScriptType == ScriptType.ValueChanged)
            {
                this.PlayScript();
            }
            if (!this.validated)
            {
                this.validated = true;
                this.MainLabel.Foreground = this.FindResource("GrayLight") as SolidColorBrush;
            }
        }
        private void RunUpdateEvent()
        {
            this.OnFieldUpdate?.Invoke(this);
        }

        private void Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.RunUpdateEvent();
            this.CheckForScriptOnUpdate();          
        }

        private void Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.RunUpdateEvent();
            this.CheckForScriptOnUpdate();
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            this.RunUpdateEvent();
            this.CheckForScriptOnUpdate();
        }

        private void SelectionBox_OnValueChanged(object sender, TextChangedEventArgs e)
        {
            this.RunUpdateEvent();
            this.CheckForScriptOnUpdate();
        }

        private void Generator_OnValueChanged(object sender)
        {
            this.RunUpdateEvent();
            this.CheckForScriptOnUpdate();
        }

        private void NumericBox_OnNumberChanged(object sender)
        {
            this.RunUpdateEvent();
            this.CheckForScriptOnUpdate();
        }

        private void CopyFromIncasClipboard(object sender, RoutedEventArgs e)
        {
            string value = DialogsManager.ShowClipboardManager(true);
            if (!string.IsNullOrEmpty(value))
            {
                this.SetValue(value);
            }          
        }
    }
}
