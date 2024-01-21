using Common;
using Incubator_2.Common;
using Incubator_2.Forms.OneInstance;
using Incubator_2.Models;
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Incubator_2.Forms
{
    /// <summary>
    /// Логика взаимодействия для ListOfGenerations.xaml
    /// </summary>
    public partial class ListOfGenerations : UserControl
    {
        List<FileCreated> Selection = new List<FileCreated>();
        public ListOfGenerations()
        {
            InitializeComponent();
            UpdateList();
        }
        public void UpdateList()
        {
            this.ContentPanel.Children.Clear();
            using (GeneratedDocument d = new())
            {
                foreach (string template in d.GetAllUsedTemplates())
                {
                    Expander exp = new();
                    exp.Header = template;
                    exp.Expanded += CurrentExpander_Expanded;
                    exp.Style = FindResource("ExpanderMain") as Style;
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
            FillExpander(exp, exp.Header.ToString());
        }

        public void FillExpander(Expander exp, string templateName)
        {
            if (exp.Content == null)
            {
                using (GeneratedDocument d = new())
                {
                    StackPanel content = new();
                    int counter = 1;
                    d.GetDocumentsByTemplate(templateName).ForEach(document =>
                    {
                        FileCreated fc = new FileCreated(document, counter);
                        fc.OnSelectorChecked += AddToSelection;
                        fc.OnSelectorUnchecked += RemoveFromSelection;
                        content.Children.Add(fc);
                        counter++;
                    });
                    exp.Content = content;
                }
            }
        }

        private void AddToSelection(FileCreated fc)
        {
            Selection.Add(fc);
        }
        private void RemoveFromSelection(FileCreated fc)
        {
            Selection.Remove(fc);
        }

        private void UpdateClick(object sender, MouseButtonEventArgs e)
        {
            Selection.Clear();
            UpdateList();
        }

        private void RemoveClick(object sender, MouseButtonEventArgs e)
        {
            if (ProgramState.IsWorkspaceOpened())
            {
                if (Selection.Count > 0)
                {
                    List<SGeneratedDocument> docs = new();
                    foreach (FileCreated item in Selection)
                    {
                        docs.Add(item.record);
                    }
                    ProgramState.ShowWaitCursor();
                    RegistreCreatedJSON.RemoveRecords(docs);
                    ProgramState.ShowWaitCursor(false);
                    this.UpdateList();
                }
                else
                {
                    ProgramState.ShowExlamationDialog("Не выбрано ни одного элемента для удаления.\n" +
                        "Используйте селекторы для выбора тех записей, которые нужно удалить.", "Действие невозможно");
                }
            }
        }

        private void UseClick(object sender, MouseButtonEventArgs e)
        {
            if (Selection.Count > 0)
            {
                using (Template tm = new())
                {
                    ProgramState.ShowWaitCursor();
                    if (tm.GetTemplateById(Selection[0].record.template) != null)
                    {
                        try
                        {
                            List<TemplateJSON> tj = new List<TemplateJSON>();
                            foreach (FileCreated fc in Selection)
                            {
                                tj.Add(RegistreCreatedJSON.LoadRecord(fc.record));
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
            }
            else
            {
                ProgramState.ShowExlamationDialog("Не выбрано ни одного элемента для отправки в секвенсор.\n" +
                    "Используйте селекторы для выбора тех записей, которые нужно открыть.", "Действие невозможно");
            }
        }
    }
}
