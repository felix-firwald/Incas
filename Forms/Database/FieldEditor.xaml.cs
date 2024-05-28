using Incas.CustomDatabases.ViewModels;
using System.Windows.Controls;

namespace Incubator_2.Forms.Database
{
    /// <summary>
    /// Логика взаимодействия для FieldEditor.xaml
    /// </summary>
    public partial class FieldEditor : UserControl
    {
        private FieldEditorViewModel vm;
        public FieldEditor()
        {
            InitializeComponent();
            this.vm = new();
            this.DataContext = this.vm;
        }
        public string GetDefinition()
        {
            return this.vm.GetDefinition();
        }
    }
}
