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
using System.Windows.Shapes;

namespace Incubator_2.Windows
{
    public enum DialogType
    {
        Error,
        Warning,
        Information,
        Question
    } 
    /// <summary>
    /// Логика взаимодействия для Error.xaml
    /// </summary>
    public partial class Dialog : Window
    {
        private DialogType typeDialog;
        private Dictionary<DialogType, string> icons = new Dictionary<DialogType, string> {
            { DialogType.Error, "ExclamationCircle" },
            { DialogType.Warning, "ExclamationTriangle" },
            { DialogType.Information, "InfoCircle" },
            { DialogType.Question, "QuestionCircle" }
        };
        public Dialog(string text, string title="Неизвестная ошибка", DialogType dialog=DialogType.Error)
        {
            InitializeComponent();
            this.Title.Content = title;
            this.Description.Text = text;
            this.typeDialog = dialog;
            switch (dialog)
            {
                case DialogType.Warning:
                    SetAsWarning();
                    break;
                case DialogType.Information:
                    SetAsInformation();
                    break;
                default:
                    break;
            }
        }
        private void SetAsWarning()
        {
            
        }
        private void SetAsInformation()
        {

        }
    }
}
