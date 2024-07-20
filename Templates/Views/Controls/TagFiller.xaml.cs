using Incas.Core.Classes;
using Incas.Core.Models;
using Incas.Core.Views.Controls;
using Incas.Templates.Components;
using Incas.Users.Models;
using Incubator_2.Common;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tag = Incas.Templates.Models.Tag;

namespace Incas.Templates.Views.Controls
{
    public static class CommandIcons
    {
        public static Geometry ConvertFromString(string input)
        {
            return (Geometry)TypeDescriptor.GetConverter(typeof(Geometry)).ConvertFrom(input);
        }
    }
    public enum FillerMode
    {
        Tag,
        RecordField
    }
    /// <summary>
    /// Логика взаимодействия для TagFiller.xaml
    /// </summary>
    public partial class TagFiller : UserControl
    {
        //public static Dictionary<TagType, >
        public readonly Tag tag;
        public FillerMode Mode;
        private bool isRequired = false;
        private bool validated = true;
        private Control control;
        private CommandSettings command;
        public delegate void StringAction(int tag, string text);
        public event StringAction OnInsert;
        public delegate void CommandScript(string script);
        public event CommandScript OnScriptRequested;
        public delegate void StringActionRecalculate(string tag);
        public event StringActionRecalculate OnRename;
        public TagFiller(Tag t)
        {
            this.InitializeComponent();
            this.Mode = FillerMode.Tag;
            this.tag = t;
            this.MainLabel.Text = string.IsNullOrWhiteSpace(this.tag.visibleName) ? this.tag.name + ":" : this.tag.visibleName + ":";
            this.GenerateUIControl();
            this.MakeButton();
        }

        private void GenerateUIControl()
        {
            switch (this.tag.type)
            {
                case TagType.Variable:
                case TagType.Text:
                    TextBox textBox = new()
                    {
                        Text = this.tag.value,
                        Tag = this.tag.description
                    };
                    textBox.TextChanged += this.Textbox_TextChanged;
                    if (this.tag.type == TagType.Variable)
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
                    if (this.tag.value is null)
                    {
                        this.tag.value = "";
                    }
                    ComboBox comboBox = new()
                    {
                        
                        ItemsSource = this.tag.type == TagType.LocalEnumeration? this.tag.value.Split(';') : ProgramState.GetEnumeration(this.tag.value),
                        SelectedIndex = 0,
                        Style = this.FindResource("ComboBoxMain") as Style
                    };
                    comboBox.SelectionChanged += this.Combobox_SelectionChanged;
                    if (!string.IsNullOrWhiteSpace(this.tag.description))
                    {
                        comboBox.ToolTip = this.tag.description;
                    }
                    this.PlaceUIControl(comboBox);
                    break;
                case TagType.Number:
                    NumericBox numeric = new();
                    numeric.OnNumberChanged += this.NumericBox_OnNumberChanged;
                    numeric.ApplyMinAndMax(this.tag.value);
                    this.PlaceUIControl(numeric);
                    break;
                case TagType.LocalConstant:              
                case TagType.HiddenField:
                    this.Visibility = Visibility.Collapsed;
                    break;
                case TagType.GlobalConstant:
                    this.Visibility = Visibility.Collapsed;
                    this.tag.value = ProgramState.GetConstant(this.tag.value);
                    break;
                case TagType.Relation:
                    SelectionBox selectionBox = new()
                    {
                        Source = this.tag.value,
                        ToolTip = this.tag.description
                    };
                    selectionBox.OnValueChanged += this.SelectionBox_OnValueChanged;
                    this.PlaceUIControl(selectionBox);
                    break;
                case TagType.Date:
                    DatePicker picker = new()
                    {
                        ToolTip = this.tag.description,
                        Style = this.FindResource("DatePickerMain") as Style
                    };
                    picker.SelectedDateChanged += this.DatePicker_SelectedDateChanged;
                    this.PlaceUIControl(picker);
                    break;
                case TagType.Generator:
                case TagType.Macrogenerator:
                    int num = 0;
                    int.TryParse(this.tag.value, out num);
                    Generator generator = new(this.tag.type)
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
            this.command = this.tag.GetCommand();
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
        public TagFiller(FieldCreator fc, string path)
        {
            this.InitializeComponent();
            this.tag = new();
            this.Mode = FillerMode.RecordField;
            this.tag.name = fc.Name;
            this.MainLabel.Text = this.tag.name + ":";
            this.isRequired = fc.NotNULL;
            this.tag.type = TagType.Text;           
            if (fc.FKtable != null)
            {
                this.tag.type = TagType.Relation;
            }
            this.GenerateUIControl();
        }

        public void SetValue(string value)
        {
            switch (this.tag.type)
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
                    ((SelectionBox)this.control).Value = value;
                    break;
                case TagType.LocalConstant:
                case TagType.GlobalConstant:
                    return;
                case TagType.HiddenField:
                    this.tag.value = value;
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
            return this.tag.name;
        }
        public bool ValidateContent()
        {
            if (this.tag.type is TagType.Variable or TagType.Text)
            {
                if (this.isRequired && string.IsNullOrEmpty(((TextBox)this.control).Text))
                {
                    DialogsManager.ShowExclamationDialog($"Поле \"{this.tag.name}\" обязательно для заполнения!", "Действие прервано");
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
            return string.IsNullOrWhiteSpace(this.tag.value)
                ? ((DateTime)picker.SelectedDate).ToString("dd.MM.yyyy")
                : picker.SelectedDate.HasValue ? ((DateTime)picker.SelectedDate).ToString(this.tag.value) : "";
        }
        public string GetValue()
        {

            switch (this.tag.type)
            {
                case TagType.Variable:
                default:
                    return ((TextBox)this.control).Text;
                case TagType.Number:
                    return ((NumericBox)this.control).Value.ToString();
                case TagType.LocalConstant:
                case TagType.HiddenField:
                case TagType.GlobalConstant:
                    return this.tag.value;
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
            switch (this.tag.type)
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
                default: return this.GetValue();

            }
        }
        public int GetId()
        {
            return this.tag.id;
        }

        private void CopyAllClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(this.GetValue());
        }
        //private void PasteClick(object sender, RoutedEventArgs e)
        //{
        //    this.Textbox.Text = Clipboard.GetText();
        //}

        //private bool IsAnythingSelected()
        //{
        //    return !string.IsNullOrEmpty(this.Textbox.SelectedText);
        //}
        //private void MakeTitleClick(object sender, RoutedEventArgs e)
        //{
        //    if (this.IsAnythingSelected())
        //    {
        //        this.Textbox.SelectedText = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(this.Textbox.SelectedText.ToLower());
        //    }
        //    else
        //    {
        //        this.Textbox.Text = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(this.Textbox.Text.ToLower());
        //    }
        //}
        //private void MakeUpperClick(object sender, RoutedEventArgs e)
        //{
        //    if (this.IsAnythingSelected())
        //    {
        //        this.Textbox.SelectedText = this.Textbox.SelectedText.ToUpper();
        //    }
        //    else
        //    {
        //        this.Textbox.Text = this.Textbox.Text.ToUpper();
        //    }
        //}
        //private void MakeLowerClick(object sender, RoutedEventArgs e)
        //{
        //    if (this.IsAnythingSelected())
        //    {
        //        this.Textbox.SelectedText = this.Textbox.SelectedText.ToLower();
        //    }
        //    else
        //    {
        //        this.Textbox.Text = this.Textbox.Text.ToLower();
        //    }
        //}
        //private void RemoveWhitespacesClick(object sender, RoutedEventArgs e)
        //{
        //    this.Textbox.Text = Regex.Replace(this.Textbox.Text, @"\s+", " ");
        //}
        //private void RemoveLineBreaksClick(object sender, RoutedEventArgs e)
        //{
        //    this.Textbox.Text = this.Textbox.Text.Replace(System.Environment.NewLine, " ");
        //}
        //private void WrapAsQuoteClick(object sender, RoutedEventArgs e)
        //{
        //    if (this.IsAnythingSelected())
        //    {
        //        this.Textbox.SelectedText = $"«{this.Textbox.SelectedText}»";
        //    }
        //    else
        //    {
        //        this.Textbox.Text = $"«{this.Textbox.Text}»";
        //    }
        //}
        //private void AddDateNow(object sender, RoutedEventArgs e)
        //{
        //    this.Textbox.Text += DateTime.Now.ToString("dd.MM.yyyy");
        //}
        //private void AddDateLongNow(object sender, RoutedEventArgs e)
        //{
        //    this.Textbox.Text += DateTime.Now.ToString("D");
        //}

        private void InsertToOther(object sender, RoutedEventArgs e)
        {
            OnInsert?.Invoke(this.tag.id, this.GetData());
        }
        private void RecalculateNamesClick(object sender, RoutedEventArgs e)
        {
            OnRename?.Invoke(this.tag.name);
        }

        private void TextRequest(object sender, RoutedEventArgs e)
        {
            Session session;
            if (DialogsManager.ShowActiveUserSelector(out session, "Выберите пользователя для запроса данных"))
            {
                ServerProcessor.SendRequestTextProcess(this, session.slug);
            }
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

        private void Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.CheckForScriptOnUpdate();
        }

        private void Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.CheckForScriptOnUpdate();
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            this.CheckForScriptOnUpdate();
        }

        private void SelectionBox_OnValueChanged(object sender, TextChangedEventArgs e)
        {
            this.CheckForScriptOnUpdate();
        }

        private void Generator_OnValueChanged(object sender)
        {
            this.CheckForScriptOnUpdate();
        }

        private void NumericBox_OnNumberChanged(object sender)
        {
            this.CheckForScriptOnUpdate();
        }
    }
}
