using Common;
using Incubator_2.ViewModels;
using Incubator_2.Windows;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;


namespace Forms
{
    /// <summary>
    /// Логика взаимодействия для UC_TemplateElement.xaml
    /// </summary>
    public partial class UC_TemplateElement : UserControl
    {
        public Template template;

        public delegate void Base();
        public event Base OnUpdated;
        public UC_TemplateElement(Template t)
        {
            InitializeComponent();
            template = t;
            this.MainLabel.Content = template.name;
            //GetChilds();
            IsChild();
        }
        public void AddChild(Template t)
        {
            UC_TemplateElement c = new UC_TemplateElement(t);
            this.ChildPanel.Children.Add(c);
            c.OnUpdated += UpdateList;
            if (this.ChildPanel.Children.Count > 0)
            {
                this.MainLabel.Style = FindResource("LabelElementSpecial") as Style;
                this.UseButton.Visibility = Visibility.Hidden;
                this.ParentIcon.Visibility = Visibility.Visible;
            }
        }
        private void IsChild()
        {
            if (this.template.parent != 0)
            {
                this.CreateChild.Visibility = Visibility.Collapsed;
            }
        }

        private void AddClick(object sender, MouseButtonEventArgs e)
        {
            if (this.ChildPanel.Children.Count > 0)
            {
                ProgramState.ShowErrorDialog("Абстрактные шаблоны нельзя использовать напрямую.\nВместо этого, используйте одного из его наследников.", "Использование шаблона невозможно");
                return;
            }
            if (IsFileExists())
            {
                UseTemplate ut = new UseTemplate(template);
                ut.Show();
            }
            else
            {
                ProgramState.ShowErrorDialog($"Файл шаблона \"{template.name}\" ({ProgramState.TemplatesSourcesWordPath}\\{template.path}) не найден.\nОтредактируйте шаблон, указав правильный путь к файлу, чтобы его использование стало возможным.", "Использование шаблона невозможно");
            }
        }
        private bool IsFileExists()
        {
            return File.Exists($"{ProgramState.TemplatesSourcesWordPath}\\{template.path}");
        }

        private void RemoveClick(object sender, MouseButtonEventArgs e)
        {
            if (this.ChildPanel.Children.Count > 0)
            {
                ProgramState.ShowErrorDialog("Шаблон нельзя удалить, пока на него ссылается хотя бы один наследник.", "Удаление прервано");
                return;
            }
            if (ProgramState.ShowQuestionDialog($"Шаблон \"{template.name}\" будет безвозвратно удален, однако файл, используемый шаблоном, останется.", "Удалить шаблон?", "Удалить шаблон", "Не удалять") == DialogStatus.Yes)
            {
                Models.Tag tag = new Models.Tag();
                tag.RemoveAllTagsByTemplate(template.id);
                template.RemoveTemplate();
                UpdateList();
            }
        }

        private void EditClick(object sender, MouseButtonEventArgs e)
        {
            if (this.template.parent != 0)  // если это ребенок
            {
                VM_ChildTemplate vm = new VM_ChildTemplate(this.template.parent, this.template);
                CreateChildOfTemplate cc = new CreateChildOfTemplate(vm);
                cc.OnCreated += UpdateList;
                cc.ShowDialog();
            }
            else
            {
                CreateTemplateWord ctw = new CreateTemplateWord(this.template);
                ctw.OnCreated += UpdateList;
                ctw.ShowDialog();
            }
        }

        private void CreateChildClick(object sender, MouseButtonEventArgs e)
        {
            VM_ChildTemplate vm = new VM_ChildTemplate(this.template.id);
            CreateChildOfTemplate cc = new CreateChildOfTemplate(vm);
            cc.OnCreated += UpdateList;
            cc.ShowDialog();
        }
        private void UpdateList()
        {
            OnUpdated?.Invoke();
        }
    }
}
