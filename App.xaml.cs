using Incubator_2.Windows;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Incubator_2
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            OpenIncubator oi = new OpenIncubator();
            this.MainWindow = new MainWindow();
            if (oi.ShowDialog() == false)
            {
                Current.Shutdown();
            }
            
            //this.MainWindow.Show();

        }
    }
}
