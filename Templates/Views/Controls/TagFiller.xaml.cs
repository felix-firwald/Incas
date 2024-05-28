using Incas.Core.Classes;
using Incas.Templates.Components;
using Incas.Users.Models;
using IncasEngine.Database;
using IncasEngine.TemplateManager;
using Incubator_2.Common;
using Microsoft.Scripting.Hosting;
using System;
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
        public TagFiller(Tag t)
        {
            this.InitializeComponent();
            this.Mode = FillerMode.Tag;
            this.tag = t;
            this.MainLabel.Text = string.IsNullOrWhiteSpace(this.tag.visibleName) ? this.tag.name + ":" : this.tag.visibleName + ":";

            switch (this.tag.type)
            {
                case TagType.Variable:
                default:
                    this.SetTextBoxMode();
                    this.Textbox.Text = this.tag.value;
                    this.Textbox.MaxLength = 120;
                    this.Textbox.Tag = this.tag.description;
                    break;
                case TagType.Text:
                    this.SetTextBoxMode();
                    this.Textbox.Text = this.tag.value;
                    this.Textbox.Style = this.FindResource("TextBoxBig") as System.Windows.Style;
                    this.Textbox.MaxLength = 1200;
                    this.Textbox.Tag = this.tag.description;
                    break;
                case TagType.Number:
                    this.NumericBox.ApplyMinAndMax(this.tag.value);
                    this.NumericBox.Visibility = Visibility.Visible;
                    break;
                case TagType.LocalConstant:
                case TagType.HiddenField:
                    this.Textbox.Text = this.tag.value;
                    this.Visibility = Visibility.Collapsed;
                    break;
                case TagType.LocalEnumeration:
                    this.SetComboBoxMode();
                    this.Combobox.ItemsSource = this.tag.value.Split(';');
                    this.Combobox.SelectedIndex = 0;
                    if (!string.IsNullOrWhiteSpace(this.tag.description))
                    {
                        this.Combobox.ToolTip = this.tag.description;
                    }
                    break;
                case TagType.Relation:
                    this.SelectionBox.Visibility = Visibility.Visible;
                    this.SelectionBox.Source = this.tag.value;
                    this.SelectionBox.ToolTip = this.tag.description;
                    break;
                case TagType.Date:
                    this.DatePicker.Visibility = Visibility.Visible;
                    this.DatePicker.ToolTip = this.tag.description;
                    break;
                case TagType.Generator:
                    this.Generator.Visibility = Visibility.Visible;
                    this.Generator.TemplateId = int.Parse(this.tag.value);
                    break;
            }
            this.MakeButton();
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
            if (fc.FKtable != null)
            {
                this.tag.type = TagType.Relation;
                this.SelectionBox.Visibility = Visibility.Visible;
                this.SelectionBox.Database = path;
                this.SelectionBox.Table = fc.FKtable;
                this.SelectionBox.Field = fc.FKfield;
            }
            else
            {
                this.tag.type = TagType.Text;
                this.SetTextBoxMode();
            }
        }

        public void SetValue(string value)
        {
            switch (this.tag.type)
            {
                case TagType.Variable:
                case TagType.Text:
                case TagType.HiddenField:
                default:
                    this.Textbox.Text = value;
                    break;
                case TagType.Number:
                    int digitValue;
                    if (int.TryParse(value, out digitValue))
                    {
                        this.NumericBox.Value = digitValue;
                    }
                    break;
                case TagType.Relation:
                    this.SelectionBox.Value = value;
                    break;
                case TagType.LocalConstant:
                    return;
                case TagType.LocalEnumeration:
                    this.Combobox.SelectedValue = value;
                    break;
                case TagType.Date:
                    DateTime parsedDate;
                    bool success = DateTime.TryParseExact(value, "dd.MM.yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out parsedDate);
                    if (success)
                    {
                        this.DatePicker.SelectedDate = parsedDate;
                    }
                    break;
                case TagType.Generator:
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
            if (this.tag.type is TagType.Variable or TagType.Text)
            {
                if (this.isRequired && string.IsNullOrEmpty(this.Textbox.Text))
                {
                    ProgramState.ShowExclamationDialog($"Поле \"{this.tag.name}\" обязательно для заполнения!", "Действие прервано");
                    return false;
                }
            }
            return true;
        }
        private string GetDateInFormat()
        {
            return string.IsNullOrWhiteSpace(this.tag.value)
                ? this.DatePicker.SelectedDate.ToString()
                : this.DatePicker.SelectedDate.HasValue ? ((DateTime)this.DatePicker.SelectedDate).ToString(this.tag.value) : "";
        }
        public string GetValue()
        {

            switch (this.tag.type)
            {
                case TagType.Variable:
                default:
                    return this.Textbox.Text;
                case TagType.Number:
                    return this.NumericBox.Value.ToString();
                case TagType.LocalConstant:
                    return this.tag.value;
                case TagType.Relation:
                    return this.SelectionBox.Value;
                case TagType.LocalEnumeration:
                    if (this.Combobox.SelectedIndex != -1)
                    {
                        return this.Combobox.Items.GetItemAt(this.Combobox.SelectedIndex).ToString();
                    }
                    return "";
                case TagType.Date:
                    return this.GetDateInFormat();
                case TagType.Generator:
                    return this.Generator.GetText();
            }
        }
        public string GetData()
        {
            switch (this.tag.type)
            {
                case TagType.Generator:
                    return this.Generator.GetData();
                case TagType.Date:
                    if (this.DatePicker.SelectedDate.HasValue)
                    {
                        return ((DateTime)this.DatePicker.SelectedDate).ToString("dd.MM.yyyy");
                    }
                    return "";
                default: return this.GetValue();

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
            if (this.IsAnythingSelected())
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
            if (this.IsAnythingSelected())
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
            if (this.IsAnythingSelected())
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
            if (this.IsAnythingSelected())
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
            OnInsert?.Invoke(this.tag.id, this.GetData());
        }
        private void RecalculateNamesClick(object sender, RoutedEventArgs e)
        {
            OnRename?.Invoke(this.tag.name);
        }

        private void TextRequest(object sender, RoutedEventArgs e)
        {
            Session session;
            if (ProgramState.ShowActiveUserSelector(out session, "Выберите пользователя для запроса данных"))
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

                ProgramState.ShowErrorDialog("При обработке скрипта произошла ошибка:\n" + ex.Message);
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
