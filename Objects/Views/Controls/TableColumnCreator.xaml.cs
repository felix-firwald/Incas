using Incas.Core.Classes;
using Incas.Core.Interfaces;
using Incas.Objects.AutoUI;
using Incas.Objects.Components;
using Incas.Objects.ViewModels;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incas.Objects.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для TableColumnCreator.xaml
    /// </summary>
    public partial class TableColumnCreator : UserControl, ICollapsible
    {
        public delegate bool FieldAction(TableColumnCreator t);
        public delegate int FieldMoving(TableColumnCreator t);
        public event FieldAction OnRemove;
        public event FieldMoving OnMoveDownRequested;
        public event FieldMoving OnMoveUpRequested;
        public TableColumnViewModel vm { get; set; }
        public TableColumnCreator(TableFieldColumnData col)
        {
            this.InitializeComponent();
            this.vm = new(col);
            this.DataContext = this.vm;
            this.ExpanderButton.IsChecked = true;
        }
        public TableFieldColumnData GetField()
        {
            switch (this.vm.FieldData.FieldType)
            {
                case FieldType.LocalEnumeration:
                    try
                    {
                        List<string> values = JsonConvert.DeserializeObject<List<string>>(this.vm.FieldData.Value);
                    }
                    catch
                    {
                        throw new FieldDataFailed($"Колонка [{this.vm.FieldData.Name}] (\"{this.vm.FieldData.VisibleName}\") не настроена.");
                    }
                    break;
                case FieldType.GlobalEnumeration:
                    try
                    {
                        Guid id = Guid.Parse(this.vm.FieldData.Value);
                    }
                    catch
                    {
                        throw new FieldDataFailed($"Колонка [{this.vm.FieldData.Name}] (\"{this.vm.FieldData.VisibleName}\") не настроена.");
                    }
                    break;
            }
            return this.vm.FieldData;
        }

        private void RemoveClick(object sender, MouseButtonEventArgs e)
        {
            this.OnRemove?.Invoke(this);
        }

        private void UpClick(object sender, MouseButtonEventArgs e)
        {
            this.OnMoveDownRequested?.Invoke(this);
        }

        private void DownClick(object sender, MouseButtonEventArgs e)
        {
            this.OnMoveUpRequested?.Invoke(this);
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
        }

        private void MinimizeClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.MainBorder.Height = 40;
        }

        private void OpenSettingsClick(object sender, RoutedEventArgs e)
        {
            TableFieldColumnData f = this.vm.FieldData;
            string name = $"Настройки колонки [{f.Name}]";
            switch (f.FieldType)
            {
                case FieldType.String:
                case FieldType.Text:
                    TextColumnSettings tc = new(f);
                    tc.ShowDialog(name, Icon.Sliders, DialogSimpleForm.Components.IconColor.Green);
                    break;
                case FieldType.LocalEnumeration:
                    LocalEnumerationColumnSettings le = new(f);
                    le.ShowDialog(name, Icon.Sliders, DialogSimpleForm.Components.IconColor.Yellow);
                    break;
                case FieldType.GlobalEnumeration:
                    GlobalEnumerationColumnSettings ge = new(f);
                    ge.ShowDialog(name, Icon.Sliders, DialogSimpleForm.Components.IconColor.Yellow);
                    break;
            }
        }
    }
}
