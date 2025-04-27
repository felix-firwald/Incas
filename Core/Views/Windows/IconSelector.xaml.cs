using Incas.Core.Classes;
using Incas.Core.ViewModels;
using Incas.Tests.AutoUI;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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
                switch (this.vm.Mode)
                {
                    case IconSelectorViewModel.SelectorMode.PredefinedIcons:
                        return this.vm.SelectedIcon.Value;
                }
                return this.vm.CustomSelectedIcon;
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
            switch (this.vm.Mode)
            {
                case IconSelectorViewModel.SelectorMode.PredefinedIcons:
                    if (this.vm.SelectedIcon.Value is null)
                    {
                        DialogsManager.ShowExclamationDialog("Иконка не выбрана!", "Действие невозможно");
                        return;
                    }
                    this.isSelected = true;
                    this.Close();
                    break;
                case IconSelectorViewModel.SelectorMode.CustomIcon:
                    if (this.vm.CustomSelectedIcon is null)
                    {
                        DialogsManager.ShowExclamationDialog("Иконка не выбрана!", "Действие невозможно");
                        return;
                    }
                    this.isSelected = true;
                    this.Close();
                    break;
            }            
        }

        private void LoadFromClipboard(object sender, RoutedEventArgs e)
        {
            string geo = ViewExtensions.TryGetGeometryFromSvgPath(Clipboard.GetText());
            if (geo == null)
            {
                DialogsManager.ShowExclamationDialog("Не удалось распознать SVG-путь иконки.", "Ошибка парсинга");
                return;
            }
            this.vm.CustomSelectedIcon = geo;
            this.vm.Mode = IconSelectorViewModel.SelectorMode.CustomIcon;            
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    DialogsManager.ShowWebPage("https://fonts.google.com/icons?icon.style=Rounded");
                    break;
            }
        }
    }
}
