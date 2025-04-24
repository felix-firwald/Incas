using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using Incas.Objects.Views.Windows;
using IncasEngine.Core;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Incas.Core.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для SelectionBoxGrid.xaml
    /// </summary>
    public partial class SelectionBoxGrid : UserControl
    {
        public SelectionBoxGrid()
        {
            this.InitializeComponent();
        }
        public static readonly DependencyProperty SelectedObjectProperty =
            DependencyProperty.Register(
                "SelectedObject",  // Имя свойства
                typeof(IObject), // Тип данных свойства
                typeof(SelectionBoxGrid), // Тип владельца свойства (UserControl)
                new FrameworkPropertyMetadata(  // Метаданные свойства
                    null,  // Значение по умолчанию
                    FrameworkPropertyMetadataOptions.AffectsRender, // Опции, указывающие, что изменение свойства влияет на рендеринг
                    new PropertyChangedCallback(OnMyPropertyChanged) // Callback-функция для уведомления об изменении свойства
                )
            );
        public static readonly DependencyProperty TargetFieldProperty =
            DependencyProperty.Register(
                "TargetField",  // Имя свойства
                typeof(BindingData), // Тип данных свойства
                typeof(SelectionBoxGrid), // Тип владельца свойства (UserControl)
                new FrameworkPropertyMetadata(  // Метаданные свойства
                    null,  // Значение по умолчанию
                    FrameworkPropertyMetadataOptions.AffectsRender // Опции, указывающие, что изменение свойства влияет на рендеринг
                    
                )
            );
        public BindingData TargetField
        {
            get => (BindingData)this.GetValue(TargetFieldProperty);
            set => this.SetValue(TargetFieldProperty, value);
        }
        public IObject SelectedObject
        {
            get => (IObject)this.GetValue(SelectedObjectProperty);
            set => this.SetValue(SelectedObjectProperty, value);
        }

        private static void OnMyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Дополнительная логика, которую нужно выполнить при изменении свойства
            SelectionBoxGrid control = (SelectionBoxGrid)d;
            if (e.NewValue is IObject selected)
            {
                control.Input.Text = selected[control.TargetField.BindingField].Value.ToString();
            }
            else
            {
                control.Input.Text = string.Empty;
            }
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
                        this.SelectedObject = Processor.GetObject(EngineGlobals.GetClass(this.TargetField.BindingClass), id);
                    });
                    this.Input.Text = await this.SelectedObject.GetFieldValue(this.TargetField.BindingField);
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
                        this.SelectedObject = Processor.GetObject(EngineGlobals.GetClass(this.TargetField.BindingClass), id);
                    });
                    this.Input.Text = await this.SelectedObject.GetFieldValue(this.TargetField.BindingField);
                }
            }
            catch { }
        }

        private void OpenSelectionDialogClick(object sender, RoutedEventArgs e)
        {
            if (this.TargetField?.BindingClass == Guid.Empty || this.TargetField?.BindingField == Guid.Empty)
            {
                DialogsManager.ShowExclamationDialog("Не определена привязка к классу объектов!", "Действие прервано");
                return;
            }
            DatabaseSelection s = new(this.TargetField);
            s.ShowDialog();
            if (s.Result == DialogStatus.Yes)
            {
                try
                {
                    this.SelectedObject = s.GetSelectedObject();
                    //this.Input.Text = await this.SelectedObject.GetFieldValue(this.TargetField.BindingField);
                }
                catch (Exception ex)
                {
                    DialogsManager.ShowErrorDialog(ex.Message);
                }
            }
        }
    }
}
