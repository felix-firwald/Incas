using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using Incas.Objects.Components;
using Incas.Templates.Components;
using Incas.Templates.Models;
using Incas.Templates.Views.Windows;
using Incas.Users.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incas.Templates.Views.Controls
{
    public class GeneratorUndefinedStateException : Exception
    {
        public GeneratorUndefinedStateException(string message) : base(message)
        {

        }
    }
    public enum GeneratorMode
    {
        OneForm,
        ManyForms
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
        public Guid TemplateId;
        public List<GeneratedElement> Result = [];
        private string resultText;
        private FieldType tagType;
        public delegate void ValueChanged(object sender);
        public event ValueChanged OnValueChanged;
        public Generator(FieldType type) // new
        {
            this.InitializeComponent();
            this.tagType = type;
        }
        public void SetData(string data)
        {
            try
            {
                this.Result = JsonConvert.DeserializeObject<List<GeneratedElement>>(data);
                this.SetWarning("Требуется открыть для обновления");
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
                this.Result = [];
            }
        }
        public void SetData(GeneratedElement data, string warning = "Требуется открыть для обновления")
        {
            try
            {
                this.Result = new()
                {
                    [0] = data
                };
                this.SetWarning(warning);
                OnValueChanged?.Invoke(this);
            }
            catch (Exception)
            {
                this.SetNotContented();
            }
        }
        public void SetData(List<GeneratedElement> data, string warning = "Требуется открыть для обновления")
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
            if (this.Result.Count == 0)
            {
                this.Result.Add(new());
            }
            switch (this.Status)
            {
                case GeneratorStatus.InProcess:
                    DialogsManager.ShowExclamationDialog("Генератор ожидает получение ввода от другого пользователя. " +
                        "В документе будет пустая строка.");
                    return "";
                case GeneratorStatus.Warning:
                    using (Template t = new())
                    {
                        switch (this.tagType)
                        {
                            case FieldType.Generator:
                                UseTemplateText utt = new(t.GetTemplateById(this.TemplateId), this.Result[0]);
                                try
                                {
                                    this.Result[0] = utt.GetData();
                                    this.resultText = utt.GetText();
                                    this.SetContented();
                                }
                                catch (Exception)
                                {
                                    return "";
                                }
                                break;
                        }
                    }
                    break;
            }
            return string.IsNullOrWhiteSpace(this.resultText) ? "" : this.resultText;
        }
        public string GetData()
        {
            return JsonConvert.SerializeObject(this.Result);
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
            if (this.TemplateId == Guid.Empty)
            {
                DialogsManager.ShowExclamationDialog("Шаблон не определен", "Действие прервано");
                return;
            }
            switch (this.Status)
            {
                case GeneratorStatus.InProcess:
                    WaitControls.RemoveGenerator(this);
                    this.SetNotContented();
                    break;
                default:
                    Session session;
                    if (DialogsManager.ShowActiveUserSelector(out session, "Выберите пользователя для заполнения этой части документа."))
                    {
                        GeneratedElement ge = this.Result[0];
                        ge.template = this.TemplateId;
                        this.Result[0] = ge;
                        ServerProcessor.SendOpenGeneratorProcess(this.Result, this, session.slug);
                        this.SetInProcess($"Делегировано: {session.user}");
                    }
                    break;
            }
        }

        private void OpenClick(object sender, MouseButtonEventArgs e)
        {
            if (this.TemplateId == Guid.Empty)
            {
                DialogsManager.ShowExclamationDialog("Шаблон не определен", "Действие прервано");
                return;
            }
            if (this.Result.Count == 0)
            {
                this.Result.Add(new GeneratedElement { });
            }
            using Template t = new();
            switch (this.tagType)
            {
                case FieldType.Generator:
                    UseTemplateText utt = new(t.GetTemplateById(this.TemplateId), this.Result[0]);
                    utt.ShowDialog();
                    try
                    {
                        if (utt.Result == DialogStatus.Yes)
                        {
                            this.Result[0] = utt.GetData();
                            this.resultText = utt.GetText();
                            this.SetContented();
                        }
                    }
                    catch (Exception ex)
                    {
                        DialogsManager.ShowErrorDialog(ex.Message);
                    }
                    break;
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
