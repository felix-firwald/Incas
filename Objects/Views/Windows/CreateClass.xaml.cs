using Incas.Core.Classes;
using Incas.Objects.Models;
using Incas.Objects.ViewModels;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Collections.Generic;
using System;
using Incas.Objects.AutoUI;
using Incas.Objects.Components;
using Incas.Objects.Exceptions;
using Incas.Objects.Views.Controls;

namespace Incas.Objects.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateClass.xaml
    /// </summary>
    public partial class CreateClass : Window
    {
        ClassViewModel vm;
        public CreateClass(ClassTypeSettings primary)
        {
            this.InitializeComponent();
            this.vm = new(new());
            this.vm.CategoryOfClass = primary.Category;
            this.vm.NameOfClass = primary.Name;
            this.vm.Type = (ClassType)primary.Selector.SelectedObject;
            if (this.vm.Type == ClassType.Document)
            {
                this.vm.ShowCard = true;
                this.TemplatesArea.Visibility = Visibility.Visible;
            }
            this.DataContext = this.vm;            
        }
        public CreateClass(Guid id)
        {
            this.InitializeComponent();
            this.Title = "Редактирование класса";
            Class cl = new(id);
            this.vm = new(cl);
            this.DataContext = this.vm;
            foreach (Models.Field f in cl.GetClassData().fields)
            {
                this.AddField(f);
            }
            this.UpdateStatusesList();
        }

        private void GetMoreInfoClick(object sender, MouseButtonEventArgs e)
        {
            ProgramState.OpenWebPage("https://teletype.in/@incas/classes");
        }

        private void AddField(Incas.Objects.Models.Field data = null)
        {
            Incas.Objects.Views.Controls.FieldCreator fc = new(data);
            fc.OnRemove += this.Fc_OnRemove;
            this.ContentPanel.Children.Add(fc);
        }

        private void Fc_OnRemove(Controls.FieldCreator t)
        {
            this.ContentPanel.Children.Remove(t);
        }

        private void AddFieldClick(object sender, MouseButtonEventArgs e)
        {
            this.AddField();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            List<Incas.Objects.Models.Field> fields = new();
            try
            {
                foreach (Incas.Objects.Views.Controls.FieldCreator item in this.ContentPanel.Children)
                {
                    Incas.Objects.Models.Field f = item.GetField();
                    f.SetId();
                    fields.Add(f);
                }
                this.vm.SetData(fields);
                this.vm.Source.Save();
                this.Close();
            }
            catch (FieldDataFailed fd)
            {
                DialogsManager.ShowExclamationDialog(fd.Message, "Сохранение прервано");
            }
        }
        #region Templates
        private void AddTemplateClick(object sender, MouseButtonEventArgs e)
        {

        }
        private void UpdateTemplatesList()
        {
            this.TemplatesPanel.Children.Clear();
            if (this.vm.SourceData.Templates is null)
            {
                return;
            }
            foreach (KeyValuePair<string, string> template in this.vm.SourceData.Templates)
            {
                TemplateClassElement tce = new(template.Key, template.Value);
                tce.OnEdit += this.Tce_OnEdit;
                tce.OnRemove += this.Tce_OnRemove;
            }
        }

        private void Tce_OnRemove(string name, string path)
        {
            
        }

        private void Tce_OnEdit(string name, string path)
        {
            
        }
        #endregion

        #region Statuses
        private void AddStatusClick(object sender, MouseButtonEventArgs e)
        {
            StatusSettings ss = new();
            if (ss.ShowDialog("Настройка статуса", Core.Classes.Icon.Tag) == true)
            {
                this.vm.SourceData.AddStatus(ss.GetData());
                this.UpdateStatusesList();
            }
        }
        private void UpdateStatusesList()
        {
            this.StatusesPanel.Children.Clear();
            if (this.vm.SourceData.Statuses is null)
            {
                return;
            }
            int index = 0;
            foreach (StatusData data in this.vm.SourceData.Statuses.Values)
            {
                index++;
                StatusElement se = new(index, data);
                se.OnEdit += this.Se_OnEdit;
                se.OnRemove += this.Se_OnRemove;
                this.StatusesPanel.Children.Add(se);
            }
        }

        private void Se_OnRemove(int index, StatusData statusData)
        {
            this.vm.SourceData.RemoveStatus(statusData);
        }

        private void Se_OnEdit(int index, StatusData statusData)
        {
            this.vm.SourceData.Statuses[index] = statusData;
            this.UpdateStatusesList();
        }

        #endregion
    }
}
