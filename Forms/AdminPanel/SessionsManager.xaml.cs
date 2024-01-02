
using Models;
using System.Threading;
using System.Windows;
using System.Windows.Controls;


namespace Incubator_2.Forms.AdminPanel
{
    /// <summary>
    /// Логика взаимодействия для SessionsManager.xaml
    /// </summary>
    public partial class SessionsManager : UserControl
    {
        public SessionsManager()
        {
            InitializeComponent();
            FillContentPanel();
        }
        public void FillContentPanel()
        {
            this.SessionsList.Children.Clear();
            using (Session s = new())
            {
                s.GetAllSessions().ForEach(session =>
                {
                    SessionElement element = new(session);
                    this.SessionsList.Children.Add(element);
                });
            }
        }

        private void RefreshClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FillContentPanel();
        }
    }
}
