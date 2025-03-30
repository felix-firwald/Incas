using Incas.Objects.Documents.ViewModels;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using System.Windows;

namespace Incas.Objects.Documents.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для PropertyReplicationSettings.xaml
    /// </summary>
    public partial class PropertyReplicationSettings : Window
    {
        public PropertyReplicationSettingsViewModel vm { get; set; }
        public PropertyReplicationSettings(TemplateProperty prop)
        {
            this.InitializeComponent();
            this.vm = new(prop);
            this.DataContext = this.vm;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (this.vm.Save())
            {
                this.Close();
            }           
        }
    }
}
