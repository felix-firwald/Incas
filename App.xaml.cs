using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using System;
using System.Windows;

namespace Incas
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>

    public partial class App : System.Windows.Application
    {
        public static DateTime ExpirationDate = new(2024, 12, 12);
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (DateTime.Now > ExpirationDate)
            {
                DialogsManager.ShowErrorDialog("Истек предельный срок использования этой демонстрационной версии. Обновите программу.", "Лицензия истекла");
                App.Current.Shutdown();
            }
            ProgramState.CheckoutWorkspaces();

            OpenWorkspace ow = new();
            if (ow.ShowDialog() == false)
            {

            }
        }

        private void Unhandled(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                ProgramState.CloseSession();
            }
            catch (Exception)
            {

            }
            ProgramState.OpenWebPage($"https://forms.yandex.ru/cloud/66fa578c505690e91c121ddc/" + "?version=" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + "&description=" + e.Exception.Message);
            DialogsManager.ShowErrorDialog($"Возникла ошибка, не позволяющая приложению продолжать свою работу.\n" +
                $"Описание: {e.Exception.Message}\nПриложение будет немедленно закрыто.", "Критическая ошибка");
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            ProgramState.CloseSession();
        }
    }
}
