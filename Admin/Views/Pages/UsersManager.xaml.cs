using Incas.Core.Classes;
using Incas.Core.Interfaces;
using Incas.Users.AutoUI;
using Incas.Users.Models;
using Incas.Users.Views.Controls;
using System.Windows.Controls;
using System.Windows.Input;
using static Incas.Core.Interfaces.ITabItem;

namespace Incas.Admin.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для UsersManager.xaml
    /// </summary>
    public partial class UsersManager : UserControl, ITabItem
    {
        public event TabAction OnClose;
        public string Id { get; set; }
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
        public void FillGroupsList()
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

        private void AddClick(object sender, System.Windows.RoutedEventArgs e)
        {
            User user = new();
            UserSettings us = new(user);
            us.ShowDialog("Создание пользователя", Icon.UserPlus);
            this.FillUsersList();
        }

        private void UpdateClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.FillUsersList();
        }
        private void AddGroupClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Group group = new();
            GroupSettings us = new(group);
            if (us.ShowDialog("Создание группы", Icon.UserPlus))
            {
                group.Save();
                this.FillGroupsList();
            }
        }

        private void UpdateGroupsClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.FillGroupsList();
        }
    }
}
