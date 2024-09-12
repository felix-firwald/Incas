using Incas.Core.Classes;
using Incas.Core.Views.Controls;
using Incas.Miniservices.Clipboard.AutoUI;
using Incas.Miniservices.Clipboard.Classes;
using Incas.Miniservices.Clipboard.Views.Controls;
using Incas.Miniservices.Tasks.AutoUI;
using Incas.Miniservices.Tasks.Classes;
using Incas.Miniservices.Tasks.Views.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Incas.Miniservices.Tasks.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для Taskboard.xaml
    /// </summary>
    public partial class Taskboard : Window
    {
        public Taskboard()
        {
            this.InitializeComponent();
        }

        public void UpdateRecordsView()
        {
            this.PlaceElements(TaskManager.GetTaskboardRecords());
        }
        private void PlaceElements(List<TaskRecord> elements)
        {
            this.ContentPanel.Children.Clear();
            foreach (TaskRecord rec in elements)
            {
                TaskElement ce = new(rec);
                ce.OnUpdateRequested += this.OnUpdateRequested;
                this.ContentPanel.Children.Add(ce);
            }
            if (this.ContentPanel.Children.Count == 0)
            {
                this.ContentPanel.Children.Add(new NoContent());
            }
        }

        private void OnUpdateRequested()
        {
            this.FindText.Text = "";
            this.UpdateRecordsView();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            TaskManager.ClearCache();
            this.Close();
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            TaskRecord cr = new();
            DialogsManager.ShowSimpleFormDialog(cr, "Добавление задачи");
            this.UpdateRecordsView();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (this.FindText.Text.Length > 3)
            {
                this.PlaceElements(TaskManager.FindByName(this.FindText.Text));
            }
            else
            {
                this.UpdateRecordsView();
            }
        }
    }
}
