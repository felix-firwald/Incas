using Common;
using Incubator_2.Common;
using Microsoft.Scripting.Hosting;
using Models;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
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
            this.Mode = FillerMode.Tag;
            this.tag = t;
            if (string.IsNullOrWhiteSpace(this.tag.visibleName))
            {
                this.MainLabel.Text = this.tag.name + ":";
            }
            else
            {
                this.MainLabel.Text = this.tag.visibleName + ":";
            }

            switch (this.tag.type)
            {
                case TypeOfTag.Variable:
                default:
                    this.SetTextBoxMode();
                    this.Textbox.Text = this.tag.value;
                    this.Textbox.MaxLength = 120;
                    this.Textbox.Tag = this.tag.description;
                    break;
                case TypeOfTag.Text:
                    this.SetTextBoxMode();
                    this.Textbox.Text = this.tag.value;
                    this.Textbox.Style = this.FindResource("TextBoxBig") as System.Windows.Style;
                    this.Textbox.MaxLength = 1200;
                    this.Textbox.Tag = this.tag.description;
                    break;
                case TypeOfTag.LocalConstant:
                case TypeOfTag.HiddenField:
                    this.Textbox.Text = this.tag.value;
                    this.Visibility = Visibility.Collapsed;
                    break;
                case TypeOfTag.LocalEnumeration:
                    this.SetComboBoxMode();
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
        public UC_TagFiller(FieldCreator fc, string path)
        {
            InitializeComponent();
            this.tag = new();
            this.Mode = FillerMode.RecordField;
            this.tag.name = fc.Name;
            this.MainLabel.Text = this.tag.name + ":";
            this.isRequired = fc.NotNULL;
            if (fc.FKtable != null)
            {
                this.tag.type = TypeOfTag.Relation;
                this.SelectionBox.Visibility = Visibility.Visible;
                this.SelectionBox.Database = path;
                this.SelectionBox.Table = fc.FKtable;
                this.SelectionBox.Field = fc.FKfield;
            }
            else
            {
                this.tag.type = TypeOfTag.Text;
                this.SetTextBoxMode();
            }
        }

        public void SetValue(string value)
        {
            switch (this.tag.type)
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
            if (this.tag.type == TypeOfTag.Variable || this.tag.type == TypeOfTag.Text)
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

            switch (this.tag.type)
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
                    return this.GetDateInFormat();
                case TypeOfTag.Generator:
                    return this.Generator.GetText();
            }
        }
        public string GetData()
        {
            switch (this.tag.type)
            {
                case TypeOfTag.Generator:
                    return this.Generator.GetData();
                case TypeOfTag.Date:
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
    }
}
