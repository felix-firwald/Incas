using Incas.Admin.ViewModels;
using IncasEngine.Models;
using System.Windows;

namespace Incas.Admin.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для ConverterEditor.xaml
    /// </summary>
    public partial class ConverterEditor : Window
    {
        private ConverterViewModel vm { get; set; }
        public ConverterEditor() // if new
        {
            this.InitializeComponent();
            this.Title = "Создание конвертера";
            this.vm = new();
            this.DataContext = this.vm;
        }
        public ConverterEditor(ConverterItem item) // if edit
        {
            this.InitializeComponent();
            this.Title = "Редактирование конвертера";
            this.vm = new(new(item));
            this.DataContext = this.vm;
        }
    }
}
