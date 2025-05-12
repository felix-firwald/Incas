using Incas.Core.Classes;
using Incas.Objects.AutoUI;
using IncasEngine.Core;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Components;
using IncasEngine.RuntimeCompilation;
using IncasEngine.Workspace;
using IncasEngine.Workspace.WorkspaceTemplates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace Incas.Core.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для AboutIncas.xaml
    /// </summary>
    public partial class AboutIncas : Window
    {
        public AboutIncas()
        {
            this.InitializeComponent();
            this.VersionText.Text = $"Редакция {ProgramState.Edition}, версия {ProgramState.Version}";
            if (clicksShowCount > 4)
            {
                this.SecretMenu.Visibility = Visibility.Visible;
            }
        }
        private static int clicksShowCount = 1;
        private void LogoMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (clicksShowCount > 4)
            {
                if (ProgramState.CurrentWorkspace.CurrentGroup.Data.Indestructible)
                {
                    ProgramState.PlaySound("Rooster");
                    this.SecretMenu.Visibility = Visibility.Visible;
                }
                else
                {
                    ProgramState.PlaySound("UI-Attention");
                    this.Close();
                }
            }
            clicksShowCount++;
        }

        private void CloseMenu(object sender, RoutedEventArgs e)
        {
            ProgramState.PlaySound("UI-Attention");
            clicksShowCount = 1;
            this.SecretMenu.Visibility = Visibility.Collapsed;
        }

        private void ExportWSClick(object sender, RoutedEventArgs e)
        {
            string path = "";
            if (DialogsManager.ShowSaveFileDialog(ref path, "Шаблоны рабочего пространства Incas|.twinc"))
            {
                DialogsManager.ShowWaitCursor();
                WorkspaceTemplate.ExportWorkspace(path, "Импортировано пользователем");
                DialogsManager.ShowWaitCursor(false);
            }
        }

        private void ExportComponentClick(object sender, RoutedEventArgs e)
        {

        }

        private void ImportComponentClick(object sender, RoutedEventArgs e)
        {

        }

        private void IntegrityCheckClick(object sender, RoutedEventArgs e)
        {

        }

        private void MassiveFixOMClick(object sender, RoutedEventArgs e)
        {
            using (Class c = new())
            {
                DialogsManager.ShowWaitCursor();
                List<Class> classes = c.GetAll();
                foreach (Class c2 in classes)
                {
                    Processor.UpdateObjectMap(c2);
                }
                DialogsManager.ShowWaitCursor(false);
            }
        }

        private void OpenOMClick(object sender, RoutedEventArgs e)
        {
            ClassSelector select = new();
            if (select.ShowDialog("Выбор класса"))
            {
                Class cl = select.GetSelectedClass();
                string path = EngineGlobals.CurrentWorkspace.ObjectsPath + $"\\{cl.Id}.objinc";
                Process.Start("explorer.exe", string.Format("/select,\"{0}\"", path));
            }
        }

        private void DLLExportClick(object sender, RoutedEventArgs e)
        {
            //FinalAssembler fa = new();
            //string path = "";
            //if (DialogsManager.ShowSaveFileDialog(ref path, "DLL библиотеки|.dll"))
            //{
            //    DialogsManager.ShowWaitCursor();
            //    fa.CompileWorkspace(path);
            //    DialogsManager.ShowWaitCursor(false);
            //}           
        }

        private void CheckInitDefinitionClick(object sender, RoutedEventArgs e)
        {
            WorkspaceDefinition def = ProgramState.CurrentWorkspace.GetDefinition(true);
            string text = $"Было обнаружено, что в определении рабочего пространства отсутствует следующий обязательный служебный класс: ";
            int errors = 0;
            if (def.ServiceBackgroundActions is null)
            {
                DialogsManager.ShowExclamationDialog(text + "ServiceBackgroundActions");
                def.ServiceBackgroundActions = InitializationManager.InitializeBackgroundActionsClass();
                errors++;
            }
            if (def.ServiceStorage is null)
            {
                DialogsManager.ShowExclamationDialog(text + "ServiceStorage");
                def.ServiceStorage = InitializationManager.InitializeStorageClass();
                errors++;
            }
            if (def.ServiceTasks is null)
            {
                DialogsManager.ShowExclamationDialog(text + "ServiceTasks");
                def.ServiceTasks = InitializationManager.InitializeTasksClass();
                errors++;
            }
            if (errors > 0)
            {
                ProgramState.CurrentWorkspace.UpdateDefinition(def);
                DialogsManager.ShowInfoDialog("Ошибки исправлены. Окно можно закрыть.", "Результат проверки");
            }
            else
            {
                DialogsManager.ShowInfoDialog("Ошибок обнаружено не было. Окно можно закрыть.", "Результат проверки");
            }
        }

        private void OpenServiceClick(object sender, RoutedEventArgs e)
        {
            string path = ProgramState.CurrentWorkspace.ServiceDatabasePath;
            Process.Start("explorer.exe", string.Format("/select,\"{0}\"", path));
        }
    }
}
