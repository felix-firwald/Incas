using Incas.Admin.AutoUI;
using Incas.Admin.ViewModels;
using Incas.Core.Classes;
using Incas.Core.Interfaces;
using Incas.Objects.AutoUI;
using Incas.Objects.Views.Windows;
using IncasEngine.Models;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using static Incas.Core.Interfaces.ITabItem;

namespace Incas.Admin.Views.Pages
{
    public partial class WorkspaceManager : System.Windows.Controls.UserControl, ITabItem
    {
        private WorkspaceParametersViewModel vm;
        public event TabAction OnClose;
        public string Id { get; set; }
        public WorkspaceManager()
        {
            this.InitializeComponent();
            this.vm = new WorkspaceParametersViewModel();
            this.DataContext = this.vm;
            this.FillConstants();
            //this.FillEnumerations();
        }

        private void FillConstants()
        {
            this.vm.UpdateConstants();
        }

        private void AddConstantClick(object sender, RoutedEventArgs e)
        {
            ParameterConstant c = new();
            if (c.ShowDialog("Назначение константы") == true)
            {
                using (Parameter p = new())
                {
                    p.Name = c.Name;
                    p.Value = c.Value;
                    p.Type = ParameterType.CONSTANT;
                    p.CreateParameter();
                }
                this.FillConstants();
            }
        }

        private void EditConstantClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.vm.SelectedConstant.Id == Guid.Empty)
                {
                    DialogsManager.ShowExclamationDialog("Константа не выбрана", "Действие невозможно");
                    return;
                }
                ParameterConstant c = new(true);
                using (Parameter p = new())
                {
                    Parameter par = p.GetParameter(this.vm.SelectedConstant.Id);
                    c.Name = par.Name;
                    c.Value = par.Value;
                }
                c.ShowDialog("Редактирование константы");
                using (Parameter p = new())
                {
                    p.Name = c.Name;
                    p.Value = c.Value;
                    p.UpdateParameter(this.vm.SelectedConstant.Id);
                }
                this.FillConstants();
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex.Message);
            }
        }

        private void RemoveConstantClick(object sender, RoutedEventArgs e)
        {
            if (this.vm.SelectedConstant.Id == Guid.Empty)
            {
                DialogsManager.ShowExclamationDialog("Константа не выбрана", "Действие невозможно");
                return;
            }
            try
            {              
                if (DialogsManager.ShowQuestionDialog($"Вы действительно хотите удалить константу [{this.vm.SelectedConstant.Name}] из этого рабочего пространства?", "Удалить константу?", "Удалить", "Не удалять") == Core.Views.Windows.DialogStatus.No)
                {
                    return;
                }
                using (Parameter p = new())
                {
                    p.RemoveParameterById(this.vm.SelectedConstant.Id);
                }
                this.FillConstants();
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex.Message);
            }
        }

        private void UpdateConstantsClick(object sender, RoutedEventArgs e)
        {
            this.FillConstants();
        }

        private void AddEnumerationClick(object sender, RoutedEventArgs e)
        {
            ParameterEnum en = new()
            {
                Value = [""]
            };
            if (en.ShowDialog("Назначение перечисления", Icon.NumericList) == true)
            {
                using (Parameter p = new())
                {
                    p.Name = en.Name;
                    p.SetValue(en.Value);
                    p.Type = ParameterType.ENUMERATION;
                    p.CreateParameter();
                }
                this.vm.UpdateEnumerations();
            }
        }

        private void RemoveEnumerationClick(object sender, RoutedEventArgs e)
        {
            if (this.vm.SelectedEnumeration.Id == Guid.Empty)
            {
                return;
            }
            try
            {
                if (DialogsManager.ShowQuestionDialog($"Вы действительно хотите удалить перечисление [{this.vm.SelectedEnumeration.Name}] из этого рабочего пространства?", "Удалить константу?", "Удалить", "Не удалять") == Core.Views.Windows.DialogStatus.No)
                {
                    return;
                }
                using (Parameter p = new())
                {
                    p.RemoveParameterById(this.vm.SelectedEnumeration.Id);
                }
                this.vm.UpdateEnumerations();
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex.Message);
            }
        }
        private void UpdateEnumerationsClick(object sender, RoutedEventArgs e)
        {
            this.vm.UpdateEnumerations();
        }

        private void EditEnumerationClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.vm.SelectedEnumeration.Id == Guid.Empty)
                {
                    return;
                }

                ParameterEnum en = new(true);
                using (Parameter p = new())
                {
                    Parameter par = p.GetParameter(this.vm.SelectedEnumeration.Id);
                    en.Name = par.Name;
                    en.Value = JsonConvert.DeserializeObject<List<string>>(par.Value);
                }
                if (en.ShowDialog("Редактирование перечисления", Icon.NumericList))
                {
                    using (Parameter p = new())
                    {
                        p.Name = en.Name;
                        p.SetValue(en.Value);
                        p.UpdateParameter(this.vm.SelectedEnumeration.Id);
                    }
                    this.vm.UpdateEnumerations();
                }
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex.Message);
            }
        }

        private void OpenSettingsClick(object sender, RoutedEventArgs e)
        {
            WorkspaceSettings ws = new();
            ws.ShowDialog("Редактирование настроек", Icon.GearWide);          
        }

        private void AddClassClick(object sender, RoutedEventArgs e)
        {
            ClassTypeSettings ct = new();
            if (ct.ShowDialog("Первичная настройка", Icon.Lightning))
            {
                CreateClass cc = new(ct);
                cc.ShowDialog();
                //DialogsManager.ShowPageWithGroupBox(cc, "CLASS_EDITOR", "Создание класса", TabType.Usual);
                this.vm.UpdateClasses();
            }
        }
        private Guid GetSelectedClass()
        {
            return this.vm.SelectedClass.Id;
        }

        private void EditClassClick(object sender, RoutedEventArgs e)
        {
            CreateClass cc = new(this.GetSelectedClass());
            cc.ShowDialog();
            this.vm.UpdateClasses();
        }

        private void RemoveClassClick(object sender, RoutedEventArgs e)
        {
            if (DialogsManager.ShowQuestionDialog(
                "Класс будет безвозвратно удален, вместе с объектами, которые к нему относятся. Вы уверены?",
                "Удалить класс?",
                "Удалить",
                "Не удалять") == Core.Views.Windows.DialogStatus.Yes)
            {
                using (Class cl = new())
                {
                    List<string> list = cl.FindBackReferencesNames(this.GetSelectedClass());
                    if (list.Count > 0)
                    {
                        DialogsManager.ShowExclamationDialog("Класс невозможно удалить, поскольку на него ссылаются следующие классы:\n" + string.Join(",\n", list), "Удаление невозможно");
                    }
                    else
                    {
                        cl.Remove(this.GetSelectedClass());
                    }
                }
                this.vm.UpdateClasses();
            }
        }

        private void FixClassClick(object sender, RoutedEventArgs e)
        {
            Guid id = this.GetSelectedClass();
            if (id == Guid.Empty)
            {
                return;
            }
            using (Class cl = new(id))
            {
                Processor.UpdateObjectMap(cl);
            }           
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox list = sender as ListBox;
            list.SelectedItem = null;
        }
    }
}
