using Incubator_2.ViewModels.VM_CustomDB;
using System.Windows.Controls;

namespace Incubator_2.Forms.Database
{
    /// <summary>
    /// Логика взаимодействия для FieldEditor.xaml
    /// </summary>
    public partial class FieldEditor : UserControl
    {
        VM_FieldEditor vm;
        public FieldEditor()
        {
            InitializeComponent();
            vm = new();
            this.DataContext = vm;
        }
        public string GetDefinition()
        {
            return vm.GetDefinition();
        }
    }
}
