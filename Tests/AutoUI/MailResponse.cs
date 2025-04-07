using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.ComponentModel;

namespace Incas.Tests.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для MailResponse.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class MailResponse : AutoUIBase
    {
        protected override string FinishButtonText => "Отправить письмо";
        #region Data
        [Description("Отправитель")]
        public string FromAddress { get; set; }

        [Description("Пароль отправителя")]
        public string Password { get; set; }

        [Description("Получатель")]
        public string ToAddress { get; set; }

        [Description("Тема")]
        public string Subject { get; set; }

        [Description("Содержание")]
        public string Body { get; set; }

        [Description("SmtpServer")]
        public string SmtpServer { get; set; }

        [Description("SmtpPort")]
        public int SmtpPort { get; set; }
        #endregion

        public MailResponse()
        {
            this.FromAddress = "dekanat.uf.rggu@yandex.ru"; // Замените на ваш email
            this.Password = "ccprdbatzihqcaxd"; // Замените на ваш пароль
            this.ToAddress = "dimamagon@gmail.com"; // Замените на email получателя
            this.Subject = "Тестирование INCAS";
            this.Body = "Добрый день, это проверочное письмо.";
        }

        #region Functionality
        public override void Load()
        {

        }

        public override void Validate()
        {

        }

        public override void Save()
        {
            this.SendEmailWithMailKit();
        }
        public void SendEmailWithMailKit()
        {
            try
            {
                // Создаем объект сообщения
                MimeMessage message = new();
                message.From.Add(new MailboxAddress("Деканат (каганат)", this.FromAddress)); // Имя отправителя можно указать здесь (пустая строка - без имени)
                message.To.Add(new MailboxAddress("", this.ToAddress)); // Имя получателя можно указать здесь
                message.Subject = this.Subject;

                // Создаем тело письма (текст или HTML)
                message.Body = new TextPart("html") { Text = this.Body }; // Или TextPart("plain") для обычного текста

                // Создаем SMTP клиент
                using SmtpClient client = new();

                // Подключаемся к SMTP серверу
                client.Connect(this.SmtpServer, this.SmtpPort, true);

                // Аутентифицируемся
                client.Authenticate(this.FromAddress, this.Password);

                // Отправляем сообщение
                client.Send(message);

                // Отключаемся
                client.Disconnect(true);
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
            }
        }

        #endregion
    }
}
