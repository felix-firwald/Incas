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
                
                if (Directory.Exists(this.cpath.Text))
                {
                    TryAuthenticate();
                }
            }  
        }

        private void TryAuthenticate()
        {
            if (ProgramState.CurrentUser != null)
            {
                ProgramState.ShowWaitCursor();
                ProgramState.CurrentUserParameters = ProgramState.CurrentUser.GetParametersContext();
                if (ProgramState.CurrentUserParameters.IsRightPassword(this.pwd.Text))
                {
                    Permission.CurrentUserPermission = ProgramState.CurrentUserParameters.permission_group;
                    if (Permission.CurrentUserPermission != PermissionGroup.Admin && ProgramState.IsWorkspaceLocked())
                    {
                        Locked l = new Locked();
                        l.ShowDialog();
                        this.ShowDialog();
                    }
                    else
                    {
                        ProgramState.SetSectorByUser(ProgramState.CurrentUser);
                        ProgramState.OpenSession();
                        ServerProcessor.Listen();
                        ProgramState.ShowWaitCursor(false);
                        DialogResult = true;
                        this.Close();
                    }

                }
                else
                {
                    ProgramState.ShowExclamationDialog("Пароль введен неверно.", "Вход невозможен");
                    this.pwd.Text = "";
                }
            }
            else
            {
                ProgramState.ShowExclamationDialog("Пользователь не выбран.", "Вход невозможен");
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
