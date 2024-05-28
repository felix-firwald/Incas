using Common;
using Incas.Core.Classes;
using Incas.Core.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace Incas.Core.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для OpenIncubator.xaml
    /// </summary>
    public partial class OpenWorkspace : Window
    {
        private OpenWorkspaceViewModel vm;
        public OpenWorkspace()
        {
            this.InitializeComponent();
            ProgramState.LoadUserData();
            this.vm = new OpenWorkspaceViewModel();
            this.DataContext = this.vm;
        }

        private void LogInClicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.cpath.Text) || string.IsNullOrEmpty(this.pwd.Text))
            {
                Dialog d = new("Одно из обязательных полей не заполнено!", "Вход невозможен");
                d.ShowDialog();
            }
            else
            {
                ProgramState.SetCommonPath(this.cpath.Text);
                ProgramState.GetDBFile();

                if (Directory.Exists(this.cpath.Text))
                {
                    this.TryAuthenticate();
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
                        Locked l = new();
                        l.ShowDialog();
                        this.ShowDialog();
                    }
                    else
                    {
                        ProgramState.SetSectorByUser(ProgramState.CurrentUser);
                        ProgramState.OpenSession();
                        ServerProcessor.Listen();
                        ProgramState.ShowWaitCursor(false);
                        this.DialogResult = true;
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
            FolderBrowserDialog folderDialog = new();
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
            DefExistWorkspaceViewModel vmedit = new()
            {
                WorkspaceName = this.vm.SelectedWorkspace,
                WorkspacePath = this.vm.Path
            };
            DefineExistingWorkspace dew = new(vmedit);
            dew.ShowDialog();
            this.vm.Refresh();
        }

        private void AddClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            switch (ProgramState.ShowQuestionDialog("Вы собираетесь добавить существующее рабочее пространство или хотите создать новое?", "Выбор добавления", "Создать новое", "Добавить существующее"))
            {
                case DialogStatus.Yes:
                default:
                    CreateWorkspace cw = new();
                    cw.ShowDialog();
                    break;
                case DialogStatus.No:
                    DefineExistingWorkspace dew = new();
                    dew.ShowDialog();
                    break;
            }
            this.vm.Refresh();
        }
    }
}
