using Incas.Admin.ViewModels;
using IncasEngine.Models;
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
    }
}
