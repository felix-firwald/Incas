using Common;
using Incubator_2.Common;
using Incubator_2.Models;
using Incubator_2.Windows.Templates;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Incubator_2.Forms.Templates
{
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
                Result = JsonConvert.DeserializeObject<SGeneratedDocument>(data);
                SetWarning("Требуется открыть для обновления");
            }
            catch { }
        }
        public void SetData(SGeneratedDocument data, string warning = "Требуется открыть для обновления")
        {
            try
            {
                Result = data;
                SetWarning(warning);
                OnValueChanged?.Invoke(this);
            }
            catch (Exception)
            {
                SetNotContented();
            }
        }

        public string GetText()
        {
            if (string.IsNullOrWhiteSpace(resultText))
            {
                return "";
            }
            return resultText;
        }
        public string GetData()
        {
            return JsonConvert.SerializeObject(Result);
        }
        private void ApplyGenerated(SGeneratedDocument data)
        {
            Result = data;            
            SetContented();
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
                    SetNotContented();
                    break;
                default:
                    Session s = ProgramState.ShowActiveUserSelector("Выберите пользователя для заполнения этой части документа.");
                    if (s.userId != 0)
                    {
                        Result.template = TemplateId;
                        ServerProcessor.SendOpenGeneratorProcess(Result, this, s.slug);
                        SetInProcess($"Делегировано: {s.user}");
                    }
                    break;
            }
        }

        private void OpenClick(object sender, MouseButtonEventArgs e)
        {
            using (Template t = new())
            {
                UseTemplateText utt = new(t.GetTemplateById(TemplateId), Result);
                utt.ShowDialog();
                try
                {
                    if (utt.Result == Windows.DialogStatus.Yes)
                    {
                        Result = utt.GetData();
                        resultText = utt.GetText();
                        SetContented();
                    }
                    
                }
                catch (Exception ex)
                {
                    ProgramState.ShowErrorDialog(ex.Message);
                }
            }
        }

        private void ShowTextClick(object sender, MouseButtonEventArgs e)
        {
            this.TextBoxResult.Text = resultText;
            this.PopupText.IsOpen = !this.PopupText.IsOpen;
        }

        private void CopyResultClick(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(resultText);
        }
    }
}
