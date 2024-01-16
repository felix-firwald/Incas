using Common;
using Incubator_2.Common;
using Incubator_2.Forms.OneInstance;
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
            RegistreCreatedJSON.GetRegistry();
            foreach (SGeneratedDocument record in RegistreCreatedJSON.generatedDocuments)
            {
                FileCreated fc = new FileCreated(record);
                fc.OnSelectorChecked += AddToSelection;
                fc.OnSelectorUnchecked += RemoveFromSelection;
                this.ContentPanel.Children.Add(fc);
            }
            if (this.ContentPanel.Children.Count == 0)
            {
                this.ContentPanel.Children.Add(new NoContent());
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
                    RegistreCreatedJSON.GetRegistry();
                    foreach (FileCreated fc in Selection)
                    {
                        this.ContentPanel.Children.Remove(fc);
                        RegistreCreatedJSON.RemoveRecord(fc.record);
                    }
                    RegistreCreatedJSON.SaveRegistry();
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
                    if (tm.GetTemplateById(Selection[0].record.template_id) != null)
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
