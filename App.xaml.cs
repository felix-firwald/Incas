using Incubator_2.Models;
using Incubator_2.Windows;
using Incubator_2.Windows.ToolBar;
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
                //if (TestFunc())
                //{

                //}
                
                OpenIncubator oi = new OpenIncubator();
                
                if (oi.ShowDialog() == false)
                {
                    //Current.Shutdown();
                }
                
                //MainWindow mw = new MainWindow();
                //this.MainWindow = mw;
            }
            catch (DBParamNotFound p)
            {
                Dialog d = new Dialog($"Параметр с именем \"{p}\" не найден в базе данных инкубатора.\n" +
                    $"Вероятно, имело место быть ручное вмешательство в файл data.dbinc, в результате чего " +
                    $"параметр был удален.\nДальнейшее использование рабочей области находится под угрозой.", "Возникла критическая ошибка");
                d.ShowDialog();
                App.Current.Shutdown();
            }
            catch (Exception ex)
            {
                Dialog d = new Dialog($"{ex.Message}\nПрограмма будет немедленно закрыта.", "Возникла критическая ошибка");
                d.ShowDialog();
                App.Current.Shutdown();
            }

        }

        private bool TestFunc()
        {
            DialogQuestion test = new DialogQuestion("Инкубатор не найден по указанному пути", "Создать новое рабочее пространство?", "Создать", "Закрыть программу");
            test.ShowDialog();
            return true;
        }
    }
}
