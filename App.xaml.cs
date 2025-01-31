using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using Microsoft.VisualBasic.Devices;
using Newtonsoft.Json;
using System;
using System.Windows;

namespace Incas
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>

    public partial class App : System.Windows.Application
    {
        private const string FormError = "https://forms.yandex.ru/cloud/66fa578c505690e91c121ddc/";
        private bool criticalErrorOccured = false;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ProgramState.CheckUMIExists();
            ProgramState.InitDocumentsFolder();
            if (!ProgramState.CheckLicense())
            {
                License.Views.Windows.LicenseDialog ld = new();
                if (ld.ShowDialog() != true)
                {
                    System.Windows.Application.Current.Shutdown();
                }
            }
            ProgramState.CheckoutWorkspaces();
            
            OpenWorkspace ow = new();
            if (ow.ShowDialog() == true)
            {
                //Core.Views.Windows.MainWindow mw = new Core.Views.Windows.MainWindow();
                //mw.Show();
            }
        }
        private void ShowCI()
        {
            ComputerInfo ci = new();
            string info = JsonConvert.SerializeObject(ci, Formatting.Indented);
            DialogsManager.ShowInfoDialog(info);
            DialogsManager.ShowInfoDialog(Environment.MachineName);
            DialogsManager.ShowInfoDialog(Environment.UserDomainName);
        }

        private void Unhandled(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (!this.criticalErrorOccured)
            {
                this.criticalErrorOccured = true;
                ProgramState.OpenWebPage(FormError + "?version=" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + "&description=" + e.Exception.Message);
                DialogsManager.ShowErrorDialog($"Возникла ошибка, не позволяющая INCAS продолжать свою работу.\n" +
                    $"Описание: {e.Exception.Message}\nПриложение будет немедленно закрыто.", "Критическая ошибка");
                this.Shutdown();
            }           
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            
        }
    }
}
