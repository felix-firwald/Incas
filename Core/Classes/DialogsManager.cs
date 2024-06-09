using Incas.Core.Views.Windows;
using Incas.CustomDatabases.Views.Windows;
using Incas.Templates.Models;
using Incas.Templates.Views.Windows;
using Incas.Users.Models;
using Incas.Users.Views.Windows;
using IncasEngine.TemplateManager;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incas.Core.Classes
{
    internal static class DialogsManager
    {
        public static void ShowAccessErrorDialog(string message, string title = "Нет доступа")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ProgramState.PlaySound("UI-Exclamation");
                Dialog d = new(message, title, Dialog.DialogIcon.AccessDenied);
                d.ShowDialog();
            });
        }
        public static bool ShowActiveUserSelector(out Session session, string helpText)
        {
            ActiveUserSelector au = new(helpText);
            Application.Current.Dispatcher.Invoke(() =>
            {
                au.ShowDialog();
            });
            if (au.SelectedSession != null)
            {
                session = au.SelectedSession;
                return true;
            }
            session = null;
            return false;
        }
        public static BindingSelector ShowBindingSelector()
        {
            BindingSelector bd = new();
            Application.Current.Dispatcher.Invoke(() =>
            {
                bd.ShowDialog();
            });
            return bd;
        }
        public static BindingSelector ShowBindingSelector(string database, bool dbEnabled = true)
        {
            BindingSelector bd = new(database, dbEnabled);
            bd.ShowDialog();
            return bd;
        }
        public static BindingSelector ShowBindingSelector(string database, string table, bool dbEnabled = true, bool tableEnabled = true)
        {
            BindingSelector bd = new(database, table, dbEnabled, tableEnabled);
            Application.Current.Dispatcher.Invoke(() =>
            {
                bd.ShowDialog();
            });
            return bd;
        }
        public static void ShowDatabaseErrorDialog(string message, string title = "Ошибка при выполнении запроса")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ProgramState.PlaySound("UI-Exclamation");
                Dialog d = new(message, title, Dialog.DialogIcon.DatabaseError);
                d.ShowDialog();
            });
        }
        public static void ShowErrorDialog(string message, string title = "Возникла неизвестная ошибка")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (message == null)
                {
                    return;
                }
                ProgramState.PlaySound("UI-Exclamation");
                Dialog d = new(message, title, Dialog.DialogIcon.Error);
                d.ShowDialog();
            });
        }
        public static void ShowExclamationDialog(string message, string title = "Обратите внимание")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ProgramState.PlaySound("UI-Attention");
                Dialog d = new(message, title, Dialog.DialogIcon.Exclamation);
                d.ShowDialog();
            });
        }
        public static void ShowInfoDialog(string message, string title = "Оповещение")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ProgramState.PlaySound("UI-Attention");
                Dialog d = new(message, title, Dialog.DialogIcon.Info);
                d.ShowDialog();
            });
        }
        // for dev only
        public static void ShowInfoDialog(object message, string title = "Оповещение")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ProgramState.PlaySound("UI-Attention");
                Dialog d = new(message.ToString(), title, Dialog.DialogIcon.Info);
                d.ShowDialog();
            });
        }
        public static string ShowInputBox(string title, string description = "Введите значение")
        {
            DialogInput dialog = new(title, description);
            Application.Current.Dispatcher.Invoke(() =>
            {
                dialog.ShowDialog();
            });
            return dialog.Input;
        }
        public static void ShowOpenFileDialog()
        {

        }
        public static DialogStatus ShowQuestionDialog(string message, string title, string yesText = "Да", string noText = "Нет")
        {
            DialogQuestion d = new(message, title, yesText, noText);
            Application.Current.Dispatcher.Invoke(() =>
            {
                ProgramState.PlaySound("UI-Attention");
                d.ShowDialog();
            });
            return d.status;
        }
        public static Template ShowTemplateSelector(TemplateType type, string help)
        {
            TemplateSelector ts = new(type, help);
            Application.Current.Dispatcher.Invoke(() =>
            {
                ts.ShowDialog();
            });
            return ts.SelectedTemplate.AsModel();
        }
        public static bool ShowUserSelector(out User user)
        {
            UserSelector us = new();
            Application.Current.Dispatcher.Invoke(() =>
            {
                us.ShowDialog();
            });
            if (us.Result == DialogStatus.Yes)
            {
                user = us.SelectedUser;
                return true;
            }
            user = null;
            return false;
        }
        public static void ShowWaitCursor(bool wait = true)
        {
            Mouse.OverrideCursor = wait ? Cursors.Wait : null;
        }
        public static void ShowWindow(UserControl control, string title)
        {
            ContainerWindow cw = new(control, title);
            cw.Show();
        }
        public static void ShowWindowDialog(UserControl control, string title)
        {
            ContainerWindow cw = new(control, title);
            cw.ShowDialog();
        }
    }
}