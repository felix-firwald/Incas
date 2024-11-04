using Incas.Admin.Views.Controls;
using Incas.Core.Views.Controls;
using Incas.Users.Models;
using System;
using System.Windows.Controls;

namespace Incas.Admin.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для SessionsManager.xaml
    /// </summary>
    public partial class SessionsManager : UserControl
    {
        public SessionsManager()
        {
            this.InitializeComponent();
            this.FillContentPanel();
        }
        public void FillContentPanel()
        {
            this.SessionsList.Children.Clear();
            //using (Session s = new())
            //{
            //    s.GetAllSessions().ForEach(session =>
            //    {
            //        SessionElement element = new(session);
            //        this.SessionsList.Children.Add(element);
            //    });
            //}
            this.SessionsList.Children.Add(new NoContent());
        }

        private void RefreshClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.FillContentPanel();
        }
    }
}
