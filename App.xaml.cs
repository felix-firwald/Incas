using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using Incas.Templates.Components;
using Microsoft.VisualBasic.Devices;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;

namespace Incas
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>

    public partial class App : System.Windows.Application
    {
        private const string FormError = "https://forms.yandex.ru/cloud/66fa578c505690e91c121ddc/";
        public static DateTime ExpirationDate = new(2024, 12, 30);
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
            if (ow.ShowDialog() == false)
            {

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
            ProgramState.OpenWebPage(FormError + "?version=" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + "&description=" + e.Exception.Message);
            DialogsManager.ShowErrorDialog($"Возникла ошибка, не позволяющая приложению продолжать свою работу.\n" +
                $"Описание: {e.Exception.Message}\nПриложение будет немедленно закрыто.", "Критическая ошибка");
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            
        }
    }
}
