using Incas.Core.Classes;
using Incas.Miniservices.Clipboard.AutoUI;
using Incas.Miniservices.Clipboard.Classes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incas.Miniservices.Clipboard.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для ClipboardElement.xaml
    /// </summary>
    public partial class ClipboardElement : UserControl
    {
        private ClipboardRecord record;
        public delegate void UpdateRequested();
        public event UpdateRequested OnUpdateRequested;
        public delegate void TextClicked(string text);
        public event TextClicked OnClicked;
        public ClipboardElement(ClipboardRecord cr)
        {
            this.InitializeComponent();
            this.record = cr;
            this.RecordName.Content = cr.Name;
            this.Text.Text = cr.Text;
        }

        private void EditClick(object sender, MouseButtonEventArgs e)
        {
            this.record.ShowDialog("Редактирование записи");
            OnUpdateRequested?.Invoke();
        }

        private void RemoveClick(object sender, MouseButtonEventArgs e)
        {
            ClipboardManager.RemoveFromClipboard(this.record);
            OnUpdateRequested?.Invoke();
        }

        private void OnTextClicked(object sender, MouseButtonEventArgs e)
        {
            OnClicked?.Invoke(this.Text.Text);
        }
    }
}
