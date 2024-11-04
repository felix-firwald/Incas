using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using Incas.Objects.Components;
using Incas.Objects.Views.Windows;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incas.Core.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для SelectionBox.xaml
    /// </summary>
    public partial class SelectionBox : UserControl
    {
        public Objects.Components.Object SelectedObject { get; set; }
        public string Value => this.Input.Text;
        private readonly BindingData Binding;
        public delegate void ValueChanged(object sender, TextChangedEventArgs e);
        public event ValueChanged OnValueChanged;
        public SelectionBox(BindingData bd)
        {
            this.InitializeComponent();
            this.Binding = bd;
        }
        public async void SetObject(string guid)
        {
            try
            {
                Guid id = Guid.Parse(guid);
                if (id != Guid.Empty)
                {
                    await Task.Run(() =>
                    {
                        this.SelectedObject = ObjectProcessor.GetObject(new(this.Binding.Class), id);                       
                    });
                    this.Input.Text = await this.SelectedObject.GetFieldValue(this.Binding.Field);
                    this.SetAsEditingEnabled();
                }
            }
            catch { }
        }
        public async void SetObject(Guid id)
        {
            try
            {
                if (id != Guid.Empty)
                {
                    await Task.Run(() =>
                    {
                        this.SelectedObject = ObjectProcessor.GetObject(new(this.Binding.Class), id);
                    });
                    this.Input.Text = await this.SelectedObject.GetFieldValue(this.Binding.Field);
                    this.SetAsEditingEnabled();
                }
            }
            catch { }
        }

        private async void ButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (this.Binding.Class == Guid.Empty || this.Binding.Field == Guid.Empty)
            {
                DialogsManager.ShowExclamationDialog("Не определена привязка к классу объектов!", "Действие прервано");
                return;
            }
            DatabaseSelection s = new(this.Binding);
            s.ShowDialog();
            if (s.Result == DialogStatus.Yes)
            {
                try
                {
                    this.SelectedObject = s.GetSelectedObject();
                    this.Input.Text = await this.SelectedObject.GetFieldValue(this.Binding.Field);
                    this.SetAsEditingEnabled();
                }
                catch (Exception ex)
                {
                    DialogsManager.ShowErrorDialog(ex.Message);
                }
            }
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnValueChanged?.Invoke(this, e);
        }

        private void FindClick(object sender, MouseButtonEventArgs e)
        {
            this.Hints.IsOpen = true;
            this.HintsSearchField.Focusable = true;
            this.HintsSearchField.Focus();
        }

        private void CancelSearch(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Hints.IsOpen = false;
        }

        private void HintsSearchField_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void HintsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void SetAsEditingEnabled()
        {
            this.AddIcon.Visibility = System.Windows.Visibility.Collapsed;
            this.EditIcon.Visibility = System.Windows.Visibility.Visible;
        }
        private void SetAsCreatingEnabled()
        {
            this.AddIcon.Visibility = System.Windows.Visibility.Visible;
            this.EditIcon.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void EditClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Objects.Models.Class cl = new(this.Binding.Class);
                cl.GetClassData();
                ObjectsEditor oe = new(cl, [this.SelectedObject]);
                oe.OnUpdateRequested += this.Oe_OnUpdateRequested;
                oe.ShowDialog();
            }
            catch { }
        }

        private void Oe_OnUpdateRequested()
        {
            this.SetObject(this.SelectedObject.Id);
        }

        private void AddClick(object sender, MouseButtonEventArgs e)
        {
            ObjectsEditor oe = new(new(this.Binding.Class));
            oe.SetSingleObjectMode();
            oe.OnSetNewObjectRequested += this.Oe_OnSetNewObjectRequested;
            oe.ShowDialog();
        }

        private void Oe_OnSetNewObjectRequested(Guid id)
        {
            this.SetObject(id);
        }
    }
}
