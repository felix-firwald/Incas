using Common;
using DocumentFormat.OpenXml.Wordprocessing;
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
    /// <summary>
    /// Логика взаимодействия для UC_TagFiller.xaml
    /// </summary>
    public partial class UC_TagFiller : UserControl
    {
        public readonly Tag tag;
        public delegate void StringAction(int tag, string text);
        public event StringAction OnInsert;
        public delegate void StringActionRecalculate(string tag);
        public event StringActionRecalculate OnRename;
        public UC_TagFiller(Tag t)
        {
            InitializeComponent();
            this.tag = t;
            this.MainLabel.Content = this.tag.name + ":";
            switch (tag.type)
            {
                case TypeOfTag.Variable:
                default:
                    SetTextBoxMode();
                    this.Textbox.Text = this.tag.value;
                    this.Textbox.MaxLength = 120;
                    break;
                case TypeOfTag.Text:
                    SetTextBoxMode();
                    this.Textbox.Text = this.tag.value;
                    this.Textbox.Style = FindResource("TextBoxBig") as System.Windows.Style;
                    this.Textbox.MaxLength = 1200;
                    break;
                case TypeOfTag.LocalConstant:
                    this.Visibility = Visibility.Collapsed;
                    break;
                case TypeOfTag.LocalEnumeration:
                    SetComboBoxMode();
                    this.Combobox.ItemsSource = this.tag.value.Split(';');
                    //this.Combobox.Text = this.tag.value;
                    this.Combobox.SelectedIndex = 0;
                    break;
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
                case TypeOfTag.LocalConstant:
                    return;
                case TypeOfTag.LocalEnumeration:
                    this.Combobox.SelectedValue = value;
                    break;
            }
        }

        private void SetTextBoxMode()
        {
            this.Textbox.Visibility = Visibility.Visible;
            this.Combobox.Visibility = Visibility.Hidden;
        }
        private void SetComboBoxMode()
        {
            this.Textbox.Visibility = Visibility.Hidden;
            this.Combobox.Visibility = Visibility.Visible;
        }
        public string GetTagName()
        {
            return this.tag.name;
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
                case TypeOfTag.LocalEnumeration:
                    if (this.Combobox.SelectedIndex != -1)
                    {
                        return this.Combobox.Items.GetItemAt(this.Combobox.SelectedIndex).ToString();
                    }
                    return "";

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
        
    }
}
