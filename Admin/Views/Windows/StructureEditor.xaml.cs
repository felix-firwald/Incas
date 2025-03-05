using Incas.Admin.ViewModels;
using IncasEngine.Models;
using System.Windows;

namespace Incas.Admin.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для StructureEditor.xaml
    /// </summary>
    public partial class StructureEditor : Window
    {
        private StructureViewModel vm { get; set; }
        public StructureEditor() // if new
        {
            this.InitializeComponent();
            this.Title = "Создание структуры";
            this.vm = new();
            this.DataContext = this.vm;
        }
        public StructureEditor(StructureItem item) // if edit
        {
            this.InitializeComponent();
            this.Title = "Редактирование структуры";
            this.vm = new(new(item));
            this.DataContext = this.vm;
        }

        private void AddFieldClick(object sender, RoutedEventArgs e)
        {

        }

        private void MinimizeAllClick(object sender, RoutedEventArgs e)
        {

        }

        private void MaximizeAllClick(object sender, RoutedEventArgs e)
        {

        }

        private void ShowFormClick(object sender, RoutedEventArgs e)
        {

        }

        private void GetMoreInfoClick(object sender, RoutedEventArgs e)
        {

        }

        private void AddMethod(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveMethod(object sender, RoutedEventArgs e)
        {

        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
