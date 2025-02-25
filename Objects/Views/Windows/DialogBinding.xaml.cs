using Incas.Core.Classes;
using Incas.Objects.Components;
using Incas.Objects.ViewModels;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incas.Objects.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для DialogBinding.xaml
    /// </summary>
    public partial class DialogBinding : Window
    {
        public Guid SelectedClass { get; set; }
        public Guid SelectedField { get; set; }
        public DialogBindingViewModel vm;
        public bool Result = false;
        public DialogBinding(ObservableCollection<FieldViewModel> myClassFields, Field data)
        {
            this.InitializeComponent();
            BindingData bd = data.GetBindingData();
            if (bd is null)
            {
                this.vm = new(myClassFields, new());
            }
            else
            {
                try
                {
                    this.vm = new(myClassFields, bd);
                }
                catch
                {
                    this.vm = new(myClassFields, new());
                }
            }
            this.DataContext = this.vm;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void FinishClick(object sender, RoutedEventArgs e)
        {
            if (this.vm.SelectedClass.Id == Guid.Empty.ToString())
            {
                DialogsManager.ShowExclamationDialog("Не выбран класс объекта!", "Сохранение прервано");
                return;
            }
            if (this.vm.BindingField is null)
            {
                DialogsManager.ShowExclamationDialog("Не выбрано поле у объекта!", "Сохранение прервано");
                return;
            }
            this.SelectedClass = Guid.Parse(this.vm.SelectedClass.Id);
            this.SelectedField = this.vm.BindingField.Id;
            this.Result = true;
            this.Close();
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            this.vm.AddConstraint();
        }

        private void ClassesPanel_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
