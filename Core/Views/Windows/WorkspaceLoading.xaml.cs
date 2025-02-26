using System.Windows;

namespace Incas.Core.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для WorkspaceLoading.xaml
    /// </summary>
    public partial class WorkspaceLoading : Window
    {
        public WorkspaceLoading(string name)
        {
            this.InitializeComponent();
            this.WorkspaceName.Text = name;
        }
    }
}
