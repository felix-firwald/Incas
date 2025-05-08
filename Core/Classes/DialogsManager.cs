using Incas.Core.Views.Windows;
using Incas.Help.Components;
using Incas.Help.Windows;
using Incas.Objects.Processes.Views.Pages;
using Incas.Objects.Views.Pages;
using Incas.Objects.Views.Windows;
using IncasEngine.Core.DatabaseQueries;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Exceptions;
using IncasEngine.ObjectiveEngine.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace Incas.Core.Classes
{
    internal static class DialogsManager
    {
        static DialogsManager()
        {
            IncasEngine.Core.EngineEvents.OnMessageRequested += EngineEvents_OnMessageRequested;
            IncasEngine.Core.EngineEvents.OnQuestionRequested += EngineEvents_OnQuestionRequested;
            IncasEngine.Core.EngineEvents.OnShowTabRequested += EngineEvents_OnShowTabRequested;
            IncasEngine.Core.EngineEvents.OnFileDialogRequested += EngineEvents_OnFileDialogRequested;
            IncasEngine.Core.EngineEvents.OnEditorRequested += EngineEvents_OnEditorRequested;
            IncasEngine.Core.EngineEvents.OnTableViewerRequested += EngineEvents_OnTableViewerRequested;
        }

        private static void EngineEvents_OnTableViewerRequested(IncasEngine.AdditionalFunctionality.TableViewerSettings viewer)
        {
            TableViewer tv = new(viewer);
            tv.Show();
        }

        private static void EngineEvents_OnEditorRequested(IncasEngine.ObjectiveEngine.Interfaces.IClass cl, IncasEngine.ObjectiveEngine.Interfaces.IObject obj)
        {
            if (cl.GetClassData().PreferPage)
            {
                ObjectPage page = new(cl, obj);
                DialogsManager.ShowPageWithGroupBox(page, obj.Name, "EDITOR" + obj.Id.ToString());
            }
            else
            {
                switch (obj.Class.Type)
                {
                    case ClassType.Model:
                    case ClassType.Document:
                    case ClassType.Event:
                    case ClassType.ServiceClassGroup:
                    case ClassType.ServiceClassUser:
                    case ClassType.ServiceClassTask:
                        ObjectsEditor oc = new(cl, [obj]);
                        oc.Show();
                        break;
                    case ClassType.Process:
                        ProcessViewer pv = new(cl as Class, obj as IncasEngine.ObjectiveEngine.Types.Processes.Process);
                        DialogsManager.ShowPageWithGroupBox(pv, obj.Name, obj.Id.ToString());
                        break;
                }
            }
        }

        private static bool EngineEvents_OnFileDialogRequested(ref string file, string filter, string root)
        {
            return DialogsManager.ShowOpenFileDialog(ref file, filter, root);
        }

        private static void EngineEvents_OnShowTabRequested(IncasEngine.ObjectiveEngine.Interfaces.IClass cl, System.Collections.Generic.List<Guid> list, IncasEngine.ObjectiveEngine.Models.Field field)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ObjectsCorrector oc = new(cl, list, field);
                ShowPageWithGroupBox(oc, "Исправление несоответствий", "FIX:" + field.Id.ToString());
            });
        }
        public static void ShowLimitedEditionMessage()
        {
#if E_FREE
            string message = "В редакции Free этот функционал недоступен. Попробуйте обновиться до одной из следующих редакций: Community, Extended, Business Pro.";
            DialogsManager.ShowAccessErrorDialog(message, "Функционал недоступен");
#elif E_COMMUNITY
            string message = "В редакции Community этот функционал недоступен. Попробуйте обновиться до одной из следующих редакций: Extended, Business Pro.";
            DialogsManager.ShowAccessErrorDialog(message, "Функционал недоступен");
#elif E_EXTENDED
            string message = "В редакции Extended этот функционал недоступен. Попробуйте обновиться до редакции Business Pro.";
            DialogsManager.ShowAccessErrorDialog(message, "Функционал недоступен");
#endif

        }
        private static bool EngineEvents_OnQuestionRequested(string header, string message, string yesButton, string noButton)
        {
            switch (ShowQuestionDialog(message, header, yesButton, noButton))
            {
                case DialogStatus.Yes:
                    return true;
            }
            return false;
        }

        private static void EngineEvents_OnMessageRequested(IncasEngine.Core.DialogMessageType type, string header, string message)
        {
            switch (type)
            {
                case IncasEngine.Core.DialogMessageType.AccessError:
                    ShowAccessErrorDialog(message, header);
                    break;
                case IncasEngine.Core.DialogMessageType.DatabaseError:
                    ShowDatabaseErrorDialog(message, header);
                    break;
                case IncasEngine.Core.DialogMessageType.CommonError:
                    ShowErrorDialog(message, header);
                    break;
                case IncasEngine.Core.DialogMessageType.Exclamation:
                    ShowExclamationDialog(message, header);
                    break;
                case IncasEngine.Core.DialogMessageType.Info:
                    ShowInfoDialog(message, header);
                    break;
            }
        }

        public static bool ShowFolderBrowserDialog(ref string path)
        {
            FolderBrowserDialog fb = new();
            fb.InitialDirectory = path;
            if (fb.ShowDialog() == DialogResult.OK)
            {
                path = fb.SelectedPath;
                return true;
            }
            return false;
        }
        public static bool ShowSaveFileDialog(ref string file, string filter)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new()
            {
                Filter = filter
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                file = saveFileDialog.FileName;
                return true;
            }
            file = "";
            return false;
        }
        public static void ShowWindow(string title, System.Windows.Controls.UserControl uc)
        {
            ContainerWindow cw = new(uc, title);
            cw.Show();
        }

        public static void ShowHelp(HelpType type)
        {
            HelpWindow help = new(type);
            help.Show();
        }
        public static void ShowPage(System.Windows.Controls.Control control, string name, string id, TabType tt = TabType.Usual)
        {
            ProgramState.MainWindow.AddTabItem(control, id, name, tt);
        }
        public static TabItem ShowPageWithGroupBox(System.Windows.Controls.Control control, string name, string id, TabType tt = TabType.Usual)
        {
            TabItem returning = null;
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                System.Windows.Controls.GroupBox result = new()
                {
                    Header = name,
                    Content = control
                };
                returning = ProgramState.MainWindow.AddTabItem(result, id, name, tt);
            });
            return returning;
        }
        public static bool ShowOpenFileDialog(ref string file, string filter)
        {
            return ShowOpenFileDialog(ref file, filter, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        }
        public static bool ShowOpenFileDialog(ref string file, string filter, string root)
        {
            Microsoft.Win32.OpenFileDialog fd = new()
            {
                Filter = filter,
                InitialDirectory = root
            };
            if (fd.ShowDialog() == true)
            {
                file = fd.FileName;
                return true;
            }
            return false;
        }
        public static void ShowWebViewer(string path, bool autoremove = true)
        {
            DialogsManager.ShowWebViewer("Просмотр", path, autoremove);
        }
        public static void ShowWebPage(string path)
        {
            try
            {
                System.Diagnostics.Process proc = new();
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.FileName = path;
                proc.Start();
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
            }
        }
        public static void ShowWebViewer(string name, string path, bool autoremove)
        {
            try
            {
                DialogsManager.ShowWaitCursor(false);
                ProgramStatusBar.Hide();
                WebPreviewWindow wp = new(name, path, autoremove);
                wp.ShowDialog();
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex, "При попытке открытия документа возникла ошибка");
            }
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
        public static void ShowAccessErrorDialog(string message, string title = "Нет доступа")
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ProgramState.PlaySound("UI-Exclamation");
                Dialog d = new(message, title, Dialog.DialogIcon.AccessDenied);
                d.ShowDialog();
            });
        }
        public static void ShowAccessErrorDialog(AccessException message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ProgramState.PlaySound("UI-Exclamation");
                Dialog d = new(message.Message, "Нет доступа", Dialog.DialogIcon.AccessDenied);
                d.ShowDialog();
            });
        }
        public static string ShowClipboardManager(bool autoclose = false)
        {
            Miniservices.Clipboard.Views.Windows.Clipboard c = new(autoclose);
            c.ShowDialog();
            return c.SelectedText;
        }
        public static void ShowTasksManager()
        {
            //Taskboard t = new();
            //t.ShowDialog();
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
        public static void ShowCriticalErrorDialog(string message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ProgramState.PlaySound("UI-Exclamation");
                ShowWaitCursor(false);
                CriticalError d = new(message);
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
            ProgramStatusBar.Hide();
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
            ProgramStatusBar.Hide();
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
                Dialog d = new(JsonConvert.SerializeObject(message, Formatting.Indented), title, Dialog.DialogIcon.Info);
                d.ShowDialog();
            });
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