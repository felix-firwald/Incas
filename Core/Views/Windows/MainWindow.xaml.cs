using DocumentFormat.OpenXml.Packaging;
using Incas.Core.Classes;
using Incas.Core.Interfaces;
using Incas.Core.ViewModels;
using Incas.Core.Views.Pages;
using Incas.Objects.Views.Windows;
using Incas.Rendering.Components;
using Incas.Server.AutoUI;
using IncasEngine.Core;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.Workspace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Incas.Core.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel vm;
        public MainWindow()
        {
            this.InitializeComponent();
            if (!ProgramState.CheckSensitive())
            {
                System.Windows.Application.Current.Shutdown();
                return;
            }
            ProgramState.MainWindow = this;
            this.vm = new MainWindowViewModel();
            ProgramState.MainWindowViewModel = this.vm;
            this.DataContext = this.vm;
            this.UpdateTabs();
            this.AddTabItem(new StartPage(), "$START", "Начало", TabType.General);
        }
        public void PlaceStatusBar(IStatusBar bar)
        {
            this.Dispatcher.Invoke(() =>
            {
                
                this.StatusBarContainer.Child = (Control)bar;
            });         
        }
        public void NullifyStatusBar()
        {
            this.Dispatcher.Invoke(() =>
            {
                this.StatusBarContainer.Child = null;
            });
        }
        private void PlayEasterEgg(string name = "Chicken")
        {
            try
            {
                if (RegistryData.IsRoosterExists())
                {
                    using FileStream stream = File.Open($"Static\\{name}.wav", FileMode.Open);
                    SoundPlayer myNewSound = new(stream);
                    myNewSound.Load();
                    myNewSound.Play();
                }
            }
            catch { }
        }

        private void OnClosed(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void OnResize(object sender, SizeChangedEventArgs e)
        {
            //if (this.StackLeft.RenderSize.Width < 150)
            //{
            //    this.LSurname.Visibility = Visibility.Collapsed;
            //    this.Incubator.Visibility = Visibility.Collapsed;
            //}
            //else
            //{
            //    this.LSurname.Visibility = Visibility.Visible;
            //    this.Incubator.Visibility = Visibility.Visible;
            //}
        }

        private void Logo_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.PlayEasterEgg("Rooster");
            if (DialogsManager.ShowQuestionDialog("Это действие переведет рабочее пространство в неуправляемый тестовый режим после активации специальной кнопки. Все ваши данные могут быть потеряны. Продолжить?", "Продолжить?", "Продолжай", "Вырубай") == DialogStatus.Yes)
            {
                this.vm.TestFunctionVisibility = Visibility.Visible;
            }            
        }

        private void window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    this.vm.DoOpenClipBoard("");
                    break;
                case Key.F2:
                    this.vm.DoOpenTasks("");
                    break;
                case Key.F3:
                    this.vm.DoOpenTextEditor("");
                    break;
                case Key.F4:
                    this.vm.DoOpenFileManager("");
                    break;
                case Key.F5:
                    //this.vm.DoOpenFile("Word");
                    break;
                case Key.F6:
                    //this.vm.DoOpenFile("Excel");
                    break;
                case Key.F7:
                    this.vm.DoOpenWeb("");
                    break;                    
                case Key.F12:
                    TestSignal ts = new();
                    ts.ShowDialog("345", Classes.Icon.Pin);
                    break;
            }
        }
        public void UpdateTabs()
        {
            this.CustomTabs.Children.Clear();
            foreach (WorkspaceComponent wc in ProgramState.CurrentWorkspace.CurrentGroup.GetAvailableComponents())
            {
                this.AddPageButton(wc);
            }
            if (ProgramState.CurrentWorkspace.CurrentGroup.IsUsersSettingsVisible)
            {
                this.AddAdminPageButton("Пользователи", Classes.Icon.UserGears, Controls.MainWindowButtonTab.UsersSettings, "Страница управления пользователями рабочего пространства");
            }
            if (ProgramState.CurrentWorkspace.CurrentGroup.IsGroupSettingsVisible)
            {
                this.AddAdminPageButton("Группы", Classes.Icon.HouseGear, Controls.MainWindowButtonTab.GroupsSettings, "Страница управления группами полномочий рабочего пространства");             
            }
            if (ProgramState.CurrentWorkspace.CurrentGroup.IsWorkspaceSettingsVisible)
            {
                this.AddAdminPageButton("Рабочее пространство", Classes.Icon.GearWide, Controls.MainWindowButtonTab.WorkspaceSettings, "Страница управления рабочим пространством");
            }
        }
        private void AddPageButton(WorkspaceComponent component)
        {
            Controls.MainWindowButtonTab bt = new(component);
            bt.OnNewTabRequested += this.Bt_OnNewTabRequested;
            this.CustomTabs.Children.Add(bt);
        }
        private void AddAdminPageButton(string name, Icon icon, string path, string description)
        {
            Controls.MainWindowButtonTab bt = new(name, icon, path, description);
            bt.OnNewTabRequested += this.Bt_OnNewTabRequested;
            this.CustomTabs.Children.Add(bt);
        }

        private void Bt_OnNewTabRequested(Control item, string id, string name)
        {         
            if (item is ITabItem tabItem)
            {
                this.AddTabItem(tabItem, id, name, TabType.Usual);
            }
            else
            {
                this.AddTabItem(item, id, name, TabType.Usual);
            }
        }
        public void AddTabItem(Control item, string id, string name, TabType tabType)
        {
            foreach (Control c in this.TabMain.Items)
            {
                if (c.Uid == id)
                {
                    ((TabItem)c).IsSelected = true;
                    return;
                }
            }
            TabItem ti = new()
            {
                Header = name,
                Uid = id,
                Content = item,
                IsSelected = true
            };
            switch (tabType)
            {
                case TabType.General:
                    ti.BorderBrush = new SolidColorBrush(System.Windows.Media.Colors.DodgerBlue);
                    break;
                case TabType.Tool:
                    ti.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(52, 201, 36));
                    break;
            }
            ti.IsEnabledChanged += this.TabItem_IsEnabledChanged;
            this.TabMain.Items.Add(ti);
        }
        public void AddTabItem(ITabItem item, string id, string name, TabType tabType)
        {
            item.OnClose += this.Item_OnClose;
            item.Id = id;
            this.AddTabItem((Control)item, id, name, tabType);
        }

        private void Item_OnClose(ITabItem item)
        {
            foreach (Control c in this.TabMain.Items)
            {
                if (c.Uid == item.Id)
                { 
                    this.TabMain.Items.Remove((TabItem)c);
                    return;
                }
            }
        }

        private void TabItem_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.TabMain.Items.Remove(sender);
            if (this.TabMain.Items.Count == 0)
            {
                this.Close();
            }
        }

        private void ShowLicenseClick(object sender, RoutedEventArgs e)
        {
            IncasEngine.License.License lic = IncasEngine.License.License.ReadLicense(RegistryData.GetPathToLicense());
            DialogsManager.ShowInfoDialog($"Поставщик:    {lic.LicenseAuthorComputer}\n" +
                $"Дата выдачи:  {lic.LicenseDate.ToString("dd.MM.yyyy HH:mm")}\n" +
                $"Область:      {lic.LicenseNamespace}\n" +
                $"Наименование: {lic.LicenseName}\n" +
                $"Почта:        {lic.Email}\n" +
                $"Тип (пакет):  {lic.LicenseType}\n" +
                $"Истекает:     {lic.ExpirationDate.ToString("dd.MM.yyyy HH:mm")}\n" +
                $"Комментарий:  {lic.Commentary}", "Информация о лицензии");
        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            this.vm.LoadInfo();
            this.UpdateTabs();
        }

        private async void window_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    string id = await FileTemplator.GetIdentifier(files[0]);
                    if (id is not null)
                    {
                        ObjectReference or = ObjectReference.Parse(id);
                        if (or.IsValid())
                        {
                            Class targetClass = new(or.Class);
                            if (targetClass != null)
                            {
                                ObjectsEditor editor = new(targetClass, [Processor.GetObject(targetClass, or.Object)]);
                                editor.Show();
                            }
                        }
                    }
                    else
                    {
                        DialogsManager.ShowExclamationDialog("Документ не создавался в INCAS, либо был создан в версии программы, которая еще не поддерживала такую функциональность", "Действие невозможно");
                    }
                }
            }
            catch (IOException)
            {
                DialogsManager.ShowErrorDialog("INCAS не удалось получить доступ к файлу (возможно он блокируется другим процессом).");
            }
            catch (KeyNotFoundException)
            {
                DialogsManager.ShowExclamationDialog("Документ не создавался в INCAS, либо был создан в версии программы, которая еще не поддерживала такую функциональность", "Действие невозможно");
            }
        }

        private void OpenTestClick(object sender, RoutedEventArgs e)
        {
            TesterWindow tw = new();
            Application.Current.MainWindow = tw;
            this.Hide();
            tw.ShowDialog();
            this.Close();
        }
    }
}
