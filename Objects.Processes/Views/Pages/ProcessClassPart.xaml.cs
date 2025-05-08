using Incas.Objects.Interfaces;
using Incas.Objects.Processes.AutoUI;
using Incas.Objects.Processes.ViewModels;
using Incas.Objects.ViewModels;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using System.Windows.Controls;

namespace Incas.Objects.Processes.Views.Pages
{
    /// <summary>
    /// CLASS SETTINGS ONLY! Логика взаимодействия для ProcessClassPart.xaml
    /// </summary>
    public partial class ProcessClassPart : UserControl, IClassPartSettings
    {

        public event IClassPartSettings.OpenAdditionalSettings OnAdditionalSettingsOpenRequested;
        public string ItemName => "Настройка процессов";
        public ProcessPartViewModel vm { get; set; }
        public ProcessClassPart()
        {
#if !E_FREE
            this.InitializeComponent();
#endif
        }     

        public IClassPartSettings SetUp(IMembersContainerViewModel classViewModel)
        {
            this.vm = new(classViewModel as ClassViewModel);
            this.DataContext = this.vm;
            return this;
        }

        public void Save()
        {
            this.vm.Save();
        }

        private void AddDocument(object sender, System.Windows.RoutedEventArgs e)
        {
            DocumentSelectorAutoUI doc = new();
            if (doc.ShowDialog("Выбор модели документа"))
            {
                ClassItem item = doc.GetSelectedDocument();
                this.vm.AddDocument(item);
            }
        }

        private void RemoveDocument(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        public void Validate()
        {
            
        }
    }
}
