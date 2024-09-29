using Incas.Core.Classes;
using Incas.Objects.Models;
using Incas.Objects.ViewModels;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Collections.Generic;
using System;
using Incas.Objects.AutoUI;
using Incas.Objects.Components;

namespace Incas.Objects.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateClass.xaml
    /// </summary>
    public partial class CreateClass : Window
    {
        ClassViewModel vm;
        public CreateClass(ClassTypeSettings primary)
        {
            this.InitializeComponent();
            this.vm = new(new());
            this.vm.CategoryOfClass = primary.Category;
            this.vm.NameOfClass = primary.Name;
            this.vm.Type = (ClassType)primary.Selector.SelectedObject;
            if (this.vm.Type == ClassType.Document)
            {
                this.vm.ShowCard = true;
            }
            this.DataContext = this.vm;
        }
        public CreateClass(Guid id)
        {
            this.InitializeComponent();
            this.Title = "Редактирование класса";
            Class cl = new(id);
            this.vm = new(cl);
            this.DataContext = this.vm;
            foreach (Models.Field f in cl.GetClassData().fields)
            {
                this.AddField(f);
            }
        }

        private void GetMoreInfoClick(object sender, MouseButtonEventArgs e)
        {
            ProgramState.OpenWebPage("https://teletype.in/@incas/classes");
        }

        private void AddField(Incas.Objects.Models.Field data = null)
        {
            Incas.Objects.Views.Controls.FieldCreator fc = new(data);
            fc.OnRemove += this.Fc_OnRemove;
            this.ContentPanel.Children.Add(fc);
        }

        private void Fc_OnRemove(Controls.FieldCreator t)
        {
            this.ContentPanel.Children.Remove(t);
        }

        private void AddFieldClick(object sender, MouseButtonEventArgs e)
        {
            this.AddField();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            List<Incas.Objects.Models.Field> fields = new();
            foreach (Incas.Objects.Views.Controls.FieldCreator item in this.ContentPanel.Children)
            {
                Incas.Objects.Models.Field f = item.vm.Source;
                f.SetId();
                fields.Add(f);
            }
            this.vm.SetData(fields);
            this.vm.Source.Save();
            this.Close();
        }
    }
}
