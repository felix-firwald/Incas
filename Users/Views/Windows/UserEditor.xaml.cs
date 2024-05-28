using Incas.Core.Classes;
using Incas.Users.Models;
using Incas.Users.ViewModels;

using System.Windows;

namespace Incas.Users.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для UserEditor.xaml
    /// </summary>
    public partial class UserEditor : Window
    {
        private UserEditorViewModel vm;
        public UserEditor(User user)
        {
            this.InitializeComponent();
            this.vm = new(user);
            this.DataContext = this.vm;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (this.vm.Save() == true)
            {
                this.Close();
            }
            else
            {
                ProgramState.ShowExclamationDialog("Одно или несколько обязательных полей не заполнены.", "Сохранение невозможно");
            }
        }
    }
}
