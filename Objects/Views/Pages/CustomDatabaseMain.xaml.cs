using Incas.Core.Classes;
using Incas.Core.Interfaces;
using Incas.Objects.Components;
using Incas.Objects.ViewModels;
using Incas.Objects.Views.Windows;
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
        public CustomDatabaseMain(string category)
        {
            this.InitializeComponent();
            this.vm = new()
            {
                SelectedCategory = category
            };
            this.CategoriesList.IsEnabled = false;
            this.CategoriesList.Visibility = System.Windows.Visibility.Collapsed;
            this.vm.OnClassSelected += this.OnClassSelected;
            this.vm.OnPresetSelected += this.OnPresetSelected;
            this.DataContext = this.vm;
        }

        private void OnPresetSelected(Models.Class selectedClass, Components.PresetReference preset)
        {
            if (selectedClass == null)
            {
                this.ContentPanel.Content = new Core.Views.Controls.NoContent();
                return;
            }
            this.ContentPanel.Content = new ObjectsList(selectedClass, ObjectProcessor.GetPreset(selectedClass, preset));
        }

        private void OnClassSelected(Models.Class selectedClass)
        {
            if (selectedClass == null)
            {
                this.ContentPanel.Content = new Core.Views.Controls.NoContent();
                return;
            }
            this.ContentPanel.Content = new ObjectsList(selectedClass);
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.vm.UpdateAll();
        }

        private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.vm.SelectedClass == null)
            {
                return;
            }
            GroupBox gb = new()
            {
                Header = this.vm.SelectedCategory + ": " + this.vm.SelectedClass.name,
                Content = new ObjectsList(this.vm.SelectedClass)
            };
            DialogsManager.ShowPage(gb, this.vm.SelectedClass.name, this.vm.SelectedClass.identifier.ToString());
        }

        private void AddPresetClick(object sender, System.Windows.RoutedEventArgs e)
        {
            AddPreset ap = new(this.vm.SelectedClass);
            ap.ShowDialog();
        }
    }
}
