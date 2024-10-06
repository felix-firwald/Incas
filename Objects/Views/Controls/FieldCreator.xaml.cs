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
        public delegate bool FieldAction(FieldCreator t);
        public delegate int FieldMoving(FieldCreator t);
        public event FieldAction OnRemove;
        public event FieldMoving OnMoveDownRequested;
        public event FieldMoving OnMoveUpRequested;
        public FieldViewModel vm;
        public FieldCreator(int index, Incas.Objects.Models.Field data = null)
        {
            this.InitializeComponent();
            if (data == null)
            {
                this.vm = new();
                this.vm.NameOfField = "Новое_поле";
            }
            else
            {
                this.vm = new(data);
            }
            this.vm.OrderNumber = index;
            this.DataContext = this.vm;
            this.ExpanderButton.IsChecked = true;
        }
        public Models.Field GetField()
        {
            this.vm.CheckField();
            return this.vm.Source;
        }
        public void DublicateName()
        {
            this.vm.VisibleName = this.vm.NameOfField;
        }
        public void Maximize()
        {
            this.ExpanderButton.IsChecked = true;           
        }
        public void Minimize()
        {
            this.ExpanderButton.IsChecked = false;           
        }

        private void MaximizeClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.MainBorder.Height = this.ContentPanel.Height + 40;
            //this.TitleUp.Visibility = Visibility.Collapsed;
            //this.TitleDown.Visibility = Visibility.Collapsed;
        }

        private void MinimizeClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.MainBorder.Height = 40;
            //this.TitleUp.Visibility = Visibility.Visible;
            //this.TitleDown.Visibility = Visibility.Visible;
        }

        private void RemoveClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DialogsManager.ShowQuestionDialog(
                "Вы уверены, что хотите удалить поле? Отменить это действие после сохранения будет невозможно.",
                "Удалить поле?",
                "Удалить", "Не удалять") == Core.Views.Windows.DialogStatus.Yes)
            {
                this.OnRemove?.Invoke(this);
            }           
        }

        private void UpClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.vm.OrderNumber = (int)this.OnMoveDownRequested?.Invoke(this);
        }

        private void DownClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.vm.OrderNumber = (int)this.OnMoveUpRequested?.Invoke(this);
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
            string name = $"Настройки поля [{f.Name}]";
            switch (f.Type)
            {
                case Components.FieldType.Variable:
                    TextFieldSettings tf = new(f);
                    tf.ShowDialog(name, Icon.Sliders);
                    break;
                case Components.FieldType.Text:
                    TextBigFieldSettings tb = new(f);
                    tb.ShowDialog(name, Icon.Sliders);
                    break;
                case Components.FieldType.Number:
                    NumberFieldSettings n = new(f);
                    n.ShowDialog(name, Icon.Sliders);
                    break;
                case Components.FieldType.Relation:
                    Objects.Components.BindingData db = DialogsManager.ShowBindingDialog(f.Value);
                    f.Value = JsonConvert.SerializeObject(db);
                    break;
                case Components.FieldType.LocalEnumeration:
                    LocalEnumerationFieldSettings le = new(f);
                    le.ShowDialog(name, Icon.Sliders);
                    break;
                case Components.FieldType.GlobalEnumeration:
                    GlobalEnumerationFieldSettings ge = new(f);
                    ge.ShowDialog(name, Icon.Sliders);
                    break;
                case Components.FieldType.LocalConstant:
                    ConstantFieldSettings cf = new(f);
                    cf.ShowDialog(name, Icon.Sliders);
                    break;
                case Components.FieldType.GlobalConstant:
                    GlobalConstantFieldSettings gc = new(f);
                    gc.ShowDialog(name, Icon.Sliders);
                    break;
                case Components.FieldType.HiddenField:
                    ConstantFieldSettings hf = new(f);
                    hf.ShowDialog(name, Icon.Sliders);
                    break;
                case Components.FieldType.Date:
                    DateFieldSettings dt = new(f);
                    dt.ShowDialog(name, Icon.Sliders);
                    break;
                case Components.FieldType.Generator:
                    break;
            }
        }
    }
}
