using Common;
using Incubator_2.ViewModels.VMAdmin;
using Models;

using System.Windows;


namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для UserEditor.xaml
    /// </summary>
    public partial class UserEditor : Window
    {
        private VM_UserEditor vm;
        public UserEditor(User user)
        {
            InitializeComponent();
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
