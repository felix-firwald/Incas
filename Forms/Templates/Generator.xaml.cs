using Common;
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
    /// <summary>
    /// Логика взаимодействия для Generator.xaml
    /// </summary>
    public partial class Generator : UserControl
    {
        public int TemplateId;
        public SGeneratedDocument Result;
        private string resultText;
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
            this.NotContented.Visibility = Visibility.Collapsed;
            this.Warning.Visibility = Visibility.Collapsed;
            this.Contented.Visibility = Visibility.Visible;
        }
        private void SetNotContented()
        {
            this.Contented.Visibility = Visibility.Collapsed;
            this.Warning.Visibility = Visibility.Collapsed;
            this.NotContented.Visibility = Visibility.Visible;   
        }
        private void SetWarning(string text)
        {
            this.NotContented.Visibility = Visibility.Collapsed;
            this.Contented.Visibility = Visibility.Collapsed;
            this.WarningText.Text = text;
            this.Warning.Visibility = Visibility.Visible;
        }
        private void SendToUserClick(object sender, MouseButtonEventArgs e)
        {

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
    }
}
