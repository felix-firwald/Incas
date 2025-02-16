using Incas.Objects.ViewModels;
using IncasEngine.ObjectiveEngine.Models;
using System.Windows.Controls;

namespace Incas.Objects.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для FieldScriptViewer.xaml
    /// </summary>
    public partial class FieldScriptViewer : UserControl
    {
        public FieldViewModel vm;
        public delegate void FieldScriptAction(Field field);
        public event FieldScriptAction OnLinkInsertingRequested;
        public event FieldScriptAction OnBindingEventRequested;
        public event FieldScriptAction OnBindingActionRequested;
        public FieldScriptViewer(FieldViewModel vm)
        {
            this.InitializeComponent();
            this.vm = vm;
            this.DataContext = vm;
        }

        private void InsertLinkClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.OnLinkInsertingRequested?.Invoke(this.vm.Source);
        }

        private void InsertEventClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //this.vm.Source.ChangedEvent = $"{this.vm.Source.Name}_changed";
            this.OnBindingEventRequested?.Invoke(this.vm.Source);
        }

        private void InsertActionClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.OnBindingActionRequested?.Invoke(this.vm.Source);
        }
    }
}
