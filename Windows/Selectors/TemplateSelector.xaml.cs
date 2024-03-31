using Common;
using Incubator_2.ViewModels.Selectors;
using Models;
using System.Windows;

namespace Incubator_2.Windows.Selectors
{
    /// <summary>
    /// Логика взаимодействия для TemplateSelector.xaml
    /// </summary>
    public partial class TemplateSelector : Window
    {
        VM_TemplateSelector vm;
        public STemplate SelectedTemplate
        {
            get
            {
                return vm.SelectedTemplate;
            }
        }
        public TemplateSelector(TemplateType type, string helpText)
        {
            InitializeComponent();
            this.vm = new();
            this.DataContext = vm;
            this.vm.HelpTextTitle = helpText;
            this.vm.TemplateType = type;
        }

        private void SelectClick(object sender, RoutedEventArgs e)
        {
            if (vm.SelectedTemplate.id != 0)
            {
                this.Close();
            }
            else
            {
                ProgramState.ShowExclamationDialog("Шаблон не выбран!", "Действие невозможно");
            }
        }
        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
