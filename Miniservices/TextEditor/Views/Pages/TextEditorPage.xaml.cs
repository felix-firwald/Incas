using System.Windows.Controls;
using Incas.Core.Classes;
using Incas.Miniservices.TextEditor.ViewModels;

namespace Incas.Miniservices.TextEditor.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для TextEditorPage.xaml
    /// </summary>
    public partial class TextEditorPage : UserControl
    {
        public TextEditorViewModel vm;
        public TextEditorPage()
        {
            this.InitializeComponent();
            this.vm = new();
            this.DataContext = this.vm;
        }

        private void CopyClick(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                System.Windows.Clipboard.SetText(this.vm.ResultText);
            }
            catch (System.Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
            }
        }

        private void ApplyClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.vm.SourceText = this.vm.ResultText;
        }
    }
}
