using Incas.Users.Models;
using Incas.Users.Views.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incubator_2.Forms.AdminPanel
{
    /// <summary>
    /// Логика взаимодействия для UsersManager.xaml
    /// </summary>
    public partial class UsersManager : UserControl
    {
        public UsersManager()
        {
            InitializeComponent();
            this.FillUsersList();
        }
        public void FillUsersList()
        {
            this.UsersList.Children.Clear();
            using (User u = new())
            {
                u.GetAllUsers().ForEach(u =>
                {
                    UserElement element = new(u);
                    element.OnDeleted += this.RemoveElement;
                    this.UsersList.Children.Add(element);

                });
            }
        }
        private void RemoveElement(UserElement element)
        {
            this.UsersList.Children.Remove(element);
        }

        private void RefreshClick(object sender, MouseButtonEventArgs e)
        {
            this.FillUsersList();
        }

        private void AddClick(object sender, MouseButtonEventArgs e)
        {
            User user = new();
            UserEditor editor = new(user);
            editor.ShowDialog();
            this.FillUsersList();
        }
    }
}
