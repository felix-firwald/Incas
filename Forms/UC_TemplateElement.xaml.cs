using Common;
using Incas.Core.Views.Windows;
using Incas.Templates.Models;
using Incas.Templates.Views.Windows;
using IncasEngine.TemplateManager;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public STemplate template;

        public delegate void Base();
        public event Base OnUpdated;
        public UC_TemplateElement(STemplate t)
        {
            this.InitializeComponent();
            this.template = t;
            this.MainLabel.Content = this.template.name;
            this.FindChilds();
        }

        private async void FindChilds()
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                List<string> parents = this.template.parent.Split(";").ToList();
                parents.Add(this.template.id.ToString());
                List<STemplate> children = this.template.AsModel().GetAllChildren(parents);
                if (children.Count > 0)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (STemplate item in children)
                        {
                            UC_TemplateElement c = new(item);
                            this.ChildPanel.Children.Add(c);
                            c.OnUpdated += this.UpdateList;
                        }
                        if (this.ChildPanel.Children.Count > 0)
                        {
                            this.MainLabel.Style = this.FindResource("LabelElementSuccess") as Style;
                            this.UseButton.Visibility = Visibility.Hidden;
                            this.ParentIcon.Visibility = Visibility.Visible;
                            this.Line.Visibility = Visibility.Visible;
                            this.ParentIconBottom.Visibility = Visibility.Visible;
                        }
                    });
                }

            });
        }

        private void AddClick(object sender, MouseButtonEventArgs e)
        {
            ProgramState.ShowWaitCursor();
            switch (this.template.type)
            {
                case TemplateType.Word:
                case TemplateType.Excel:
                    if (this.IsFileExists())
                    {
                        UseTemplate ut = new(this.template.AsModel());
                        ut.Show();
                    }
                    else
                    {
                        ProgramState.ShowErrorDialog($"Файл шаблона \"{this.template.name}\" ({this.template.path}) не найден.\nОтредактируйте шаблон, указав правильный путь к файлу, чтобы его использование стало возможным.", "Использование шаблона невозможно");
                    }
                    break;
            }
        }
        private bool IsFileExists()
        {
            return File.Exists($"{ProgramState.TemplatesSourcesWordPath}\\{this.template.path}") || File.Exists($"{ProgramState.TemplatesSourcesExcelPath}\\{this.template.path}");
        }

        private void RemoveClick(object sender, MouseButtonEventArgs e)
        {
            if (ProgramState.CurrentUserParameters.modify_templates)
            {
                if (ProgramState.IsWorkspaceOpened())
                {
                    if (this.ChildPanel.Children.Count > 0)
                    {
                        ProgramState.ShowExclamationDialog("Шаблон нельзя удалить, пока на него ссылается хотя бы один наследник.", "Удаление прервано");
                        return;
                    }
                    if (ProgramState.ShowQuestionDialog($"Шаблон \"{this.template.name}\" будет безвозвратно удален, однако файл, используемый шаблоном, останется.", "Удалить шаблон?", "Удалить шаблон", "Не удалять") == DialogStatus.Yes)
                    {
                        Tag tag = new();
                        tag.RemoveAllTagsByTemplate(this.template.id);
                        this.template.AsModel().RemoveTemplate();
                        this.UpdateList();
                    }
                }
            }
            else
            {
                ProgramState.ShowAccessErrorDialog("Вам недоступна функция удаления шаблонов.", "Удаление прервано");
            }
        }

        private void EditClick(object sender, MouseButtonEventArgs e)
        {
            if (ProgramState.CurrentUserParameters.modify_templates)
            {
                if (ProgramState.IsWorkspaceOpened())
                {
                    ProgramState.ShowWaitCursor();
                    CreateDocumentTemplate ctw = new(this.template.AsModel());
                    ctw.OnCreated += this.UpdateList;
                    ctw.Show();
                }
            }
            else
            {
                ProgramState.ShowAccessErrorDialog("Вам недоступна функция редактирования шаблонов.", "Редактирование прервано");
            }
        }

        private void CreateChildClick(object sender, MouseButtonEventArgs e)
        {
            if (ProgramState.CurrentUserParameters.create_templates)
            {
                if (ProgramState.IsWorkspaceOpened())
                {
                    string parents = this.template.id.ToString();
                    if (!string.IsNullOrWhiteSpace(this.template.parent))
                    {
                        parents = this.template.parent + ";" + parents;
                    }
                    CreateDocumentTemplate ctw = new(parents: parents);
                    ctw.OnCreated += this.UpdateList;
                    ctw.Show();
                }
            }
            else
            {
                ProgramState.ShowAccessErrorDialog("Вам недоступна функция создания шаблонов.", "Создание прервано");
            }
        }
        private void UpdateList()
        {
            OnUpdated?.Invoke();
        }

        private void ShowTransferClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
