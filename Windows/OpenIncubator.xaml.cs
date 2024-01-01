using Common;
using Incubator_2.Common;
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
        VM_OpenWorkspace vm;
        public OpenIncubator()
        {
            InitializeComponent();
            ProgramState.LoadUserData();
            this.vm = new VM_OpenWorkspace();
            this.DataContext = this.vm;
            //this.cpath.Text = ProgramState.CommonPath;
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
                ProgramState.GetDBFile();
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
                    this.Visibility = Visibility.Hidden;
                    ProgramState.User = user.username;
                    Permission.CurrentUserPermission = user.status;
                    if (Permission.CurrentUserPermission != PermissionGroup.Admin && ProgramState.IsWorkspaceLocked())
                    {
                        Locked l = new Locked();
                        l.ShowDialog();
                        this.ShowDialog();
                    }
                    else
                    {
                        ProgramState.OpenSession();
                        ServerProcessor.Listen();
                        DialogResult = true;
                        this.Close();
                    }
                    
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

        private void RefreshClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.vm.Refresh();
        }

        private void RemoveClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.vm.RemoveSelected();
        }

        private void EditClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VM_DefExistWorkspace vmedit = new VM_DefExistWorkspace();
            vmedit.WorkspaceName = this.vm.SelectedWorkspace;
            vmedit.WorkspacePath = this.vm.Path;
            DefineExistingWorkspace dew = new DefineExistingWorkspace(vmedit);
            dew.ShowDialog();
            this.vm.Refresh();
        }

        private void AddClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            switch (ProgramState.ShowQuestionDialog("Вы собираетесь добавить существующее рабочее пространство или хотите создать новое?", "Выбор добавления", "Создать новое", "Добавить существующее"))
            {
                case DialogStatus.Yes:
                default:
                    CreateWorkspace cw = new CreateWorkspace();
                    cw.ShowDialog();
                    break;
                case DialogStatus.No:
                    DefineExistingWorkspace dew = new DefineExistingWorkspace();
                    dew.ShowDialog();
                    break;
            }
            this.vm.Refresh();
        }
    }
}
