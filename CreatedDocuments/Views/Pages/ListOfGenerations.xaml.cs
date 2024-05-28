using Incas.Common;
using Incas.Core.Views.Controls;
using Incas.CreatedDocuments.Models;
using Incas.CreatedDocuments.Views.Controls;
using Incas.Templates.Models;
using Incas.Templates.Views.Windows;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incas.CreatedDocuments.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ListOfGenerations.xaml
    /// </summary>
    public partial class ListOfGenerations : UserControl
    {
        private List<FileCreated> Selection = [];
        public ListOfGenerations()
        {
            this.InitializeComponent();
            this.UpdateList();
        }
        public void UpdateList()
        {
            this.ContentPanel.Children.Clear();
            using (GeneratedDocument d = new())
            {
                foreach (string template in d.GetAllUsedTemplates())
                {
                    Expander exp = new()
                    {
                        Header = template
                    };
                    exp.Expanded += this.CurrentExpander_Expanded;
                    exp.Style = this.FindResource("ExpanderMain") as Style;
                    this.ContentPanel.Children.Add(exp);
                }
            }
            if (this.ContentPanel.Children.Count == 0)
            {
                this.ContentPanel.Children.Add(new NoContent());
            }
        }

        private void CurrentExpander_Expanded(object sender, RoutedEventArgs e)
        {
            Expander exp = (Expander)sender;
            this.FillExpander(exp, exp.Header.ToString());
        }

        public void FillExpander(Expander exp, string templateName)
        {
            if (exp.Content == null)
            {
                using GeneratedDocument d = new();
                StackPanel content = new();
                int counter = 1;
                d.GetDocumentsByTemplate(templateName).ForEach(document =>
                {
                    FileCreated fc = new FileCreated(ref document, counter);
                    fc.OnSelectorChecked += this.AddToSelection;
                    fc.OnSelectorUnchecked += this.RemoveFromSelection;
                    content.Children.Add(fc);
                    counter++;
                });
                exp.Content = content;
            }
        }

        private void AddToSelection(FileCreated fc)
        {
            this.Selection.Add(fc);
        }
        private void RemoveFromSelection(FileCreated fc)
        {
            this.Selection.Remove(fc);
        }

        private void UpdateClick(object sender, MouseButtonEventArgs e)
        {
            this.Selection.Clear();
            this.UpdateList();
        }

        private void RemoveClick(object sender, MouseButtonEventArgs e)
        {
            if (ProgramState.IsWorkspaceOpened())
            {
                if (this.Selection.Count > 0)
                {
                    List<int> docs = [];
                    foreach (FileCreated item in this.Selection)
                    {
                        docs.Add(item.record.id);
                    }
                    ProgramState.ShowWaitCursor();
                    using (GeneratedDocument d = new())
                    {
                        d.RemoveRecords(docs);
                    }
                    ProgramState.ShowWaitCursor(false);
                    this.UpdateList();
                }
                else
                {
                    ProgramState.ShowExclamationDialog("Не выбрано ни одного элемента для удаления.\n" +
                        "Используйте селекторы для выбора тех записей, которые нужно удалить.", "Действие невозможно");
                }
            }
        }

        private void UseClick(object sender, MouseButtonEventArgs e)
        {
            if (this.Selection.Count > 0)
            {
                using Template tm = new();
                ProgramState.ShowWaitCursor();
                if (tm.GetTemplateById(this.Selection[0].record.template) != null)
                {
                    try
                    {
                        List<SGeneratedDocument> tj = [];
                        foreach (FileCreated fc in this.Selection)
                        {
                            tj.Add(fc.record);
                        }
                        UseTemplate ut = new UseTemplate(tm, tj);
                        ut.Show();
                    }
                    catch (IOException ex)
                    {
                        ProgramState.ShowWaitCursor(false);
                        ProgramState.ShowErrorDialog($"Один из файлов поврежден или удален. Пожалуйста, попробуйте ещё раз.\n{ex}");
                    }
                }
            }
            else
            {
                ProgramState.ShowExclamationDialog("Не выбрано ни одного элемента для отправки в секвенсор.\n" +
                    "Используйте селекторы для выбора тех записей, которые нужно открыть.", "Действие невозможно");
            }
        }
    }
}
