using Incas.Core.Classes;

using System.Windows;
using System.Windows.Input;

namespace Incas.Objects.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateClass.xaml
    /// </summary>
    public partial class CreateClass : Window
    {
        public CreateClass()
        {
            this.InitializeComponent();
        }

        private void GetMoreInfoClick(object sender, MouseButtonEventArgs e)
        {
            ProgramState.OpenWebPage("https://teletype.in/@incas/classes");
        }

        private void AddFieldClick(object sender, MouseButtonEventArgs e)
        {
            this.ContentPanel.Children.Add(new Incas.Objects.Views.Controls.FieldCreator());
        }
    }
}
