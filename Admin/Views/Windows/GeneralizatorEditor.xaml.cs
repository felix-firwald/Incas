using Incas.Admin.ViewModels;
using Incas.Core.Classes;
using Incas.Objects.Interfaces;
using Incas.Objects.ViewModels;
using IncasEngine.Models;
using IncasEngine.ObjectiveEngine.Exceptions;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Incas.Admin.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для GeneralizatorEditor.xaml
    /// </summary>
    public partial class GeneralizatorEditor : Window
    {
        private const string part_settings_tag = "PART_SETTINGS";
        private GeneralizatorViewModel vm { get; set; }
        public GeneralizatorEditor() // if new
        {
            this.InitializeComponent();
            this.Title = "Создание обобщения";
            this.vm = new();
            this.vm.OnAdditionalSettingsOpenRequested += this.Vm_OnAdditionalSettingsOpenRequested;
            this.DataContext = this.vm;
        }
        public GeneralizatorEditor(GeneralizatorItem item) // if edit
        {
            this.InitializeComponent();
            this.Title = "Редактирование обобщения";
            this.vm = new(new(item));
            this.vm.OnAdditionalSettingsOpenRequested += this.Vm_OnAdditionalSettingsOpenRequested;
            this.DataContext = this.vm;
        }

        private void Vm_OnAdditionalSettingsOpenRequested(IClassDetailsSettings settings)
        {
            foreach (TabItem tabitem in this.TabControlMain.Items)
            {
                if (tabitem.Header.ToString() == settings.ItemName)
                {
                    tabitem.IsSelected = true;
                    return;
                }
            }
            TabItem item = new()
            {
                Content = settings,
                Header = settings.ItemName,
                Tag = part_settings_tag,
                Style = this.FindResource("TabItemRemovable") as Style,
                BorderBrush = this.FindResource("LightPurple") as Brush
            };
            item.IsSelected = true;
            item.IsEnabledChanged += this.Item_IsEnabledChanged;
            this.TabControlMain.Items.Add(item);
        }
        private void Item_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.TabControlMain.Items.Remove(sender);
        }

        private void MinimizeAllClick(object sender, RoutedEventArgs e)
        {
            foreach (FieldViewModel f in this.vm.Fields)
            {
                f.IsExpanded = false;
            }
        }

        private void MaximizeAllClick(object sender, RoutedEventArgs e)
        {
            foreach (FieldViewModel f in this.vm.Fields)
            {
                f.IsExpanded = true;
            }
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogsManager.ShowWaitCursor();
                this.vm.Validate();
                this.vm.Save();               
                DialogsManager.ShowWaitCursor(false);
                this.Close();                
            }
            catch (FieldDataFailed ex)
            {
                DialogsManager.ShowExclamationDialog(ex.Message, "Сохранение прервано");
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
            }
        }

        private void AddFieldClick(object sender, RoutedEventArgs e)
        {
            Field f = new();
            f.SetId();
            f.ListVisibility = true;
            this.vm.AddField(f);
        }

        private void AddMethodClick(object sender, RoutedEventArgs e)
        {
            Method m = new();
            m.SetId();
            m.Icon = "M340-302.23v-355.54q0-15.84 10.87-26 10.87-10.15 25.37-10.15 4.53 0 9.5 1.31 4.97 1.3 9.49 3.92l279.84 178.15q8.24 5.62 12.35 13.46 4.12 7.85 4.12 17.08 0 9.23-4.12 17.08-4.11 7.84-12.35 13.46L395.23-271.31q-4.53 2.62-9.52 3.92-4.98 1.31-9.51 1.31-14.51 0-25.35-10.15-10.85-10.16-10.85-26Z";
            m.Color = new() { R = 255, G = 255, B = 255 };
            this.vm.AddMethod(m);
        }
        private void AddStaticMethodClick(object sender, RoutedEventArgs e)
        {
            Method m = new();
            m.SetId();
            m.Icon = "M340-302.23v-355.54q0-15.84 10.87-26 10.87-10.15 25.37-10.15 4.53 0 9.5 1.31 4.97 1.3 9.49 3.92l279.84 178.15q8.24 5.62 12.35 13.46 4.12 7.85 4.12 17.08 0 9.23-4.12 17.08-4.11 7.84-12.35 13.46L395.23-271.31q-4.53 2.62-9.52 3.92-4.98 1.31-9.51 1.31-14.51 0-25.35-10.15-10.85-10.16-10.85-26Z";
            m.Color = new() { R = 255, G = 255, B = 255 };
            this.vm.AddStaticMethod(m);
        }

        private void AddTableClick(object sender, RoutedEventArgs e)
        {
            Table t = new();
            t.SetId();
            this.vm.AddTable(t);
        }
    }
}
