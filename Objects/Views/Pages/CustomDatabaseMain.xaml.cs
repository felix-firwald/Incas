using Incas.Core.Classes;
using Incas.Core.Interfaces;
using Incas.Objects.Components;
using Incas.Objects.Engine;
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
            ObjectsList ol = new(selectedClass, Processor.GetPreset(selectedClass, preset));
            ol.OnPresetsViewRequested += this.OnPresetsViewRequested;
            this.ContentPanel.Content = ol;
        }
        private void PlacePresetsListPage(IClass selectedClass)
        {
            PresetsListPage pl = new(selectedClass, this.vm.Presets);
            pl.OnViewRequested += this.Pl_OnViewRequested;
            this.vm.UpdatePresets();
            this.vm.SelectedPreset = new();
            this.ContentPanel.Content = pl;
        }
        private void OnClassSelected(Models.Class selectedClass)
        {
            if (selectedClass == null)
            {
                this.ContentPanel.Content = new Core.Views.Controls.NoContent();
                return;
            }
            Models.ClassData data = selectedClass.GetClassData();
            if (data.PresetsEnabled && data.RestrictFullView)
            {
                this.PlacePresetsListPage(selectedClass);
            }
            else
            {
                ObjectsList ol = new(selectedClass);
                ol.OnPresetsViewRequested += this.OnPresetsViewRequested;
                this.ContentPanel.Content = ol;
            }            
        }

        private void OnPresetsViewRequested(IClass source)
        {
            this.PlacePresetsListPage(source);
        }

        private void Pl_OnViewRequested(IClass sourceClass, Preset preset)
        {
            this.vm.SelectedPreset = preset.GetAsReference();
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

        private void Ap_OnUpdateRequested()
        {
            this.vm.UpdatePresets();
        }
    }
}
