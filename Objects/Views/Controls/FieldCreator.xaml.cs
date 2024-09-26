using DocumentFormat.OpenXml.Wordprocessing;
using Incas.Core.Classes;
using Incas.Objects.AutoUI;
using Incas.Objects.Models;
using Incas.Objects.ViewModels;
using Newtonsoft.Json;
using System.Windows;
using System.Windows.Controls;

namespace Incas.Objects.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для FieldCreator.xaml
    /// </summary>
    public partial class FieldCreator : UserControl
    {
        public delegate void FieldAction(FieldCreator t);
        public event FieldAction onDelete;
        public FieldViewModel vm;
        public FieldCreator(Incas.Objects.Models.Field data = null)
        {
            this.InitializeComponent();
            if (data == null)
            {
                this.vm = new();
            }
            else
            {
                this.vm = new(data);
            }           
            this.DataContext = this.vm;
        }

        private void MaximizeClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.MainBorder.Height = this.ContentPanel.Height + 40;
            this.NumberUp.Visibility = Visibility.Collapsed;
        }

        private void MinimizeClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.MainBorder.Height = 40;
            this.NumberUp.Visibility = Visibility.Visible;
        }

        private void RemoveClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.onDelete?.Invoke(this);
        }

        private void UpClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.vm.IncrementOrder();
        }

        private void DownClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.vm.DecrementOrder();
        }

        private void EditScriptClick(object sender, RoutedEventArgs e)
        {

        }

        private void CopyNameToVisible(object sender, RoutedEventArgs e)
        {

        }

        private void OpenSettingsClick(object sender, RoutedEventArgs e)
        {
            Incas.Objects.Models.Field f = this.vm.Source;
            string name = $"Настройки поля ({f.Name})";
            switch (f.Type)
            {
                case Templates.Components.TagType.Variable:
                    TextFieldSettings tf = new(f);
                    tf.ShowDialog(name, Icon.Sliders);
                    break;
                case Templates.Components.TagType.Text:
                    TextBigFieldSettings tb = new(f);
                    tb.ShowDialog(name, Icon.Sliders);
                    break;
                case Templates.Components.TagType.Number:
                    NumberFieldSettings n = new(f);
                    n.ShowDialog(name, Icon.Sliders);
                    break;
                case Templates.Components.TagType.Relation:
                    Objects.Components.BindingData db = DialogsManager.ShowBindingDialog(f.Value);
                    f.Value = JsonConvert.SerializeObject(db);
                    break;
                case Templates.Components.TagType.LocalEnumeration:
                    LocalEnumerationFieldSettings le = new(f);
                    le.ShowDialog(name, Icon.Sliders);
                    break;
                case Templates.Components.TagType.GlobalEnumeration:
                    GlobalEnumerationFieldSettings ge = new(f);
                    ge.ShowDialog(name, Icon.Sliders);
                    break;
                case Templates.Components.TagType.LocalConstant:
                    ConstantFieldSettings cf = new(f);
                    cf.ShowDialog(name, Icon.Sliders);
                    break;
                case Templates.Components.TagType.GlobalConstant:
                    GlobalConstantFieldSettings gc = new(f);
                    gc.ShowDialog(name, Icon.Sliders);
                    break;
                case Templates.Components.TagType.HiddenField:
                    ConstantFieldSettings hf = new(f);
                    hf.ShowDialog(name, Icon.Sliders);
                    break;
                case Templates.Components.TagType.Date:
                    DateFieldSettings dt = new(f);
                    dt.ShowDialog(name, Icon.Sliders);
                    break;
                case Templates.Components.TagType.Generator:
                    break;
            }
        }
    }
}
