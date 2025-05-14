using IncasEngine.Backups;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Incas.Core.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для CriticalError.xaml
    /// </summary>
    public partial class CriticalError : Window
    {
        private string logFile = "";
        public CriticalError(Exception except)
        {
            this.InitializeComponent();
            this.logFile = BackupProcessor.WriteBackup(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(), except);
            this.Description.Text = $"Возникла ошибка, не позволяющая INCAS продолжать свою работу.\n" +
                        $"Описание: {except.Message}\nПриложение будет немедленно закрыто.";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OpenLogClick(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", string.Format("/select,\"{0}\"", this.logFile));
        }
    }
}
