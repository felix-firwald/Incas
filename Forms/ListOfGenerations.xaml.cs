using Incubator_2.Common;
using Incubator_2.Forms.OneInstance;
using System;
using System.Collections.Generic;
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
            foreach (STemplateJSON record in RegistreCreatedJSON.generatedDocuments)
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
            UpdateList();
        }

        private void RemoveClick(object sender, MouseButtonEventArgs e)
        {
            RegistreCreatedJSON.GetRegistry();
            foreach (FileCreated fc in Selection)
            {
                this.ContentPanel.Children.Remove(fc);
                RegistreCreatedJSON.RemoveRecord(fc.record);
            }
            RegistreCreatedJSON.SaveRegistry();
        }
    }
}
