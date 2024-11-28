using Incas.Core.AutoUI;
using Incas.Core.Classes;
using Incas.Core.Interfaces;
using Incas.Core.ViewModels;
using Incas.Core.Views.Pages;
using Incas.Objects.Models;
using Incas.Server.AutoUI;
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
            }
            ProgramState.MainWindow = this;
            this.vm = new MainWindowViewModel();
            ProgramState.MainWindowViewModel = this.vm;
            this.DataContext = this.vm;
            if (string.IsNullOrEmpty(ProgramState.CurrentUserParameters.Password))
            {
                if (DialogsManager.ShowQuestionDialog("Текущий пароль, использованный вами для входа, " +
                    "является временным, потому известен администратору и может быть им изменен.\nРекомендуем вам придумать свой пароль. Его не сможет увидеть и изменить никто, кроме вас.", "Завершение активации", "Установить пароль", "Не сейчас") == DialogStatus.Yes)
                {
                    SetPassword sp = new();
                    sp.ShowDialog("Установление нового пароля", Classes.Icon.UserGears);
                }
            }
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
            ProgramState.CloseSession();
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
            this.AddPageButton("Простые документы", Classes.Icon.FileRichText, Controls.MainWindowButtonTab.Templates);
            //this.AddPageButton("База данных", Classes.Icon.Database, Controls.MainWindowButtonTab.CustomDatabase);
            using (Class cl = new())
            {
                foreach (string category in cl.GetCategories())
                {
                    this.AddPageButton(category, Classes.Icon.Database, Controls.MainWindowButtonTab.ClassCategoryPrefix + category);
                }
            }
            if (ProgramState.CurrentUserParameters.Permission_group == PermissionGroup.Admin)
            {
                this.AddAdminPageButton("Рабочее пространство", Classes.Icon.GearWide, Controls.MainWindowButtonTab.WorkspaceSettings);
                this.AddAdminPageButton("Пользователи", Classes.Icon.UserGears, Controls.MainWindowButtonTab.UsersSettings);
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
            this.AddTabItem(item, id, name, TabType.Usual);
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

        private void TabItem_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.TabMain.Items.Remove(sender);
            if (this.TabMain.Items.Count == 0)
            {
                this.Close();
            }
        }
    }
}
