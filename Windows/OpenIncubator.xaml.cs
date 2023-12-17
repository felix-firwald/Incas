using Common;
using Incubator_2.ViewModels;
using Models;
using System.IO;
using System.Windows;
using System.Windows.Forms;

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
                Dialog d = new Dialog("Одно из обязательных полей не заполнено!", "Вход невозможен");
                d.ShowDialog();
            }
            else
            {
                ProgramState.SetCommonPath(this.cpath.Text);
                ProgramState.GetInitFile();
                ProgramState.SaveUserData();
                
                if (Directory.Exists(this.cpath.Text))
                {
                    CheckPassword();
                }

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
                    DialogResult = true;
                    this.Close();
                }
                else
                {
                    Dialog d = new Dialog("Пароль введен неверно (пользователь с таким паролем не найден в рабочем пространстве).", "Вход невозможен");
                    d.ShowDialog();
                    this.pwd.Text = "";
                }
            }
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
