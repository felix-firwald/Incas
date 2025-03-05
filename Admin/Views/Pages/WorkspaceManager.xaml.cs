using Incas.Admin.AutoUI;
using Incas.Admin.ViewModels;
using Incas.Admin.Views.Windows;
using Incas.Core.Classes;
using Incas.Core.Interfaces;
using Incas.Objects.AutoUI;
using Incas.Objects.Views.Windows;
using IncasEngine.Core;
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
            ct.Component.SetSelection(this.vm.SelectedCategory);
            if (ct.ShowDialog("Первичная настройка", Icon.Lightning))
            {
                CreateClass cc = new(ct);
                cc.ShowDialog();
                this.vm.UpdateClasses();
            }
        }
        private Guid GetSelectedClass()
        {
            return this.vm.SelectedClass.Id;
        }

        private void EditClassClick(object sender, RoutedEventArgs e)
        {
            Guid id = this.GetSelectedClass();
            if (id == Guid.Empty)
            {
                DialogsManager.ShowExclamationDialog("Не выбран класс для редактирования.", "Действие невозможно");
                return;
            }
            CreateClass cc = new(id);
            cc.ShowDialog();
            this.vm.UpdateClasses();
        }

        private void RemoveClassClick(object sender, RoutedEventArgs e)
        {
            Guid id = this.GetSelectedClass();
            if (id == Guid.Empty)
            {
                return;
            }
            if (DialogsManager.ShowQuestionDialog(
                "Класс будет безвозвратно удален, вместе с объектами, которые к нему относятся. Вы уверены?",
                "Удалить класс?",
                "Удалить",
                "Не удалять") == Core.Views.Windows.DialogStatus.Yes)
            {              
                using (Class cl = new())
                {
                    List<string> list = cl.FindBackReferencesNames(id);
                    if (list.Count > 0)
                    {
                        DialogsManager.ShowExclamationDialog("Класс невозможно удалить, поскольку на него ссылаются следующие классы:\n" + string.Join(",\n", list), "Удаление невозможно");
                    }
                    else
                    {
                        Class targetClass = EngineGlobals.GetClass(id);
                        targetClass.Remove();
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
            using (Class cl = EngineGlobals.GetClass(id))
            {
                Processor.UpdateObjectMap(cl);
            }           
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox list = sender as ListBox;
            list.SelectedItem = null;
        }

        private async void FixUsersMapClick(object sender, RoutedEventArgs e)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                Processor.UpdateObjectMap(ProgramState.CurrentWorkspace.CurrentUser.Class);
            });
            
        }

        private async void FixGroupsMapClick(object sender, RoutedEventArgs e)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                Processor.UpdateObjectMap(ProgramState.CurrentWorkspace.CurrentGroup.Class);
            });           
        }

        private void InheritClassClick(object sender, RoutedEventArgs e)
        {
            Guid id = this.GetSelectedClass();
            if (id == Guid.Empty)
            {
                return;
            }
            Class parent = EngineGlobals.GetClass(id);
            ClassTypeSettings ct = new(parent);
            ct.Component.SetSelection(this.vm.SelectedCategory);
            if (ct.ShowDialog("Первичная настройка наследника", Icon.Lightning))
            {
                CreateClass cc = new(ct);
                
                cc.ShowDialog();
                this.vm.UpdateClasses();
            }
        }

        private void OpenComponentsSettings(object sender, RoutedEventArgs e)
        {
            ComponentsSettings settings = new();
            settings.ShowDialog();
        }

        private void OpenCommandsSettings(object sender, RoutedEventArgs e)
        {
            CommandSettings settings = new();
            settings.ShowDialog();
        }

        private void EditGroupsClick(object sender, RoutedEventArgs e)
        {
#if !E_FREE
            CreateClass cc = new(ProgramState.CurrentWorkspace.GetDefinition().ServiceGroups);
            cc.ShowDialog();
#else
            DialogsManager.ShowLimitedEditionMessage();
#endif
        }

        private void EditUsersClick(object sender, RoutedEventArgs e)
        {
#if !E_FREE
            CreateClass cc = new(ProgramState.CurrentWorkspace.GetDefinition().ServiceUsers);
            cc.ShowDialog();
#else
            DialogsManager.ShowLimitedEditionMessage();
#endif
        }
        private void AddGeneralizator(object sender, RoutedEventArgs e)
        {
            GeneralizatorEditor editor = new();
            editor.ShowDialog();
        }

        private void EditGeneralizator(object sender, RoutedEventArgs e)
        {
            GeneralizatorEditor editor = new(this.vm.SelectedGeneralizator);
            editor.ShowDialog();
        }

        private void RemoveGeneralizator(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateGeneralizators(object sender, RoutedEventArgs e)
        {

        }

        private void AddStructure(object sender, RoutedEventArgs e)
        {
#if E_BUSINESS
            StructureEditor editor = new();
            editor.ShowDialog();
#else
            DialogsManager.ShowLimitedEditionMessage();
#endif
        }

        private void EditStructure(object sender, RoutedEventArgs e)
        {
#if E_BUSINESS
            StructureEditor editor = new(this.vm.SelectedStructure);
            editor.ShowDialog();
#else
            DialogsManager.ShowLimitedEditionMessage();
#endif
        }

        private void RemoveStructure(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateStructuresList(object sender, RoutedEventArgs e)
        {

        }

        private void AddConverter(object sender, RoutedEventArgs e)
        {
            ConverterEditor editor = new();
            editor.ShowDialog();
        }

        private void EditConverter(object sender, RoutedEventArgs e)
        {
            ConverterEditor editor = new(this.vm.SelectedConverter);
            editor.ShowDialog();
        }

        private void RemoveConverter(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateConvertersList(object sender, RoutedEventArgs e)
        {

        }
    }
}
