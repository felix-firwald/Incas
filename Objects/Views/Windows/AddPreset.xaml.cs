using DocumentFormat.OpenXml.Office2010.Excel;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using Incas.DialogSimpleForm.Views.Controls;
using Incas.Objects.Components;
using Incas.Objects.Engine;
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
        public IClass TargetClass { get; set; }
        public ClassData ClassData { get; set; }
        public Preset Preset { get; set; }
        public delegate void UpdateRequested();
        public event UpdateRequested OnUpdateRequested;
        public AddPreset(IClass cl)
        {
            this.InitializeComponent();
            this.TargetClass = cl;
            this.ClassData = cl.GetClassData();
            this.Preset = new();
            this.InitializeCheckBox(null);
        }
        public AddPreset(IClass cl, Preset preset)
        {
            this.InitializeComponent();
            this.TargetClass = cl;
            this.ClassData = cl.GetClassData();
            this.Preset = preset;
            this.PresetName.Text = this.Preset.Name;
            //this.ApplyCheckBox.Visibility = Visibility.Collapsed;
            this.InitializeCheckBox(this.Preset);
        }
        private void InitializeCheckBox(Preset preset)
        {
            Dictionary<CheckedItem, bool> dict = new();
            if (preset == null)
            {
                foreach (Field f in this.ClassData.GetPresettingFields())
                {
                    CheckedItem item = new()
                    {
                        Name = f.Name,
                        Target = f
                    };
                    dict.Add(item, false);
                }
            }
            else
            {
                foreach (Field f in this.ClassData.GetPresettingFields())
                {
                    CheckedItem item = new()
                    {
                        Name = f.Name,
                        Target = f
                    };
                    bool draw = preset.Data.ContainsKey(f.Id);
                    if (draw)
                    {
                        this.DrawControl(f).SetValue(preset.Data[f.Id]);
                    }                  
                    dict.Add(item, draw);
                }
            }
            CheckedList cl = new(dict);
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
        private FieldFiller DrawControl(Field f)
        {
            FieldFiller ff = new(f);
            this.ActiveFieldsPanel.Children.Add(ff);
            return ff;
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
            this.Preset.Name = this.PresetName.Text;
            this.Preset.Data = values;
            await Processor.WritePreset(this.TargetClass, this.Preset);
            this.Close();
            this.OnUpdateRequested?.Invoke();
        }
    }
}
