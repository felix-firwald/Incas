using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Incubator_2.Forms.Communication
{
    /// <summary>
    /// Логика взаимодействия для CommunicationMain.xaml
    /// </summary>
    public partial class CommunicationMain : UserControl
    {
        public CommunicationMain()
        {
            InitializeComponent();
            GetAllUsers();
        }
        public void GetAllUsers()
        {
            using (User us = new())
            {
                foreach (var item in us.GetAllUsers())
                {
                    this.Users.Items.Add(item.fullname);
                }
            }
        }

    }
}
