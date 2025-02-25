using Incas.Core.Classes;
using Incas.Core.Interfaces;
using Incas.Objects.ViewModels;
using IncasEngine.Core;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.Workspace;
using System;
using System.Windows.Controls;
using static Incas.Core.Interfaces.ITabItem;

namespace Incas.Objects.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для CustomDatabaseMain.xaml
    /// </summary>
    public partial class CustomDatabaseMain : System.Windows.Controls.UserControl, ITabItem
    {
        public CustomDatabaseViewModel vm;
        public event TabAction OnClose;
        public string Id { get; set; }
        public CustomDatabaseMain()
        {
            this.InitializeComponent();
            this.vm = new();
            this.ContentPanel.Content = new Core.Views.Controls.NoContent();
            this.vm.OnClassSelected += this.OnClassSelected;
            this.DataContext = this.vm;
        }
        public CustomDatabaseMain(WorkspaceComponent component)
        {
            this.InitializeComponent();
            this.vm = new()
            {
                SelectedCategory = component
            };
            this.CategoriesList.IsEnabled = false;
            this.CategoriesList.Visibility = System.Windows.Visibility.Collapsed;
            this.vm.OnClassSelected += this.OnClassSelected;
            this.DataContext = this.vm;
            EngineEvents.OnUpdateClassRequested += this.EngineEvents_OnUpdateClassRequested;
        }

        private void EngineEvents_OnUpdateClassRequested(IClass @class, bool requiredLock)
        {
            for (int i = 0; i < this.vm.Classes.Count; i++)
            {
                if (this.vm.Classes[i].Id == @class.Id)
                {
                    this.vm.Classes[i] = @class as Class;
                    break;
                }
            }
        }

        private void OnClassSelected(Class selectedClass)
        {
            this.ContentPanel.Content = new ObjectsListLoading();
            if (selectedClass == null)
            {
                this.ContentPanel.Content = new Core.Views.Controls.NoContent();
                return;
            }
            IClassData data = selectedClass.GetClassData();
            if (data.RestrictFullView)
            {
                
            }
            else
            {
                ObjectsList ol = new(selectedClass);
                this.ContentPanel.Content = ol;
            }            
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.vm.UpdateAll();
            this.ContentPanel.Content = new Core.Views.Controls.NoContent();
        }

        private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.vm.SelectedClass == null)
            {
                return;
            }
            GroupBox gb = new()
            {
                Header = this.vm.SelectedCategory + ": " + this.vm.SelectedClass.Name,
                Content = new ObjectsList(this.vm.SelectedClass)
            };
            DialogsManager.ShowPage(gb, this.vm.SelectedClass.Name, this.vm.SelectedClass.Id.ToString());
        }
    }
}
