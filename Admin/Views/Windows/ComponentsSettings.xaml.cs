using Avalonia.Controls;
using Incas.Admin.ViewModels;
using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using IncasEngine.ObjectiveEngine.Models;
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
    /// Логика взаимодействия для ComponentsSettings.xaml
    /// </summary>
    public partial class ComponentsSettings : System.Windows.Window
    {
        private ComponentsSettingsViewModel vm;
        public ComponentsSettings()
        {
            this.InitializeComponent();
            this.vm = new();
            this.DataContext = vm;
        }

        private void AddComponentClick(object sender, RoutedEventArgs e)
        {
            WorkspaceComponent wc = new()
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                IsVisibleToEveryone = true,
                Color = IncasEngine.Core.Color.FromRGB(255, 255, 255),
                Icon = "M210.46-200q-19.15 0-28.08-16.65-8.92-16.66 1-32.81l269.54-431.16q9.7-15.15 27.08-15.15t27.08 15.15l269.54 431.16q9.92 16.15 1 32.81Q768.69-200 749.54-200H210.46ZM224-240h512L480-650 224-240Zm256-205Z"
            };
            ComponentViewModel cvm = new(wc);
            this.vm.Components.Add(cvm);
            this.vm.SelectedComponent = cvm;
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

        }

        private void RemoveComponentClick(object sender, RoutedEventArgs e)
        {
            if (this.vm.SelectedComponent.Component.Indestructible)
            {
                DialogsManager.ShowExclamationDialog("Основной компонент рабочего пространства не может быть удален.", "Действие невозможно");
                return;
            }
            Class cl = new();
            List<ClassItem> items = cl.GetAllClassItemsByComponent(this.vm.SelectedComponent.Component);
            if (items.Count > 0)
            {
                DialogsManager.ShowExclamationDialog($"Этот компонент не может быть удален, поскольку он содержит в себе классы. Количество классов: {items.Count}.", "Действие невозможно");
                return;
            }
            this.vm.Components.Remove(this.vm.SelectedComponent);
        }

        private void ResetIconClick(object sender, RoutedEventArgs e)
        {
            IconSelector selector = new();
            selector.ShowDialog();
            if (selector.IsSelected)
            {
                this.vm.SelectedComponent.SelectedIconPath = selector.SelectedIconPath;
            }
        }
    }
}
