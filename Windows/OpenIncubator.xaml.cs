using Common;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для OpenIncubator.xaml
    /// </summary>
    public partial class OpenIncubator : Window
    {
        public OpenIncubator()
        {
            InitializeComponent();
            ProgramState.LoadUserData();
            this.cpath.Text = ProgramState.CommonPath;
        }

        private void LogInClicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.cpath.Text) || string.IsNullOrEmpty(this.pwd.Text)) {
                System.Windows.Forms.MessageBox.Show("Одно из полей пустое!", "Ошибка");
            }
            else
            {
                ProgramState.SetCommonPath(this.cpath.Text);
                ProgramState.GetInitFile();
                ProgramState.SaveUserData();
                DialogResult = true;
                if (Directory.Exists(this.cpath.Text))
                {
                    CheckPassword();
                }
                
                this.Close();
            }
            
        }

        private void CheckPassword()
        {
            using (User user = new User())
            {
                if (user.IsPasswordExists(this.pwd.Text))
                {
                    ProgramState.User = user.username;
                    Permission.CurrentUserPermission = user.status;
                    this.Close();
                }
                else
                {
                    System.Windows.MessageBox.Show("Пароль введен неверно.", "Неправильный пароль");
                    this.pwd.Text = "";
                }
            }
        }

        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //System.Windows.Application.Current.Shutdown();
        }

        private void PathReview(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.cpath.Text = folderDialog.SelectedPath;
            }
        }
    }
}
