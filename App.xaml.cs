using Common;
using Incas.Core.Views.Windows;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
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
            this.SetLicenses();
            //this.DoSomething();
            if (DateTime.Now > DateTime.Parse("15.07.2024"))
            {
                ProgramState.ShowErrorDialog("Истек предельный срок использования этой демонстрационной версии. Обновите программу.", "Лицензия истекла");
                App.Current.Shutdown();
            }
            OpenWorkspace ow = new();

            if (ow.ShowDialog() == false)
            {

            }
        }
        private void SetLicenses()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }
        private void DoSomething()
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(20));

                    page.Header()
                        .Text("Hello PDF!")
                        .SemiBold().FontSize(36).FontFamily("Times New Roman").FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(20);

                            x.Item().Text("Привет меня зовут горгожопа!");
                            x.Item().Image(Placeholders.Image(200, 100));
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            })
            .GeneratePdf("C:\\Users\\1\\Documents\\1 потом удалить\\горгожопа.pdf");
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
