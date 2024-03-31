using Incubator_2.Windows;
using Models;
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
            FillUsersList();
        }
        public void FillUsersList()
        {
            this.UsersList.Children.Clear();
            using (User u = new())
            {
                u.GetAllUsers().ForEach(u =>
                {
                    UserElement element = new(u);
                    element.OnDeleted += RemoveElement;
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
            FillUsersList();
        }

        private void AddClick(object sender, MouseButtonEventArgs e)
        {
            User user = new();
            UserEditor editor = new(user);
            editor.ShowDialog();
            FillUsersList();
        }
    }
}
