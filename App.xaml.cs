using Common;
using Incubator_2.Models;
using Incubator_2.Windows;
using System;
using System.Windows;

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
                
                if (DateTime.Now > DateTime.Parse("24.05.2024"))
                {
                    ProgramState.ShowErrorDialog("Истек предельный срок для лицензии этой версии. Обновите программу.", "Лицензия истекла");
                    App.Current.Shutdown();
                }
                OpenIncubator oi = new OpenIncubator();
                
                if (oi.ShowDialog() == false)
                {
                    
                }
            }
            catch (DBParamNotFound p)
            {
                Dialog d = new Dialog($"Параметр с именем \"{p}\" не найден в базе данных рабочего пространства.\n" +
                    $"Вероятно, имело место быть ручное вмешательство в файл data.dbinc, в результате чего " +
                    $"параметр был удален.\nДальнейшее использование рабочей области находится под угрозой.", "Возникла критическая ошибка");
                d.ShowDialog();               
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
            ProgramState.ShowErrorDialog($"Возникла ошибка, не позволяющая приложению продолжать свою работу.\n" +
                $"Описание: {e.Exception.Message}\nПриложение будет немедленно закрыто.", "Критическая ошибка");
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            ProgramState.CloseSession();
        }
    }
}
