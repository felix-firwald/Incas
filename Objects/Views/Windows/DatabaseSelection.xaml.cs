﻿using Incas.Core.Classes;
using Incas.Core.Views.Controls;
using Incas.Core.Views.Windows;
using Incas.DialogSimpleForm.Components;
using Incas.Objects.Views.Controls;
using IncasEngine.Core;
using IncasEngine.Core.DatabaseQueries.RequestsUtils.Where;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Incas.Objects.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для DatabaseSelection.xaml
    /// </summary>
    public partial class DatabaseSelection : Window
    {
        public DialogStatus Result = DialogStatus.Undefined;
        public readonly BindingData Binding;
        public readonly Class Class;
        public readonly IClassData ClassData;
        private Guid SelectedId
        {
            get
            {
                try
                {
                    return Guid.Parse(((DataRowView)this.Grid.SelectedItems[0]).Row[Helpers.IdField].ToString());
                }
                catch
                {
                    return Guid.Empty;
                }
            }
        }
        public IObject GetSelectedObject()
        {
            return Processor.GetObject(this.Class, this.SelectedId);
        }

        public string SelectedValue => ((DataRowView)this.Grid.SelectedItems[0]).Row[Helpers.IdField].ToString();
        public DatabaseSelection(BindingData data)
        {
            this.InitializeComponent();
            this.Binding = data;
            this.Class = EngineGlobals.GetClass(this.Binding.BindingClass);
            if (!ProgramState.CurrentWorkspace.CurrentGroup.IsComponentInAccess(this.Class.Component))
            {
                this.ShowDisabledComponentMessage();
            }
            else
            {
                this.ClassData = this.Class.GetClassData();
                if (this.ClassData is null || this.ClassData.Fields is null)
                {
                    DialogsManager.ShowDatabaseErrorDialog("Не удалось идентифицировать класс и показать карту его объектов. Вероятно, это означает, что класс удален. Обратитесь к администратору рабочего пространства для устранения ошибки.", "Привязка сломана");
                    this.IsEnabled = false;
                    return;
                }
                this.Title = this.ClassData.ListName;
                this.SetFields();
                this.FillList();
                this.Class.OnUpdated += this.EngineEvents_OnUpdateClassRequested;
                this.Class.OnRemoved += this.Class_OnRemoved;
            }            
        }

        private void Class_OnRemoved()
        {
            this.Close();
        }
        private void SetOtherContent(UIElement element)
        {
            this.MainGrid.Children.Clear();
            this.MainGrid.Children.Add(element);
            System.Windows.Controls.Grid.SetRow(element, 0);
            System.Windows.Controls.Grid.SetRowSpan(element, 3);
            System.Windows.Controls.Grid.SetColumn(element, 0);
        }
        private void EngineEvents_OnUpdateClassRequested()
        {
            this.Dispatcher.Invoke(() =>
            {
                ClassUpdatedMessage message = new();
                this.SetOtherContent(message);
            });
        }
        private void ShowDisabledComponentMessage()
        {
            ComponentNotActive na = new();
            this.SetOtherContent(na);
        }
        private WhereInstruction GetBaseInstruction()
        {
            WhereInstruction instruction = new();
            if (this.Binding.Compliance is not null && this.Binding.Compliance.Count > 0)
            {
                foreach (KeyValuePair<Guid, ConstraintValue> pair in this.Binding.Compliance)
                {
                    switch (pair.Value.Type)
                    {
                        case ConstraintValue.ConstraintValueType.ByFixedValue:
                            instruction.AndWhereEqual(pair.Key.ToString(), pair.Value.Value);
                            break;
                        //case ConstraintValue.ConstraintValueType.ByField:
                        //    instruction.AndWhereEqual(pair.Key.ToString(), this.currentObject[pair.Value.TargetField].ToString());
                        //    break;
                    }
                }
            }
            return instruction;
        }
        private async void FillList()
        {
            DataTable dt;
            WhereInstruction instruction = this.GetBaseInstruction();
            dt = await Processor.GetObjectsList(this.Class, instruction);
            this.UpdateItemsSource(dt.Columns);
            DataView dv = dt.AsDataView();
            if (this.Class.Type == ClassType.Model)
            {
                dv.Sort = $"[{Helpers.NameField}] ASC";
            }    
            this.Grid.ItemsSource = dv;
        }
        private async void FillList(string field, string value)
        {
            WhereInstruction instruction = this.GetBaseInstruction();
            instruction.AndWhereEqual(field, value);
            DataTable dt = await Processor.GetObjectsList(this.Class, instruction);
            this.UpdateItemsSource(dt.Columns);
            this.Grid.ItemsSource = dt.DefaultView;
        }
        private void SetFields()
        {
            List<string> fields = [];
            foreach (Field field in this.ClassData.Fields)
            {
                fields.Add(field.VisibleName);
            }
            this.Fields.ItemsSource = fields;
        }
        private void UpdateItemsSource(DataColumnCollection cols)
        {
            List<string> result = [];
            foreach (DataColumn col in cols)
            {
                result.Add(col.ColumnName);
            }

            try
            {
                this.Fields.SelectedIndex = 0;
            }
            catch { }
        }
        private void Grid_AutoGeneratingColumn(object sender, System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            Style style = this.FindResource("ColumnHeaderSpecial") as Style;
            switch (e.Column.Header.ToString())
            {
                case Helpers.IdField:
                    e.Column.Visibility = Visibility.Hidden;
                    break;
                case Helpers.NameField:
                    e.Column.Header = "Наименование";
                    e.Column.HeaderStyle = style;
                    e.Column.MinWidth = 100;
                    e.Column.CanUserReorder = false;
                    break;
                case Helpers.DateCreatedField:
                    e.Column.Header = "Дата создания";
                    e.Column.HeaderStyle = style;
                    e.Column.MinWidth = 100;
                    e.Column.MaxWidth = 120;
                    e.Column.CanUserReorder = false;
                    break;
                case Helpers.StatusField:
                    e.Column.Visibility = Visibility.Hidden;
                    break;
                case Helpers.TargetClassField:
                case Helpers.TargetObjectField:
                    e.Column.Visibility = Visibility.Hidden;
                    break;
                default:
                    string colHeader = e.Column.Header.ToString();
                    foreach (Field f in this.ClassData.Fields)
                    {
                        if (f.Type is FieldType.Boolean && colHeader == f.VisibleName)
                        {
                            DataGridCheckBoxColumn cbc = new()
                            {
                                Header = f.VisibleName,
                                Binding = new System.Windows.Data.Binding(colHeader),
                                EditingElementStyle = ResourceStyleManager.FindStyle(ResourceStyleManager.CheckboxEditingGridStyle),
                                ElementStyle = ResourceStyleManager.FindStyle(ResourceStyleManager.CheckboxNotEditableGridStyle)
                            };
                            e.Column = cbc;
                            break;
                        }
                    }
                    break;
            }
        }
        private void SelectClick(object sender, RoutedEventArgs e)
        {
            this.Finish();
        }
        private void Finish()
        {
            if (this.Grid.SelectedItems.Count == 0)
            {
                DialogsManager.ShowExclamationDialog("Нельзя использовать пустое значение!", "Значение не выбрано");
                return;
            }
            this.Result = DialogStatus.Yes;
            this.Close();
        }

        private void SearchClick(object sender, RoutedEventArgs e)
        {
            this.FillList(this.Fields.SelectedValue?.ToString(), this.SearchText.Text);
        }

        private void ClearClick(object sender, RoutedEventArgs e)
        {
            this.SearchText.Text = "";
            this.FillList();
        }

        private void OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (this.Grid.SelectedItems == null)
                {
                    return;
                }
                this.SearchText.Text = ((DataRowView)this.Grid.SelectedItems[0]).Row[this.Fields.SelectedValue.ToString()].ToString();
            }
            catch { }
        }

        private void OnMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Finish();
        }
    }
}
