using Incas.Admin.ViewModels;
using Incas.Objects.AutoUI;
using Incas.Objects.Interfaces;
using Incas.Objects.ViewModels;
using IncasEngine.ObjectiveEngine.Models;
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

namespace Incas.Objects.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для TableEditor.xaml
    /// </summary>
    public partial class TableEditor : UserControl, IClassDetailsSettings
    {
        public string ItemName { get; private set; }
        public TableViewModel vm { get; private set; }
        public TableEditor(TableViewModel vm)
        {
            this.InitializeComponent();
            this.ItemName = $"Таблица [{vm.Name}]";
            this.vm = vm;
            this.DataContext = this.vm;
        }

        public void SetUpContext(IMembersContainerViewModel vm)
        {
            
        }

        private void AddFieldClick(object sender, RoutedEventArgs e)
        {
            Field field = new();
            field.SetId();
            field.Name = $"";
            this.vm.AddField(field);
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

        public void SetUpContext(GeneralizatorViewModel vm)
        {
            throw new NotImplementedException();
        }

        private void OpenFieldsGroupListVisibilityClick(object sender, RoutedEventArgs e)
        {
            FieldsGroupListVisibilitySettings form = new(this.vm.Fields);
            form.ShowDialog("Настройка видимых полей", Core.Classes.Icon.GroupSearchFields, DialogSimpleForm.Components.IconColor.Yellow);
        }
    }
}
