using Incas.Core.AutoUI;
using Incas.Core.Views.Windows;
using Incas.CustomDatabases.Views.Windows;
using Incas.Miniservices.Clipboard.Views.Windows;
using Incas.Templates.Components;
using Incas.Templates.Models;
using Incas.Templates.Views.Windows;
using Incas.Users.Models;
using Incas.Users.Views.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

namespace Incas.Core.Classes
{
    internal static class DialogsManager
    {
        public static bool ShowSaveFileDialog(ref string file, string filter)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = filter
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                file = saveFileDialog.FileName;
                return true;
            }
            file = "";
            return false;
        }
        public static bool ShowOpenFileDialog(ref string file, string filter)
        {
            return ShowOpenFileDialog(ref file, filter, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        }
        public static bool ShowOpenFileDialog(ref string file, string filter, string root)
        {
            OpenFileDialog fd = new()
            {
                Filter = filter,
                InitialDirectory = root
            };
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                file = fd.FileName;
                return true;
            }
            return false;
        }
        public static bool ShowFolderDialog(ref string selectedPath)
        {
            return ShowFolderDialog(ref selectedPath, selectedPath);
        }
        public static bool ShowFolderDialog(ref string selectedPath, string root)
        {
            if (!Directory.Exists(root))
            {
                root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            FolderBrowserDialog fb = new()
            {
                SelectedPath = root
            };
            if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectedPath = fb.SelectedPath;
                return true;
            }
            return false;
        }
        public static bool ShowSimpleFormDialog(AutoUIBase data, string title)
        {
            DialogSimpleForm ds = new(data, title);
            return (bool)ds.ShowDialog();
        }
        public static bool ShowSimpleFormDialog(AutoUIBase data, string title, Icon pathIcon)
        {
            DialogSimpleForm ds = new(data, title, pathIcon);
            return (bool)ds.ShowDialog();
        }
        public static void ShowAccessErrorDialog(string message, string title = "Нет доступа")
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ProgramState.PlaySound("UI-Exclamation");
                Dialog d = new(message, title, Dialog.DialogIcon.AccessDenied);
                d.ShowDialog();
            });
        }
        public static bool ShowActiveUserSelector(out Session session, string helpText)
        {
            ActiveUserSelector au = new(helpText);
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
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
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
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
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                bd.ShowDialog();
            });
            return bd;
        }
        public static string ShowClipboardManager(bool autoclose = false)
        {
            Miniservices.Clipboard.Views.Windows.Clipboard c = new(autoclose);
            c.ShowDialog();
            return c.SelectedText;
        }
        public static string ShowComboBoxDialog(string title, List<string> elements)
        {         
            return ShowComboBoxDialog(title, elements, "");
        }
        public static string ShowComboBoxDialog(string title, List<string> elements, string description)
        {
            switch (elements.Count)
            {
                case 0:
                    return "";
                case 1:
                    return elements[0];
            }
            DialogComboBox dc = new(elements, title, description);
            dc.ShowDialog();
            return dc.SelectedValue;
        }
        public static void ShowDatabaseErrorDialog(string message, string title = "Ошибка при выполнении запроса")
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ProgramState.PlaySound("UI-Exclamation");
                ShowWaitCursor(false);
                Dialog d = new(message, title, Dialog.DialogIcon.DatabaseError);
                d.ShowDialog();
            });
        }
        public static void ShowErrorDialog(Exception ex, string startMessage = "При выполнении действия возникла непредвиденная ошибка")
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                
                ProgramState.PlaySound("UI-Exclamation");
                ShowWaitCursor(false);
                Dialog d = new($"{startMessage}.\nТип исключения: {ex.GetType().Name}\n\nТехнические подробности:\n" + ex.Message, "Возникла ошибка", Dialog.DialogIcon.Error);
                d.ShowDialog();
            });
        }
        public static void ShowErrorDialog(string message, string title = "Возникла неизвестная ошибка")
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (message == null)
                {
                    return;
                }
                ProgramState.PlaySound("UI-Exclamation");
                ShowWaitCursor(false);
                Dialog d = new(message, title, Dialog.DialogIcon.Error);
                d.ShowDialog();
            });
        }
        public static void ShowExclamationDialog(string message, string title = "Обратите внимание")
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ProgramState.PlaySound("UI-Attention");
                ShowWaitCursor(false);
                Dialog d = new(message, title, Dialog.DialogIcon.Exclamation);
                d.ShowDialog();
            });
        }
        public static void ShowInfoDialog(string message, string title = "Оповещение")
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ProgramState.PlaySound("UI-Attention");
                ShowWaitCursor(false);
                Dialog d = new(message, title, Dialog.DialogIcon.Info);
                d.ShowDialog();
            });
        }
        // for dev only
        public static void ShowInfoDialog(object message, string title = "Оповещение")
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ProgramState.PlaySound("UI-Attention");
                ShowWaitCursor(false);
                Dialog d = new(message.ToString(), title, Dialog.DialogIcon.Info);
                d.ShowDialog();
            });
        }
        public static string ShowInputBox(string title, string description = "Введите значение")
        {
            DialogInput dialog = new(title, description);
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
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
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ProgramState.PlaySound("UI-Attention");
                ShowWaitCursor(false);
                d.ShowDialog();
            });
            return d.status;
        }
        public static Template ShowTemplateSelector(TemplateType type, string help)
        {
            TemplateSelector ts = new(type, help);
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ts.ShowDialog();
            });
            return ts.SelectedTemplate.AsModel();
        }
        public static bool ShowUserSelector(out Users.Models.User user)
        {
            UserSelector us = new();
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
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
            Mouse.OverrideCursor = wait ? System.Windows.Input.Cursors.Wait : null;
        }
        public static void ShowWindow(System.Windows.Controls.UserControl control, string title)
        {
            ContainerWindow cw = new(control, title);
            cw.Show();
        }
        public static void ShowWindowDialog(System.Windows.Controls.UserControl control, string title)
        {
            ContainerWindow cw = new(control, title);
            cw.ShowDialog();
        }
    }
}