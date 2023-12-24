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
        Template template;
        public UC_TemplateElement(Template t)
        {
            InitializeComponent();
            template = t;
            this.MainLabel.Content = template.name;
            GetChilds();
            
        }

        private void AddClick(object sender, MouseButtonEventArgs e)
        {
            if (template.isAbstract)
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
        private void GetChilds()
        {
            template.GetChildren().ForEach(c =>
            {
                this.ChildPanel.Children.Add(new UC_TemplateElement(c));
            });
            if (this.ChildPanel.Children.Count > 0)
            {
                this.MainLabel.Style = FindResource("LabelElementSpecial") as Style;
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
            }
        }

        private void EditClick(object sender, MouseButtonEventArgs e)
        {
            CreateTemplateWord ctw = new CreateTemplateWord(this.template);
            ctw.Show();
        }
    }
}
