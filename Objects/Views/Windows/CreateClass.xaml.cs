using Incas.Core.Classes;
using Incas.Objects.Models;
using Incas.Objects.ViewModels;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Collections.Generic;
using System;

namespace Incas.Objects.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateClass.xaml
    /// </summary>
    public partial class CreateClass : Window
    {
        ClassViewModel vm;
        public CreateClass()
        {
            this.InitializeComponent();
            this.vm = new(new());
            this.DataContext = this.vm;
        }
        public CreateClass(Guid id)
        {
            this.InitializeComponent();
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
            this.ContentPanel.Children.Add(fc);
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
