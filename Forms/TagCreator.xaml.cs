using Common;
using Incubator_2.ViewModels;
using Incubator_2.Windows;
using Incubator_2.Windows.CustomDatabase;
using Incubator_2.Windows.Templates;
using Models;
using System;
using System.Windows;
using System.Windows.Controls;


namespace Incubator_2.Forms
{
    /// <summary>
    /// Логика взаимодействия для TagCreator.xaml
    /// </summary>
    public partial class TagCreator : UserControl
    {
        public delegate void MethodContainer(TagCreator t);
        public event MethodContainer onDelete;
        public Tag tag;
        VM_Tag vm;

        private bool IsCollapsed = false;
        public TagCreator(Tag t, bool isNew = false)
        {
            InitializeComponent();
            tag = t;
            vm = new(t);
            this.DataContext = vm;
            if (t.parent is not 0)
            {
                this.OverridenLabel.Visibility = System.Windows.Visibility.Visible;
                this.TagName.IsEnabled = false;
            }
        }

        public bool Check()
        {
            switch (this.tag.type)
            {
                case TypeOfTag.Variable:
                case TypeOfTag.Text:
                default:
                    return true;
                case TypeOfTag.Relation:
                case TypeOfTag.LocalEnumeration:
                case TypeOfTag.Generator:
                case TypeOfTag.Table:
                    if (string.IsNullOrWhiteSpace(this.tag.value))
                    {
                        return false;
                    }
                    return true;
            }
        }

        public void SaveTag(int templ, bool isEdit = false)
        {
            tag.template = templ;
            if (isEdit)
            {
                tag.UpdateTag();
            }
            else
            {
                tag.AddTag();
            }
        }
        public void Minimize()
        {
            this.MainBorder.Height = 40;
            this.IsCollapsed = !this.IsCollapsed;
            this.NumberUp.Visibility = Visibility.Visible;
        }
        public void Maximize()
        {
            this.MainBorder.Height = this.ContentPanel.Height + 40;
            this.IsCollapsed = !this.IsCollapsed;
            this.NumberUp.Visibility = Visibility.Collapsed;
        }

        private void TurnSizeClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.IsCollapsed)
            {
                Maximize();
            }
            else
            {
                Minimize();
            }
        }

        private void RemoveClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            onDelete?.Invoke(this);
            tag.RemoveTag();
        }

        private void CopyAllClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Clipboard.SetText(this.tag.value);
        }

        private void ClearClick(object sender, RoutedEventArgs e)
        {
            this.vm.DefaultValue = "";
        }

        private void AddVirtualTagClick(object sender, RoutedEventArgs e)
        {
            int start = this.vm.DefaultValue is not null ? this.vm.DefaultValue.Length : 0;
            this.vm.DefaultValue += "[Новый]";
            this.MainTextBox.SelectionStart = start + 1;
            this.MainTextBox.SelectionLength = 5;
        }

        private void DefineRelationClick(object sender, RoutedEventArgs e)
        {
            BindingSelector bs = new();
            bs.ShowDialog();
            if (bs.Result == Windows.DialogStatus.Yes)
            {
                this.vm.DefaultValue = $"{bs.SelectedDatabase}.{bs.SelectedTable}.{bs.SelectedField}";
            }

        }

        private void DefineGeneratorClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(vm.DefaultValue))
                {
                    if (ProgramState.ShowQuestionDialog(
                    "Вы хотите использовать существующий генератор или создать новый?",
                    "Новый или существующий",
                    "Новый", "Существующий") == Windows.DialogStatus.Yes)
                    {
                        CreateTextTemplate ctt = new();
                        ctt.ShowDialog();
                        vm.DefaultValue = ctt.template.id.ToString();
                    }
                    else
                    {
                        Template t = ProgramState.ShowTemplateSelector(TemplateType.Text, "Выберите генератор для использования");
                        if (t.id != 0)
                        {
                            vm.DefaultValue = t.id.ToString();
                        }
                    }
                }
                else
                {
                    using (Template t = new Template())
                    {
                        CreateTextTemplate ctt = new(t.GetTemplateById(int.Parse(vm.DefaultValue)));
                        ctt.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog("При попытке открытия генератора возникла ошибка:\n" + ex.Message);
            }

        }

        private void EditScriptClick(object sender, RoutedEventArgs e)
        {
            CreateTagCommand ct = new(tag.GetCommand());
            ct.ShowDialog();
            if (ct.Result == DialogStatus.Yes)
            {
                tag.SaveCommand(ct.Command);
            }
            else if (ct.Result == DialogStatus.No)
            {
                tag.command = "";
            }
        }
        public void SetOrderNumber(int orderNumber)
        {
            this.vm.OrderNumber = orderNumber;
        }
        private void UpClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.vm.IncrementOrder();
        }

        private void DownClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.vm.DecrementOrder();
        }
    }
}
