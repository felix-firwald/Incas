using Common;
using DocumentFormat.OpenXml.Wordprocessing;
using Incubator_2.Common;
using Irony.Parsing;
using Microsoft.Scripting.Hosting;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tag = Models.Tag;


namespace Incubator_2.Forms
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
    /// Логика взаимодействия для UC_TagFiller.xaml
    /// </summary>
    public partial class UC_TagFiller : UserControl
    {
        public readonly Tag tag;
        public FillerMode Mode;
        private bool isRequired = false;
        private bool validated = true;
        private CommandSettings command;
        public delegate void StringAction(int tag, string text);
        public event StringAction OnInsert;
        public delegate void CommandScript(string script);
        public event CommandScript OnScriptRequested;
        public delegate void StringActionRecalculate(string tag);
        public event StringActionRecalculate OnRename;
        public UC_TagFiller(Tag t)
        {
            InitializeComponent();
            Mode = FillerMode.Tag;
            this.tag = t;
            this.MainLabel.Text = this.tag.name + ":";
            //this.CommandIcon.Data = FindResource("Regex") as Geometry;
            switch (tag.type)
            {
                case TypeOfTag.Variable:
                default:
                    SetTextBoxMode();
                    this.Textbox.Text = this.tag.value;
                    this.Textbox.MaxLength = 120;
                    this.Textbox.Tag = this.tag.description;
                    break;
                case TypeOfTag.Text:
                    SetTextBoxMode();
                    this.Textbox.Text = this.tag.value;
                    this.Textbox.Style = FindResource("TextBoxBig") as System.Windows.Style;
                    this.Textbox.MaxLength = 1200;
                    this.Textbox.Tag = this.tag.description;
                    break;
                case TypeOfTag.LocalConstant:
                case TypeOfTag.HiddenField:
                    this.Textbox.Text = this.tag.value;
                    this.Visibility = Visibility.Collapsed;
                    break;
                case TypeOfTag.LocalEnumeration:
                    SetComboBoxMode();
                    this.Combobox.ItemsSource = this.tag.value.Split(';');
                    this.Combobox.SelectedIndex = 0;
                    if (!string.IsNullOrWhiteSpace(this.tag.description))
                    {
                        this.Combobox.ToolTip = this.tag.description;
                    }
                    break;
                case TypeOfTag.Relation:
                    this.SelectionBox.Visibility = Visibility.Visible;
                    this.SelectionBox.Source = this.tag.value;
                    this.SelectionBox.ToolTip = this.tag.description;
                    break;
                case TypeOfTag.Date:
                    this.DatePicker.Visibility = Visibility.Visible;
                    this.DatePicker.ToolTip = this.tag.description;
                    break;
                case TypeOfTag.Generator:
                    this.Generator.Visibility = Visibility.Visible;
                    this.Generator.TemplateId = int.Parse(this.tag.value);
                    break;
            }
            MakeButton();
        }
        private void MakeButton()
        {
            command = tag.GetCommand();
            if (command.ScriptType != ScriptType.Button)
            {
                return;
            }
            this.CommandButtonIcon.Data = FindResource(command.Icon.ToString()) as PathGeometry;
            this.CommandButton.Visibility = Visibility.Visible;
            this.CommandButtonText.Content = command.Name;
        }
        public void MarkAsNotValidated()
        {
            validated = false;
            this.MainLabel.Foreground = FindResource("Error") as SolidColorBrush;
        }
        public UC_TagFiller(FieldCreator fc, string path)
        {
            InitializeComponent();
            this.tag = new();
            Mode = FillerMode.RecordField;
            tag.name = fc.Name;
            this.MainLabel.Text = tag.name + ":";
            isRequired = fc.NotNULL;
            if (fc.FKtable != null)
            {
                tag.type = TypeOfTag.Relation;
                this.SelectionBox.Visibility = Visibility.Visible;
                this.SelectionBox.Database = path;
                this.SelectionBox.Table = fc.FKtable;
                this.SelectionBox.Field = fc.FKfield;
            }
            else
            {
                tag.type = TypeOfTag.Text;
                SetTextBoxMode();
            }
        }

        public void SetValue(string value)
        {
            switch (tag.type)
            {
                case TypeOfTag.Variable:
                case TypeOfTag.Text:
                case TypeOfTag.HiddenField:
                default:
                    this.Textbox.Text = value;
                    break;
                case TypeOfTag.Relation:
                    this.SelectionBox.Value = value;
                    break;
                case TypeOfTag.LocalConstant:
                    return;
                case TypeOfTag.LocalEnumeration:
                    this.Combobox.SelectedValue = value;
                    break;
                case TypeOfTag.Date:
                    DateTime parsedDate;
                    bool success = DateTime.TryParseExact(value, "dd.MM.yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out parsedDate);
                    if (success)
                    {
                        this.DatePicker.SelectedDate = parsedDate;
                    }
                    break;
                case TypeOfTag.Generator:
                    this.Generator.SetData(value);
                    break;
            }
        }

        private void SetTextBoxMode()
        {
            this.Textbox.Visibility = Visibility.Visible;
        }
        private void SetComboBoxMode()
        {
            this.Combobox.Visibility = Visibility.Visible;
        }
        public string GetTagName()
        {
            return this.tag.name;
        }
        public bool ValidateContent()
        {
            if (tag.type == TypeOfTag.Variable || tag.type == TypeOfTag.Text)
            {
                if (isRequired && string.IsNullOrEmpty(this.Textbox.Text))
                {
                    ProgramState.ShowExclamationDialog($"Поле \"{tag.name}\" обязательно для заполнения!", "Действие прервано");
                    return false;
                }
            }
            return true;
        }
        private string GetDateInFormat()
        {
            if (string.IsNullOrWhiteSpace(this.tag.value))
            {
                return this.DatePicker.SelectedDate.ToString();
            }
            if (this.DatePicker.SelectedDate.HasValue)
            {
                return ((DateTime)this.DatePicker.SelectedDate).ToString(this.tag.value);
            }
            return "";
        }
        public string GetValue()
        {
            
            switch (tag.type)
            {
                case TypeOfTag.Variable:
                default:
                    return this.Textbox.Text;
                case TypeOfTag.LocalConstant:
                    return this.tag.value;
                case TypeOfTag.Relation:
                    return this.SelectionBox.Value;
                case TypeOfTag.LocalEnumeration:
                    if (this.Combobox.SelectedIndex != -1)
                    {
                        return this.Combobox.Items.GetItemAt(this.Combobox.SelectedIndex).ToString();
                    }
                    return "";
                case TypeOfTag.Date:
                    return GetDateInFormat();
                case TypeOfTag.Generator:
                    return this.Generator.GetText();
            }
        }
        public string GetData()
        {
            switch (tag.type)
            {    
                case TypeOfTag.Generator:
                    return this.Generator.GetData();
                case TypeOfTag.Date:
                    if (this.DatePicker.SelectedDate.HasValue)
                    {
                        return ((DateTime)this.DatePicker.SelectedDate).ToString("dd.MM.yyyy");
                    }
                    return "";
                default: return GetValue();

            }
        }
        public int GetId()
        {
            return this.tag.id;
        }
        private void ClearClick(object sender, RoutedEventArgs e)
        {
            this.Textbox.Text = "";
        }

        private void CopyClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(this.Textbox.SelectedText);
        }
        private void CopyAllClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(this.Textbox.Text);
        }
        private void PasteClick(object sender, RoutedEventArgs e)
        {
            this.Textbox.Text = Clipboard.GetText();
        }

        private bool IsAnythingSelected()
        {
            return !string.IsNullOrEmpty(this.Textbox.SelectedText);
        }
        private void MakeTitleClick(object sender, RoutedEventArgs e)
        {
            if (IsAnythingSelected())
            {
                this.Textbox.SelectedText = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(this.Textbox.SelectedText.ToLower());
            }
            else
            {
                this.Textbox.Text = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(this.Textbox.Text.ToLower());
            }
        }
        private void MakeUpperClick(object sender, RoutedEventArgs e)
        {
            if (IsAnythingSelected())
            {
                this.Textbox.SelectedText = this.Textbox.SelectedText.ToUpper();
            }
            else
            {
                this.Textbox.Text = this.Textbox.Text.ToUpper();
            }
        }
        private void MakeLowerClick(object sender, RoutedEventArgs e)
        {
            if (IsAnythingSelected())
            {
                this.Textbox.SelectedText = this.Textbox.SelectedText.ToLower();
            }
            else
            {
                this.Textbox.Text = this.Textbox.Text.ToLower();
            }
            
        }
        private void RemoveWhitespacesClick(object sender, RoutedEventArgs e)
        {
            this.Textbox.Text = Regex.Replace(this.Textbox.Text, @"\s+", " ");
        }
        private void RemoveLineBreaksClick(object sender, RoutedEventArgs e)
        {
            this.Textbox.Text = this.Textbox.Text.Replace(System.Environment.NewLine, " ");
        }
        private void WrapAsQuoteClick(object sender, RoutedEventArgs e)
        {
            if (IsAnythingSelected())
            {
                this.Textbox.SelectedText = $"«{this.Textbox.SelectedText}»";
            }
            else
            {
                this.Textbox.Text = $"«{this.Textbox.Text}»";
            }
        }
        private void AddDateNow(object sender, RoutedEventArgs e)
        {
            this.Textbox.Text += DateTime.Now.ToString("dd.MM.yyyy");
        }
        private void AddDateLongNow(object sender, RoutedEventArgs e)
        {
            this.Textbox.Text += DateTime.Now.ToString("D");
        }

        private void InsertToOther(object sender, RoutedEventArgs e)
        {
            OnInsert?.Invoke(this.tag.id, this.Textbox.Text);
        }
        private void RecalculateNamesClick(object sender, RoutedEventArgs e)
        {
            OnRename?.Invoke(this.tag.name);
        }

        private void TextRequest(object sender, RoutedEventArgs e)
        {
            string rec = ProgramState.ShowActiveUserSelector("Выберите пользователя для запроса данных").slug;
            ServerProcessor.SendRequestTextProcess(this, rec);
        }
        private void PlayScript()
        {
            try
            {
                if (command.Script.Contains("# [affects other]"))
                {
                    OnScriptRequested?.Invoke(command.Script);
                }
                else
                {
                    ScriptScope scope = ScriptManager.GetEngine().CreateScope();
                    scope.SetVariable("input_data", this.GetValue());
                    ScriptManager.Execute(command.Script, scope);

                    this.SetValue((string)scope.GetVariable("output"));
                }

            }
            catch (Exception ex)
            {

                ProgramState.ShowErrorDialog("При обработке скрипта произошла ошибка:\n" + ex.Message);
            }
        }
        private void CommandClick(object sender, RoutedEventArgs e)
        {
            PlayScript();
        }
        private void CheckForScriptOnUpdate()
        {
            if (command.ScriptType == ScriptType.ValueChanged)
            {
                PlayScript();
            }
            if (!validated)
            {
                validated = true;
                this.MainLabel.Foreground = FindResource("GrayLight") as SolidColorBrush;
            }
        }

        private void Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckForScriptOnUpdate();
        }

        private void Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckForScriptOnUpdate();
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckForScriptOnUpdate();
        }

        private void SelectionBox_OnValueChanged(object sender, TextChangedEventArgs e)
        {
            CheckForScriptOnUpdate();
        }
    }
}
