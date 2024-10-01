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
using Incas.Templates.Components;
using System.IO;

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
            if (this.vm.Type == ClassType.Document)
            {
                this.TemplatesArea.Visibility = Visibility.Visible;
            }
            this.UpdateStatusesList();
            this.UpdateTemplatesList();
        }

        private void GetMoreInfoClick(object sender, MouseButtonEventArgs e)
        {
            ProgramState.OpenWebPage("https://teletype.in/@incas/classes");
        }

        private void AddField(Incas.Objects.Models.Field data = null)
        {
            Incas.Objects.Views.Controls.FieldCreator fc = new(this.ContentPanel.Children.Count, data);
            fc.OnRemove += this.Fc_OnRemove;
            fc.OnMoveDownRequested += this.Fc_OnMoveDownRequested;
            fc.OnMoveUpRequested += this.Fc_OnMoveUpRequested;
            this.ContentPanel.Children.Add(fc);
        }

        private int Fc_OnMoveUpRequested(Controls.FieldCreator t)
        {
            int position = t.vm.OrderNumber;
            if (position < this.ContentPanel.Children.Count - 1)
            {
                position += 1;
            }
            this.ContentPanel.Children.Remove(t);
            this.ContentPanel.Children.Insert(position, t);
            return position;
        }

        private int Fc_OnMoveDownRequested(Controls.FieldCreator t)
        {
            int position = t.vm.OrderNumber;
            if (position > 0)
            {
                position -= 1;
            }
            this.ContentPanel.Children.Remove(t);
            this.ContentPanel.Children.Insert(position, t);
            return position;
        }

        private bool Fc_OnRemove(Controls.FieldCreator t)
        {
            this.ContentPanel.Children.Remove(t);
            return true;
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
            TemplateClassEditor ce = new();
            ce.ShowDialog();
            if (ce.Result)
            {
                TemplateData td = new()
                {
                    Name = ce.SelectedName,
                    File = ce.SelectedPath
                };
                this.vm.SourceData.AddTemplate(td);
            }
            this.UpdateTemplatesList();
        }
        private void UpdateTemplatesList()
        {
            this.TemplatesPanel.Children.Clear();
            if (this.vm.SourceData.Templates is null)
            {
                return;
            }
            foreach (KeyValuePair<int, TemplateData> template in this.vm.SourceData.Templates)
            {
                TemplateClassElement tce = new(template.Key, template.Value);
                tce.OnEdit += this.Tce_OnEdit;
                tce.OnRemove += this.Tce_OnRemove;
                tce.OnSearchInFileRequested += this.Tce_OnSearchInFileRequested;
                this.TemplatesPanel.Children.Add(tce);
            }
        }

        private void Tce_OnSearchInFileRequested(string path)
        {
            bool CheckNameUniqueness(string name)
            {
                foreach (Objects.Views.Controls.FieldCreator creator in this.ContentPanel.Children)
                {
                    if (creator.vm.Source.Name == name)
                    {
                        return false;
                    }
                }
                return true;
            }
            try
            {
                WordTemplator wt = new(path);
                foreach (string tagname in wt.FindAllTags())
                {
                    if (!CheckNameUniqueness(tagname))
                    {
                        continue;
                    }
                    Objects.Models.Field tag = new()
                    {
                        Name = tagname,
                        VisibleName = tagname.Replace("_", " ")
                    };
                    this.AddField(tag);
                }
            }
            catch (IOException)
            {
                DialogsManager.ShowErrorDialog("Файл занят другим процесом. Его использование невозможно.");
            }
        }

        private void Tce_OnRemove(int index, TemplateData data)
        {
            this.vm.SourceData.Templates.Remove(index);
            this.UpdateTemplatesList();
        }

        private void Tce_OnEdit(int index, TemplateData data)
        {
            this.vm.SourceData.EditTemplate(index, data);
            this.UpdateTemplatesList();
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

        private void MinimizeAllClick(object sender, MouseButtonEventArgs e)
        {
            foreach (Incas.Objects.Views.Controls.FieldCreator item in this.ContentPanel.Children)
            {
                item.Minimize();
            }
        }

        private void MaximizeAllClick(object sender, MouseButtonEventArgs e)
        {
            foreach (Incas.Objects.Views.Controls.FieldCreator item in this.ContentPanel.Children)
            {
                item.Maximize();
            }
        }

        private void ShowFormClick(object sender, MouseButtonEventArgs e)
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
                this.vm.SourceData.fields = fields;
                ObjectsEditor oe = new(this.vm.Source, this.vm.SourceData);
                oe.ShowDialog();
            }
            catch (FieldDataFailed fd)
            {
                DialogsManager.ShowExclamationDialog(fd.Message, "Предпросмотр прерван");
            }          
        }
    }
}
