using Incas.Core.AutoUI;
using Incas.Core.Classes;
using Incas.Core.Interfaces;
using Incas.Core.ViewModels;
using Incas.Objects.Models;
using System;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

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
            if (string.IsNullOrEmpty(ProgramState.CurrentUserParameters.password))
            {
                if (DialogsManager.ShowQuestionDialog("Текущий пароль, использованный вами для входа, " +
                    "является временным, потому известен администратору и может быть им изменен.\nРекомендуем вам придумать свой пароль. Его не сможет увидеть и изменить никто, кроме вас.", "Завершение активации", "Установить пароль", "Не сейчас") == DialogStatus.Yes)
                {
                    SetPassword sp = new();
                    sp.ShowDialog("Установление нового пароля", Classes.Icon.UserGears);
                }
            }
            this.PlayEasterEgg();
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
                    this.vm.DoOpenFile("Pdf");
                    break;
                case Key.F4:
                    this.vm.DoOpenFile("Word");
                    break;
                case Key.F5:
                    this.vm.DoOpenFile("Excel");
                    break;
                case Key.F6:
                    this.vm.DoOpenWeb("");
                    break;
            }
        }
        public void UpdateTabs()
        {
            using Class cl = new();
            foreach (string category in cl.GetCategories())
            {
                this.AddPage(category, new Objects.Views.Pages.CustomDatabaseMain(category));
            }
        }
        private void AddPage(string name, UserControl control)
        {
            RadioButton rb = new()
            {
                Style = this.FindResource("MenuButton") as Style,
                Content = name,
                GroupName = "Tabs",
                Name = name
            };
            this.CustomTabs.Children.Add(rb);
            Binding binding = new()
            {
                ElementName = name,
                Path = new("IsChecked")
            };
            TabItem tabItem = new()
            {
                Content = control,
                Background = null,
                BorderBrush = null
            };
            BindingOperations.SetBinding(tabItem, TabItem.IsSelectedProperty, binding);
            this.TabMain.Items.Add(tabItem);
        }
    }
}
