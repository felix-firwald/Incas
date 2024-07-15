using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Windows;

namespace Incas
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public enum CheckEnum
    {
        [Description("Первый вариант")]
        One,
        [Description("Второй вариант")]
        Two,
        [Description("Третий вариант")]
        Three
    }
    public class CheckStruct
    {
        [Description("Наименование")]
        public string Name { get; set; }
        [Description("Список значений")]
        public DataTable Data { get; set; }
    }
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            if (DateTime.Now > DateTime.Parse("02.09.2024"))
            {
                DialogsManager.ShowErrorDialog("Истек предельный срок использования этой демонстрационной версии. Обновите программу.", "Лицензия истекла");
                App.Current.Shutdown();
            }
            //this.CheckForm();
            OpenWorkspace ow = new();
            if (ow.ShowDialog() == false)
            {

            }
        }

        private void CheckForm()
        {
            CheckStruct cs = new();
            cs.Data = new DataTable();
            cs.Data.Columns.Add("Значение");
            DialogsManager.ShowSimpleFormDialog(cs, "Назначение перечисления");
            DialogsManager.ShowSimpleFormDialog(cs, "Назначение перечисления");
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
