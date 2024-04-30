using Common;
using Incubator_2.Common;
using Incubator_2.Models;
using Incubator_2.Windows.Templates;
using Models;
using Newtonsoft.Json;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incubator_2.Forms.Templates
{
    public class GeneratorUndefinedStateException : Exception
    {
        public GeneratorUndefinedStateException(string message) : base(message)
        {

        }
    }
    public enum GeneratorStatus
    {
        NotContented,
        Contented,
        Warning,
        InProcess
    }
    public class NotReadyException : Exception
    {
        public NotReadyException(string message)
        : base(message) { }
    }

    /// <summary>
    /// Логика взаимодействия для Generator.xaml
    /// </summary>
    public partial class Generator : UserControl
    {
        public GeneratorStatus Status = GeneratorStatus.NotContented;
        public int TemplateId;
        public SGeneratedDocument Result;
        private string resultText;
        public delegate void ValueChanged(object sender);
        public event ValueChanged OnValueChanged;
        public Generator() // new
        {
            InitializeComponent();
        }
        public void SetData(string data)
        {
            try
            {
                this.Result = JsonConvert.DeserializeObject<SGeneratedDocument>(data);
                this.SetWarning("Требуется открыть для обновления");
            }
            catch { }
        }
        public void SetData(SGeneratedDocument data, string warning = "Требуется открыть для обновления")
        {
            try
            {
                this.Result = data;
                this.SetWarning(warning);
                OnValueChanged?.Invoke(this);
            }
            catch (Exception)
            {
                this.SetNotContented();
            }
        }

        public string GetText()
        {
            switch (this.Status)
            {
                case GeneratorStatus.InProcess:
                    throw new GeneratorUndefinedStateException("Генератор ожидает получение ввода от другого пользователя. " +
                        "Отзовите делегирование или дождитесь ввода.");
                case GeneratorStatus.Warning:
                    using (Template t = new())
                    {
                        UseTemplateText utt = new(t.GetTemplateById(this.TemplateId), this.Result);
                        try
                        {
                            this.Result = utt.GetData();
                            this.resultText = utt.GetText();
                            this.SetContented();
                        }
                        catch (Exception)
                        {
                            throw new GeneratorUndefinedStateException("Генератор требует подтверждения данных.");
                        }
                    }
                    break;
            }
            if (string.IsNullOrWhiteSpace(this.resultText))
            {
                return "";
            }
            return this.resultText;
        }
        public string GetData()
        {
            return JsonConvert.SerializeObject(this.Result);
        }
        private void ApplyGenerated(SGeneratedDocument data)
        {
            this.Result = data;
            this.SetContented();
        }
        private void SetContented()
        {
            this.Status = GeneratorStatus.Contented;
            this.IconUser.ToolTip = "Делегировать другому пользователю";
            this.NotContented.Visibility = Visibility.Collapsed;
            this.Warning.Visibility = Visibility.Collapsed;
            this.InProcess.Visibility = Visibility.Collapsed;
            this.Contented.Visibility = Visibility.Visible;
            this.OpenButton.IsEnabled = true;
            OnValueChanged?.Invoke(this);
        }
        private void SetNotContented()
        {
            this.Status = GeneratorStatus.NotContented;
            this.IconUser.ToolTip = "Делегировать другому пользователю";
            this.Contented.Visibility = Visibility.Collapsed;
            this.Warning.Visibility = Visibility.Collapsed;
            this.InProcess.Visibility = Visibility.Collapsed;
            this.NotContented.Visibility = Visibility.Visible;
            this.OpenButton.IsEnabled = true;
        }
        private void SetWarning(string text)
        {
            this.Status = GeneratorStatus.Warning;
            this.IconUser.ToolTip = "Делегировать другому пользователю";
            this.NotContented.Visibility = Visibility.Collapsed;
            this.Contented.Visibility = Visibility.Collapsed;
            this.WarningText.Text = text;
            this.InProcess.Visibility = Visibility.Collapsed;
            this.Warning.Visibility = Visibility.Visible;
            this.OpenButton.IsEnabled = true;
        }
        private void SetInProcess(string text)
        {
            this.Status = GeneratorStatus.InProcess;
            this.NotContented.Visibility = Visibility.Collapsed;
            this.Contented.Visibility = Visibility.Collapsed;
            this.Warning.Visibility = Visibility.Collapsed;
            this.ProcessText.Text = text;
            this.InProcess.Visibility = Visibility.Visible;
            this.OpenButton.IsEnabled = false;
            this.IconUser.ToolTip = "Отозвать делегацию";
        }
        private void SendToUserClick(object sender, MouseButtonEventArgs e)
        {
            switch (this.Status)
            {
                case GeneratorStatus.InProcess:
                    WaitControls.RemoveGenerator(this);
                    this.SetNotContented();
                    break;
                default:
                    Session session;
                    if (ProgramState.ShowActiveUserSelector(out session, "Выберите пользователя для заполнения этой части документа."))
                    {
                        this.Result.template = this.TemplateId;
                        ServerProcessor.SendOpenGeneratorProcess(this.Result, this, session.slug);
                        this.SetInProcess($"Делегировано: {session.user}");
                    }
                    break;
            }
        }

        private void OpenClick(object sender, MouseButtonEventArgs e)
        {
            using Template t = new();
            UseTemplateText utt = new(t.GetTemplateById(this.TemplateId), this.Result);
            utt.ShowDialog();
            try
            {
                if (utt.Result == Windows.DialogStatus.Yes)
                {
                    this.Result = utt.GetData();
                    this.resultText = utt.GetText();
                    this.SetContented();
                }

            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog(ex.Message);
            }
        }

        private void ShowTextClick(object sender, MouseButtonEventArgs e)
        {
            this.TextBoxResult.Text = this.resultText;
            this.PopupText.IsOpen = !this.PopupText.IsOpen;
        }

        private void CopyResultClick(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(this.resultText);
        }
    }
}
