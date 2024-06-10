using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using System;
using System.Windows;
using WebSupergoo.WordGlue3;

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
            this.SetLicenses();
            //this.DoSomething();
            if (DateTime.Now > DateTime.Parse("21.08.2024"))
            {
                DialogsManager.ShowErrorDialog("Истек предельный срок использования этой демонстрационной версии. Обновите программу.", "Лицензия истекла");
                App.Current.Shutdown();
            }
            OpenWorkspace ow = new();

            if (ow.ShowDialog() == false)
            {

            }
        }
        private void SetLicenses()
        {
            //QuestPDF.Settings.License = LicenseType.Community;
        }
        private void DoSomething()
        {
            Doc doc = new("C:\\Users\\1\\Documents\\1 потом удалить\\привет.docx");
            doc.SaveAs("C:\\Users\\1\\Documents\\1 потом удалить\\привет.pdf");
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
            DialogsManager.ShowErrorDialog($"Возникла ошибка, не позволяющая приложению продолжать свою работу.\n" +
                $"Описание: {e.Exception.Message}\nПриложение будет немедленно закрыто.", "Критическая ошибка");
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            ProgramState.CloseSession();
        }
    }
}
