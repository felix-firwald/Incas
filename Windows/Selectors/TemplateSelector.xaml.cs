using Common;
using DocumentFormat.OpenXml.Wordprocessing;
using Incubator_2.ViewModels.Selectors;
using Models;
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
using System.Windows.Shapes;

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
