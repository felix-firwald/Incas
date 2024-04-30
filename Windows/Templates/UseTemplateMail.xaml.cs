using Incubator_2.Common;
using Incubator_2.Models;
using Models;
using System.Windows;

namespace Incubator_2.Windows.Templates
{
    /// <summary>
    /// Логика взаимодействия для UseTemplateMail.xaml
    /// </summary>
    public partial class UseTemplateMail : Window
    {
        public UseTemplateMail(Template template)
        {
            InitializeComponent();
        }

        private void SendClick(object sender, RoutedEventArgs e)
        {
            MailProfile profile = new MailProfile();
            profile.host = "smtp.gmail.com";
            profile.port = 587;
            profile.name = "Деканат ЮФ РГГУ";
            profile.email = "dekanat.uf.rggu@gmail.com";
            profile.password = "dekanat3690";
#pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
            MailService.SendEmailAsync("dimamagon@gmail.com", "Проверка связи", "Проверяем связь!", profile);
#pragma warning restore CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
        }
    }
}
