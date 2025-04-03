using Incas.Core.Classes;
using System.Windows;

namespace Incas.Core.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для AboutIncas.xaml
    /// </summary>
    public partial class AboutIncas : Window
    {
        public AboutIncas()
        {
            this.InitializeComponent();
            this.VersionText.Text = $"Редакция {ProgramState.Edition}, версия {ProgramState.Version}";
        }
    }
}
