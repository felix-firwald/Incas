using Incas.Core.AutoUI;
using Incas.Core.Classes;
using Incas.Core.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

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
                WorkspacePaths.SetCommonPath(this.cpath.Text);
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
                DialogsManager.ShowWaitCursor();
                ProgramState.CurrentUserParameters = ProgramState.CurrentUser.GetParametersContext();
                if (ProgramState.CurrentUserParameters.IsRightPassword(this.pwd.Text))
                {
                    Permission.CurrentUserPermission = ProgramState.CurrentUserParameters.Permission_group;
                    if (Permission.CurrentUserPermission != PermissionGroup.Admin && ProgramState.IsWorkspaceLocked())
                    {
                        Locked l = new();
                        l.ShowDialog();
                        this.ShowDialog();
                    }
                    else
                    {
                        DialogsManager.ShowWaitCursor(false);
                        this.DialogResult = true;
                        this.Close();
                    }
                }
                else
                {
                    DialogsManager.ShowExclamationDialog("Пароль введен неверно.", "Вход невозможен");
                    this.pwd.Text = "";
                }
            }
            else
            {
                DialogsManager.ShowExclamationDialog("Пользователь не выбран.", "Вход невозможен");
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
            DefineExistingWorkspace dew = new()
            {
                Name = this.vm.SelectedWorkspace,
                Path = this.vm.Path
            };
            dew.ShowDialog("Редактирование элемента", Classes.Icon.Folder);
            this.vm.Refresh();
        }

        private void AddClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            switch (DialogsManager.ShowQuestionDialog("Вы собираетесь добавить существующее рабочее пространство или хотите создать новое?", "Выбор добавления", "Создать новое", "Добавить существующее"))
            {
                case DialogStatus.Yes:
                default:
                    CreateWorkspace cw = new();
                    if (cw.ShowDialog("Создание рабочего пространства", Classes.Icon.Folder))
                    {
                        //ProgramState.InitWorkspace(cw);
                    }
                    break;
                case DialogStatus.No:
                    DefineExistingWorkspace dew = new();
                    dew.ShowDialog("Добавление рабочего пространства", Classes.Icon.Folder);
                    break;
            }
            this.vm.Refresh();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                try
                {
                    this.DragMove();
                }
                catch { }
            }
        }
    }
}
