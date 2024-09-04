using Incas.Core.Classes;
using Incas.Core.ViewModels;
using Incas.Users.Views.Windows;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.ApplicationModel.Core;
using Avalonia.Controls.Chrome;
using System.Media;
using Incas.Core.AutoUI;

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
            this.vm = new MainWindowViewModel();
            ProgramState.MainWindow = this.vm;
            this.DataContext = this.vm;
            if (string.IsNullOrEmpty(ProgramState.CurrentUserParameters.password))
            {
                if (DialogsManager.ShowQuestionDialog("Текущий пароль, использованный вами для входа, " +
                    "является временным, потому известен администратору и может быть им изменен.\nРекомендуем вам придумать свой пароль. Его не сможет увидеть и изменить никто, кроме вас.", "Завершение активации", "Установить пароль", "Не сейчас") == DialogStatus.Yes)
                {
                    SetPassword sp = new();
                    DialogsManager.ShowSimpleFormDialog(sp, "Установление нового пароля", Classes.Icon.UserGears);
                }
            }

            this.PlayEasterEgg();
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
                    this.vm.DoCopyFile("");
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
    }
}
