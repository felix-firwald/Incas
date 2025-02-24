using Incas.Core.Classes;
using Incas.Core.ViewModels;
using System.Windows;

namespace Incas.Core.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для IconSelector.xaml
    /// </summary>
    public partial class IconSelector : Window
    {
        public Icon SelectedIcon
        {
            get
            {
                return this.vm.SelectedIcon.Key;
            }
        }
        public string SelectedIconPath
        {
            get
            {
                return this.vm.SelectedIcon.Value;
            }
        }
        private bool isSelected = false;
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
        }
        private IconSelectorViewModel vm { get; set; }
        public IconSelector()
        {
            this.InitializeComponent();
            this.vm = new();
            this.DataContext = this.vm;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (this.vm.SelectedIcon.Value is null)
            {
                DialogsManager.ShowExclamationDialog("Иконка не выбрана!", "Действие невозможно");
                return;
            }
            this.isSelected = true;
            this.Close();
        }
    }
}
