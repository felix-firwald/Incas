using Common;
using Incubator_2.ViewModels;
using Incubator_2.Windows;
using System;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Input;

namespace Incubator_2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MV_MainWindow vm;
        public MainWindow()
        {
            InitializeComponent();
            if (!ProgramState.CheckSensitive())
            {
                System.Windows.Application.Current.Shutdown();
            }
            this.vm = new MV_MainWindow();
            ProgramState.MainWindow = this.vm;
            this.DataContext = this.vm;
            if (string.IsNullOrEmpty(ProgramState.CurrentUserParameters.password))
            {
                if (ProgramState.ShowQuestionDialog("Текущий пароль, использованный вами для входа, " +
                    "является временным, потому известен администратору и может быть им изменен.\nРекомендуем вам придумать свой пароль. Его не сможет увидеть и изменить никто, кроме вас.", "Завершение активации", "Установить пароль", "Не сейчас") == DialogStatus.Yes)
                {
                    SetPassword windowSet = new();
                    windowSet.ShowDialog();
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
                    SoundPlayer myNewSound = new SoundPlayer(stream);
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
                    this.vm.DoCopyToClipBoard("");
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
