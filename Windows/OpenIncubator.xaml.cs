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
                MV_MainWindow context = new MV_MainWindow();
                context.LoadInfo();
                MainWindow mw = new MainWindow();
                mw.Show();
                //this.Hide();
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
