using Common;
using DocumentFormat.OpenXml.InkML;
using Incubator_2.Common;
using Incubator_2.ViewModels;
using Incubator_2.Windows;
using Incubator_2.Windows.ToolBar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Media;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using Windows.UI.Notifications;

namespace Incubator_2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MV_MainWindow vm;
        public MainWindow()
        {
            InitializeComponent();
            if (!ProgramState.CheckSensitive())
            {
                System.Windows.Application.Current.Shutdown();
            }
            vm = new MV_MainWindow();
            ProgramState.MainWindow = vm;
            this.DataContext = vm;
            if (string.IsNullOrEmpty(ProgramState.CurrentUserParameters.password))
            {
                if (ProgramState.ShowQuestionDialog("Текущий пароль, использованный вами для входа, " +
                    "является временным, потому известен администратору и может быть им изменен.\nРекомендуем вам придумать свой пароль. Его не сможет увидеть и изменить никто, кроме вас.", "Завершение активации", "Установить пароль", "Не сейчас") == DialogStatus.Yes)
                {
                    SetPassword windowSet = new();
                    windowSet.ShowDialog();
                }
            }
            
            PlayEasterEgg();
        }
        private void PlayEasterEgg(string name = "Chicken")
        {
            try
            {
                if (RegistryData.IsRoosterExists())
                {
                    using (FileStream stream = File.Open($"Static\\{name}.wav", FileMode.Open))
                    {
                        SoundPlayer myNewSound = new SoundPlayer(stream);
                        myNewSound.Load();
                        myNewSound.Play();
                    }
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

        private void WindowMinimize(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void WindowMaximize(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            }
        }
        private void WindowClose(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void WindowMove(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch
            {
                
            }
        }

        private void Logo_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PlayEasterEgg("Rooster");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы", Justification = "<Ожидание>")]
        private void DashClick(object sender, RoutedEventArgs e)
        {
            string target = ProgramState.ShowActiveUserSelector("Выберите пользователя для отправки процесса.").slug;
            if (!string.IsNullOrEmpty(target))
            {
                Dictionary<string, string> filters = new()
                {
                    { "Excel", "Файлы Excel|*.xls;*.xlsx;*.xlsm" },
                    { "Word", "Файлы Word|*.doc;*.docx" },
                    { "Pdf", "Файлы PDF|*.pdf" }
                };
                string tag = ((System.Windows.Controls.Button)sender).Tag.ToString();
                switch (tag)
                {
                    case "Clipboard":
                        string result = ProgramState.ShowInputBox("Текст для буфера обмена", "Укажите текст для буфера обмена");
                        ServerProcessor.SendCopyTextProcess(result, target);
                        break;
                    case "File":
                        OpenFileDialog of = new();
                        if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            ServerProcessor.SendCopyFileProcess(of.SafeFileName, of.FileName, target);
                        }
                        break;
                    case "Word":
                    case "Excel":
                    case "Pdf":
                        OpenFileDialog of2 = new();
                        of2.Filter = filters[tag];
                        if (of2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            ServerProcessor.SendOpenFileProcess(of2.SafeFileName, of2.FileName, target);
                        }
                        break;
                    case "Web":
                        string url = ProgramState.ShowInputBox("Укажите адрес");
                        if (!url.StartsWith("https://"))
                        {
                            ProgramState.ShowExclamationDialog("Введенный адрес либо не является адресом сети, либо небезопасен.", "Действие прервано");
                            return;
                        }
                        ServerProcessor.SendOpenWebProcess(url, target);
                        break;
                }
            }
        }

        private void HandleCommandClick(object sender, RoutedEventArgs e)
        {
            CommandHandler.Handle(ProgramState.ShowInputBox("Введите команду"));
        }

        private void Logo_MouseDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
