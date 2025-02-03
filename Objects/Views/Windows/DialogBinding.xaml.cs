using Incas.Core.Classes;
using Incas.Objects.Components;
using Incas.Objects.ViewModels;
using IncasEngine.ObjectiveEngine.Common;
using Newtonsoft.Json;
using System;
using System.Windows;
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
        public DialogBinding(string data)
        {
            this.InitializeComponent();
            if (string.IsNullOrEmpty(data))
            {
                this.vm = new(new());
            }
            else
            {
                try
                {
                    this.vm = new(JsonConvert.DeserializeObject<BindingData>(data));
                }
                catch
                {
                    this.vm = new(new());
                }
            }
            this.DataContext = this.vm;
        }
        //private void ApplySelected(BindingData data)
        //{
        //    if (data.Class == Guid.Empty)
        //    {
        //        return;
        //    }
        //    foreach (ListBoxItem t in this.ClassesPanel.Items)
        //    {
        //        if (t.Uid == data.Class.ToString())
        //        {
        //            t.IsSelected = true;
        //            this.LoadClassFields();
        //            foreach (ListBoxItem fieldItem in this.FieldsPanel.Items)
        //            {
        //                if (fieldItem.Uid == data.Field.ToString())
        //                {
        //                    fieldItem.IsSelected = true;
        //                    break;
        //                }
        //            }
        //            break;
        //        }
        //    }
        //}
        //private void LoadClasses()
        //{
        //    using (Class cl = new())
        //    {
        //        foreach (Class item in cl.GetAllClasses())
        //        {
        //            TreeViewItem tvi = new()
        //            {
        //                Header = item.name,
        //                Uid = item.identifier.ToString() // system guid
        //            };
        //            tvi.Selected += this.Tvi_Selected;
        //            this.ClassesPanel.Items.Add(tvi);
        //        }
        //    }            
        //}

        //private void LoadClassFields()
        //{
        //    using (Class cl = new())
        //    {
        //        cl.GetClassById(this.SelectedClass);
        //        ClassData cd = cl.GetClassData();
        //        foreach (Incas.Objects.Models.Field f in cd.fields)
        //        {
        //            ListBoxItem sub = new()
        //            {
        //                Content = f.Name,
        //                Uid = f.Id.ToString()
        //            };
        //            sub.Selected += this.Field_Selected;
        //            this.FieldsPanel.Items.Add(sub);

        //        }
        //    }
        //}
        //private void Tvi_Selected(object sender, RoutedEventArgs e)
        //{
        //    ListBoxItem item = (ListBoxItem)sender;
        //    this.SelectedClass = System.Guid.Parse(item.Uid);
        //    this.LoadClassFields();
        //}

        //private void Field_Selected(object sender, RoutedEventArgs e)
        //{
        //    ListBoxItem item = (ListBoxItem)sender;
        //    Guid id = System.Guid.Parse(item.Uid);
        //    this.SelectedField = id;
        //}

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
            if (this.vm.SelectedClass is null)
            {
                DialogsManager.ShowExclamationDialog("Не выбран класс объекта!", "Сохранение прервано");
                return;
            }
            if (this.vm.SelectedField is null)
            {
                DialogsManager.ShowExclamationDialog("Не выбрано поле у объекта!", "Сохранение прервано");
                return;
            }
            this.SelectedClass = this.vm.SelectedClass.Id;
            this.SelectedField = this.vm.SelectedField.Id;
            this.Result = true;
            this.Close();
        }
    }
}
