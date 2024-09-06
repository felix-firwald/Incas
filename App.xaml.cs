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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (DateTime.Now > DateTime.Parse("21.10.2024"))
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
            ProgramState.OpenWebPage($"https://forms.yandex.ru/u/66d1d24f90fa7b1262d187e7/" + "?version=" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + "&description=" + e.Exception.Message);
            DialogsManager.ShowErrorDialog($"Возникла ошибка, не позволяющая приложению продолжать свою работу.\n" +
                $"Описание: {e.Exception.Message}\nПриложение будет немедленно закрыто.", "Критическая ошибка");
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            ProgramState.CloseSession();
        }
    }
}
