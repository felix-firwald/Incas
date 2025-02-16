using Incas.Core.Classes;
using Incas.Core.Interfaces;
using Incas.Core.ViewModels;
using Incas.Core.Views.Pages;
using Incas.Server.AutoUI;
using IncasEngine.Core;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Models;
using System;
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

        private void Logo_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.PlayEasterEgg("Rooster");
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
            using (Class cl = new())
            {
                foreach (string category in cl.GetCategoriesOfClassType(ClassType.Process))
                {
                    this.AddPageButton(category, Classes.Icon.Graph, Controls.MainWindowButtonTab.ClassCategoryPrefix + category);
                }
                foreach (string category in cl.GetCategoriesOfClassType(ClassType.Document))
                {
                    this.AddPageButton(category, Classes.Icon.FileRichText, Controls.MainWindowButtonTab.ClassCategoryPrefix + category);
                }
                foreach (string category in cl.GetCategoriesOfClassType(ClassType.Model))
                {
                    this.AddPageButton(category, Classes.Icon.Database, Controls.MainWindowButtonTab.ClassCategoryPrefix + category);
                }
            }
            if (ProgramState.CurrentWorkspace.CurrentGroup.IsUsersSettingsVisible)
            {
                this.AddAdminPageButton("Пользователи", Classes.Icon.UserGears, Controls.MainWindowButtonTab.UsersSettings);
            }
            if (ProgramState.CurrentWorkspace.CurrentGroup.IsGroupSettingsVisible)
            {
                this.AddAdminPageButton("Группы", Classes.Icon.HouseGear, Controls.MainWindowButtonTab.GroupsSettings);             
            }
            if (ProgramState.CurrentWorkspace.CurrentGroup.IsWorkspaceSettingsVisible)
            {
                this.AddAdminPageButton("Рабочее пространство", Classes.Icon.GearWide, Controls.MainWindowButtonTab.WorkspaceSettings);
            }
        }
        private void AddPageButton(string name, Icon icon, string path)
        {
            Controls.MainWindowButtonTab bt = new(path, icon, name);
            bt.OnNewTabRequested += this.Bt_OnNewTabRequested;
            this.CustomTabs.Children.Add(bt);
        }
        private void AddAdminPageButton(string name, Icon icon, string path)
        {
            Controls.MainWindowButtonTab bt = new(path, icon, name);
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

        private void RefreshClick(object sender, MouseButtonEventArgs e)
        {
            this.vm.LoadInfo();
            this.UpdateTabs();
        }
    }
}
