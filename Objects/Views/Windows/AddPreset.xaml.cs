using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using Incas.DialogSimpleForm.Views.Controls;
using Incas.Objects.Components;
using Incas.Objects.Models;
using Incas.Objects.Views.Controls;
using IronPython.Compiler.Ast;
using System;
using System.Collections.Generic;
using System.Windows;
using Field = Incas.Objects.Models.Field;

namespace Incas.Objects.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddPreset.xaml
    /// </summary>
    public partial class AddPreset : Window
    {
        public Class TargetClass { get; set; }
        public ClassData ClassData { get; set; }
        public AddPreset(Class cl)
        {
            this.InitializeComponent();
            this.TargetClass = cl;
            this.ClassData = cl.GetClassData();
            this.InitializeCheckBox();
        }
        private void InitializeCheckBox()
        {
            List<CheckedItem> list = new();
            foreach (Field f in this.ClassData.GetPresettingFields())
            {
                CheckedItem item = new()
                {
                    Name = f.Name,
                    Target = f
                };
                list.Add(item);
            }
            CheckedList cl = new(list, false);
            CheckedListBox clb = new(cl);
            clb.OnCheckedStateChanged += this.Clb_OnCheckedStateChanged;
            this.AvailableFieldsPanel.Child = clb;
        }

        private void Clb_OnCheckedStateChanged(string content, object id, bool isChecked)
        {
            if (isChecked)
            {
                this.DrawControl((Field)id);
            }
            else
            {
                this.RemoveControl((Field)id);
            }
        }
        private void RemoveControl(Field f)
        {
            foreach (FieldFiller ff in this.ActiveFieldsPanel.Children)
            {
                if (ff.Field == f)
                {
                    this.ActiveFieldsPanel.Children.Remove(ff);
                    return;
                }
            }
        }
        private void DrawControl(Field f)
        {
            FieldFiller ff = new(f);
            this.ActiveFieldsPanel.Children.Add(ff);
        }

        private async void SaveClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.PresetName.Text))
            {
                DialogsManager.ShowExclamationDialog("Имя пресета не может быть пустым", "Сохранение прервано");
                return;
            }
            Dictionary<Guid, string> values = new();
            foreach (FieldFiller ff in this.ActiveFieldsPanel.Children)
            {
                values.Add(ff.Field.Id, ff.GetData());
            }
            Preset preset = new()
            {
                Name = this.PresetName.Text,
                Data = values
            };
            await ObjectProcessor.WritePreset(this.TargetClass, preset);
            this.Close();
        }
    }
}
