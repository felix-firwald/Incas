using Incubator_2.ViewModels;
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
            try
            {
                OpenIncubator oi = new OpenIncubator();
                
                
                if (oi.ShowDialog() == false)
                {
                    //Current.Shutdown();
                }
                //MV_MainWindow context = new MV_MainWindow();
                //context.LoadInfo();
                MainWindow mw = new MainWindow();
                this.MainWindow = mw;
                
                //mw.LoadBio();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox
                    .Show($"В инкубаторе возникли неполадки:\n\n{ex.Message}\n\nПрограмма будет закрыта.",
                        "Критическая ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
            }
            
            //this.MainWindow.Show();

        }
    }
}
