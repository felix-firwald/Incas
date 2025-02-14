using Incas.Core.Classes;
using Incas.License.Components;
using IncasEngine.Core;
using Newtonsoft.Json;
using PdfSharp.Snippets.Pdf;
using System;
using System.Windows;
using System.Windows.Input;

namespace Incas.License.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для LicenseDialog.xaml
    /// </summary>
    public partial class LicenseDialog : Window
    {
        private const string link = "https://forms.yandex.ru/u/6748602702848f55e61de00e/";
        public LicenseDialog()
        {
            this.InitializeComponent();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            UniqueComputerIdentifier uci = new()
            {
                MachineName = Environment.MachineName,
                UserDomainName = Environment.UserDomainName,
                UniqueMachineId = EngineGlobals.GetUMI()
            };
            string result = JsonConvert.SerializeObject(uci);
            result = Cryptographer.ToDifficultHex(result);
            ProgramState.OpenWebPage(link + $"?uci={result}");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string path = RegistryData.GetPathToLicense();
            if (DialogsManager.ShowOpenFileDialog(ref path, "Файл лицензии|*" + License.Extension))
            {
                this.HandleFile(path);
            }
        }

        private void HandleFile(string path)
        {
            if (path.EndsWith(License.Extension))
            {
                RegistryData.SetPathToLicense(path);
                if (EngineGlobals.CheckLicense() == true)
                {
                    this.DialogResult = true;
                    this.Close();
                }
            }           
        }

        private void YesClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Border_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                this.HandleFile(files[0]);
            }
        }
    }
}
