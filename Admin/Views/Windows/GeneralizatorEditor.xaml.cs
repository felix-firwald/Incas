using Incas.Admin.ViewModels;
using Incas.Core.Classes;
using Incas.Objects.ViewModels;
using IncasEngine.Models;
using IncasEngine.ObjectiveEngine.Exceptions;
using System;
using System.Windows;

namespace Incas.Admin.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для GeneralizatorEditor.xaml
    /// </summary>
    public partial class GeneralizatorEditor : Window
    {
        private GeneralizatorViewModel vm { get; set; }
        public GeneralizatorEditor() // if new
        {
            this.InitializeComponent();
            this.Title = "Создание обобщения";
            this.vm = new();
            this.DataContext = this.vm;
        }
        public GeneralizatorEditor(GeneralizatorItem item) // if edit
        {
            this.InitializeComponent();
            this.Title = "Редактирование обобщения";
            this.vm = new(new(item));
            this.DataContext = this.vm;
        }

        private void AddFieldClick(object sender, RoutedEventArgs e)
        {
            this.vm.AddField(new());
        }
        private void MinimizeAllClick(object sender, RoutedEventArgs e)
        {
            foreach (FieldViewModel f in this.vm.Fields)
            {
                f.IsExpanded = false;
            }
        }

        private void MaximizeAllClick(object sender, RoutedEventArgs e)
        {
            foreach (FieldViewModel f in this.vm.Fields)
            {
                f.IsExpanded = true;
            }
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            DialogsManager.ShowWaitCursor();
            if (this.vm.Save())
            {
                DialogsManager.ShowWaitCursor(false);
                this.Close();
            }
        }
    }
}
