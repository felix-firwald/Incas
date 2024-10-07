using Incas.Core.Classes;
using Incas.Objects.AutoUI;
using Incas.Objects.Components;
using Incas.Objects.ViewModels;
using Incas.Objects.Views.Windows;
using Newtonsoft.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incas.Objects.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для TableColumnCreator.xaml
    /// </summary>
    public partial class TableColumnCreator : UserControl
    {
        public TableColumnViewModel vm { get; set; }
        public TableColumnCreator(TableFieldColumnData col)
        {
            this.InitializeComponent();
            this.vm = new(col);
            this.DataContext = this.vm;
        }
        public TableFieldColumnData GetField()
        {
            return this.vm.FieldData;
        }

        private void RemoveClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void UpClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void DownClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void MaximizeClick(object sender, RoutedEventArgs e)
        {

        }

        private void MinimizeClick(object sender, RoutedEventArgs e)
        {

        }

        private void OpenSettingsClick(object sender, RoutedEventArgs e)
        {
            TableFieldColumnData f = this.vm.FieldData;
            string name = $"Настройки поля [{f.Name}]";
            switch (f.FieldType)
            {
                case Components.FieldType.LocalEnumeration:
                    LocalEnumerationColumnSettings le = new(f);
                    le.ShowDialog(name, Icon.Sliders, DialogSimpleForm.Components.IconColor.Yellow);
                    break;
                case Components.FieldType.GlobalEnumeration:
                    GlobalEnumerationColumnSettings ge = new(f);
                    ge.ShowDialog(name, Icon.Sliders, DialogSimpleForm.Components.IconColor.Yellow);
                    break;
            }
        }
    }
}
