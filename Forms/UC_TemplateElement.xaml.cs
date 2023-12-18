using Common;
using Incubator_2.Windows;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        }

        private void AddClick(object sender, MouseButtonEventArgs e)
        {
            if (IsFileExists())
            {
                UseTemplate ut = new UseTemplate(template);
                ut.ShowDialog();
            }
            else
            {
                Dialog d = new Dialog($"Файл шаблона \"{template.name}\" ({ProgramState.TemplatesSourcesWordPath}\\{template.path}) не найден.\nОтредактируйте шаблон, указав правильный путь к файлу, чтобы его использование стало возможным.", "Использование шаблона невозможно"); ; ;
                d.ShowDialog();
            }
        }
        private bool IsFileExists()
        {
            return File.Exists($"{ProgramState.TemplatesSourcesWordPath}\\{template.path}");
        }

        private void RemoveClick(object sender, MouseButtonEventArgs e)
        {
            DialogQuestion dialog = new DialogQuestion($"Шаблон \"{template.name}\" будет безвозвратно удален, однако файл, используемый шаблоном, останется.", "Удалить шаблон?", "Удалить шаблон", "Не удалять");
            dialog.ShowDialog();
            if (dialog.status == DialogStatus.Yes)
            {
                template.RemoveTemplate();
            }
        }
    }
}
