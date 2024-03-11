using Common;
using DocumentFormat.OpenXml.Wordprocessing;
using Incubator_2.Common;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Tag = Models.Tag;


namespace Incubator_2.Forms
{
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
        public delegate void StringAction(int tag, string text);
        public event StringAction OnInsert;
        public delegate void StringActionRecalculate(string tag);
        public event StringActionRecalculate OnRename;
        public UC_TagFiller(Tag t)
        {
            InitializeComponent();
            Mode = FillerMode.Tag;
            this.tag = t;
            this.MainLabel.Text = this.tag.name + ":";
            
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
                case TypeOfTag.Generator:
                    this.Generator.Visibility = Visibility.Visible;
                    this.Generator.TemplateId = int.Parse(this.tag.value);
                    break;
            }
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
                case TypeOfTag.Generator:
                    return this.Generator.GetText();

            }
        }
        public string GetData()
        {
            switch (tag.type)
            {
                default: return "";
                case TypeOfTag.Generator:
                    return this.Generator.GetData();

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
        private void AddIncrementedDate(object sender, RoutedEventArgs e)
        {
            IncrementDate(sender);
        }
        private void AddIncrementedLongDate(object sender, RoutedEventArgs e)
        {
            IncrementDate(sender, true);
        }

        private void IncrementDate(object sender, bool longType = false)
        {
            int days = int.Parse(((MenuItem)sender).Tag.ToString());
            string format = longType ? "D" : "dd.MM.yyyy";
            DateTime result = DateTime.Now;
            switch (days)
            {
                case 31:
                    result = result.AddMonths(1);
                    break;
                case 93:
                    result = result.AddMonths(3);
                    break;
                case 186:
                    result = result.AddMonths(6);
                    break;
                case 365:
                    result = result.AddYears(1);
                    break;
                default:
                    result = result.AddDays(days);
                    break;
            }
            this.Textbox.Text += result.ToString(format);
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
    }
}
