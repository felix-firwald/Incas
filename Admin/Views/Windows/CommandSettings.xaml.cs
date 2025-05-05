using Incas.Admin.ViewModels;
using Incas.Core.Classes;
using Incas.Core.Views.Controls;
using Incas.Core.Views.Windows;
using IncasEngine.Core.ExtensionMethods;
using IncasEngine.Workspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Incas.Admin.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для CommandSettings.xaml
    /// </summary>
    public partial class CommandSettings : Window
    {
        private CommandsSettingsViewModel vm;
        public CommandSettings()
        {
            this.InitializeComponent();
            this.vm = new();
            this.DataContext = vm;
        }
        private void AddCommandClick(object sender, RoutedEventArgs e)
        {
            WorkspaceMenuCommand c = new();
            c.Name = $"Команда {this.vm.Commands.Count + 1}";
            c.Description = "<Нет описания>";
            c.Icon = "M181.23-180q-21.07 0-31.54-18.58-10.46-18.58 1-36.65l298.77-478.08q10.85-17.07 30.54-17.07 19.69 0 30.54 17.07l298.77 478.08q11.46 18.07 1 36.65Q799.84-180 778.77-180H181.23ZM224-240h512L480-650 224-240Zm256-205Z";
            c.Color = new() { R = 255, G = 255, B = 255 };
            this.vm.AddCommand(c);
        }
        private void RemoveCommandClick(object sender, RoutedEventArgs e)
        {
            if (this.vm.SelectedCommand is null)
            {
                return;
            }
            this.vm.Commands.Remove(this.vm.SelectedCommand);
        }

        private void MinimizeAllClick(object sender, RoutedEventArgs e)
        {

        }

        private void MaximizeAllClick(object sender, RoutedEventArgs e)
        {

        }

        private void GetMoreInfoClick(object sender, RoutedEventArgs e)
        {

        }
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            try
            {
                this.vm.Save();
                this.Close();
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
            }           
        }

        private void ResetIconClick(object sender, RoutedEventArgs e)
        {
            if (this.vm.SelectedCommand is null)
            {
                return;
            }
            IconSelector iconSelector = new();
            iconSelector.ShowDialog();
            if (iconSelector.IsSelected == true)
            {
                this.vm.SelectedCommand.SelectedIconPath = iconSelector.SelectedIconPath;
            }
        }

        private void MoveUp(object sender, RoutedEventArgs e)
        {
            CommandViewModel selected = this.vm.SelectedCommand;
            if (selected is null)
            {
                return;
            }
            this.vm.Commands.MoveUp(selected);
            this.vm.SelectedCommand = selected;
        }

        private void MoveDown(object sender, RoutedEventArgs e)
        {
            CommandViewModel selected = this.vm.SelectedCommand;
            if (selected is null)
            {
                return;
            }
            this.vm.Commands.MoveDown(selected);
            this.vm.SelectedCommand = selected;
        }

        private void OpenScriptClick(object sender, RoutedEventArgs e)
        {
            if (this.vm.SelectedCommand is null)
            {
                return;
            }
            CommandScript cs = new(this.vm.SelectedCommand.Name, this.vm.SelectedCommand.Script);
            cs.ShowDialog();
            this.vm.SelectedCommand.Script = cs.Code.Text;
        }
    }
}
