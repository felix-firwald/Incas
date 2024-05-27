using Common;
using Incas.Templates.Models;
using Incas.Templates.ViewModels;
using IncasEngine.TemplateManager;
using System.Windows;

namespace Incubator_2.Windows.Selectors
{
    /// <summary>
    /// Логика взаимодействия для TemplateSelector.xaml
    /// </summary>
    public partial class TemplateSelector : Window
    {
        private TemplateSelectorViewModel vm;
        public STemplate SelectedTemplate
        {
            get
            {
                return this.vm.SelectedTemplate;
            }
        }
        public TemplateSelector(TemplateType type, string helpText)
        {
            this.InitializeComponent();
            this.vm = new();
            this.DataContext = this.vm;
            this.vm.HelpTextTitle = helpText;
            this.vm.TemplateType = type;
        }

        private void SelectClick(object sender, RoutedEventArgs e)
        {
            if (this.vm.SelectedTemplate.id != 0)
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
