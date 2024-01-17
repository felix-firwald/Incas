using Common;
using DocumentFormat.OpenXml.InkML;
using Incubator_2.ViewModels;
using Incubator_2.Windows;
using Incubator_2.Windows.ToolBar;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Windows.UI.Notifications;

namespace Incubator_2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (!ProgramState.CheckSensitive())
            {
                Application.Current.Shutdown();
            }
            this.DataContext = new MV_MainWindow();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            if (string.IsNullOrEmpty(ProgramState.CurrentUserParameters.password))
            {
                if (ProgramState.ShowQuestionDialog("Текущий пароль, использованный вами для входа, " +
                    "является временным, потому известен администратору и может быть им изменен.\nРекомендуем вам придумать свой пароль. Его не сможет увидеть и изменить никто, кроме вас.", "Завершение активации", "Установить пароль", "Не сейчас") == DialogStatus.Yes)
                {
                    SetPassword windowSet = new();
                    windowSet.ShowDialog();
                }
            }
        }

        private void OnClosed(object sender, EventArgs e)
        {
            ProgramState.CloseSession();
            Application.Current.Shutdown();
        }

        private void OnResize(object sender, SizeChangedEventArgs e)
        {
            if (this.StackLeft.RenderSize.Width < 150)
            {
                this.LSurname.Visibility = Visibility.Collapsed;
                this.LFullname.Visibility = Visibility.Collapsed;
                this.LPost.Visibility = Visibility.Collapsed;
                this.Incubator.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.LSurname.Visibility = Visibility.Visible;
                this.LFullname.Visibility = Visibility.Visible;
                this.LPost.Visibility = Visibility.Visible;
                this.Incubator.Visibility = Visibility.Visible;
            }
        }

        private void WindowMinimize(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void WindowMaximize(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            }
        }
        private void WindowClose(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void WindowMove(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch
            {
                
            }
        }

        private void Logo_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            switch (Permission.CurrentUserPermission)
            {
                case PermissionGroup.Admin:
                case PermissionGroup.Moderator:
                    ProgramState.ShowWaitCursor();
                    AdminPanel adminPanel = new AdminPanel();
                    adminPanel.Show();
                    break;
                case PermissionGroup.Operator:
                default:
                    ProgramState.ShowExlamationDialog("Данная функция доступна только администраторам и модераторам.", "Нет доступа");
                    break;
            }
            
        }

        private void FilesManagerClick(object sender, RoutedEventArgs e)
        {
            FilesManager fm = new();
            fm.Show();
        }
    }
}
