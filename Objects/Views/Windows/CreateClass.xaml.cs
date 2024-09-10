using Incas.Core.Classes;
using Incas.Objects.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace Incas.Objects.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateClass.xaml
    /// </summary>
    public partial class CreateClass : Window
    {
        ClassViewModel vm;
        public CreateClass()
        {
            this.InitializeComponent();
            this.vm = new(new());
            this.DataContext = this.vm;
        }

        private void GetMoreInfoClick(object sender, MouseButtonEventArgs e)
        {
            ProgramState.OpenWebPage("https://teletype.in/@incas/classes");
        }

        private void AddFieldClick(object sender, MouseButtonEventArgs e)
        {
            Incas.Objects.Views.Controls.FieldCreator fc = new();
            this.ContentPanel.Children.Add(fc);
        }
    }
}
