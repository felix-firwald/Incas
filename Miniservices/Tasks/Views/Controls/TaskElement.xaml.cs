using Incas.Miniservices.Clipboard.Classes;
using Incas.Miniservices.Tasks.AutoUI;
using System.Windows.Controls;
using static Incas.Miniservices.Clipboard.Views.Controls.ClipboardElement;
using System.Windows.Input;
using Incas.Miniservices.Tasks.Classes;
using Incas.Core.Classes;

namespace Incas.Miniservices.Tasks.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для TaskElement.xaml
    /// </summary>
    public partial class TaskElement : UserControl
    {
        public delegate void UpdateRequested();
        public event UpdateRequested OnUpdateRequested;
        private TaskRecord record;
        public TaskElement(TaskRecord tr)
        {
            this.InitializeComponent();
            this.record = tr;
            this.RecordName.Content = tr.Name;
            this.Text.Text = tr.Text;
            this.Deadline.Text = $"Напомнить: {tr.Deadline}";
        }
        private void EditClick(object sender, MouseButtonEventArgs e)
        {
            DialogsManager.ShowSimpleFormDialog(this.record, "Редактирование задачи");
            OnUpdateRequested?.Invoke();
        }

        private void RemoveClick(object sender, MouseButtonEventArgs e)
        {
            TaskManager.RemoveFromTaskboard(this.record);
            OnUpdateRequested?.Invoke();
        }
    }
}
