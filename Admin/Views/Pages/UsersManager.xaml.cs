using Incas.Core.Classes;
using Incas.Users.AutoUI;
using Incas.Users.Models;
using Incas.Users.Views.Controls;
using Incas.Users.Views.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incas.Admin.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для UsersManager.xaml
    /// </summary>
    public partial class UsersManager : UserControl
    {
        public UsersManager()
        {
            this.InitializeComponent();
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
            UserSettings us = new(user);
            us.ShowDialog("Создание пользователя", Icon.UserPlus);
            this.FillUsersList();
        }
    }
}
