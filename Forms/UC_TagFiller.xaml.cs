using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace Incubator_2.Forms
{
    /// <summary>
    /// Логика взаимодействия для UC_TagFiller.xaml
    /// </summary>
    public partial class UC_TagFiller : UserControl
    {
        private Tag tag;
        public UC_TagFiller(Tag t)
        {
            InitializeComponent();
            this.tag = t;
            this.MainLabel.Content = this.tag.name + ":";
            switch (tag.type)
            {
                case TypeOfTag.Variable:
                case TypeOfTag.LocalConstant:
                case TypeOfTag.GlobalConstant:
                default:
                    SetTextBoxMode();
                    break;
                case TypeOfTag.GlobalEnumeration:
                case TypeOfTag.LocalEnumeration:
                    SetComboBoxMode();
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
                case TypeOfTag.GlobalEnumeration:
                case TypeOfTag.LocalEnumeration:
                    return this.Combobox.SelectedValue.ToString();

            }
        }
    }
}
