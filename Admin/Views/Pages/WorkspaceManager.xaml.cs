using Incas.Admin.ViewModels;
using Incas.Admin.Views.Controls;
using Incas.Core.Classes;
using Incas.Core.Models;
using Incubator_2.Windows.AdminWindows;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Data;
using DocumentFormat.OpenXml.Office2010.Excel;
using Newtonsoft.Json;
using System.Collections.Generic;
using Incas.Admin.AutoUI;
using Incas.Objects.Views.Windows;

namespace Incas.Admin.Views.Pages
{
    public partial class WorkspaceManager : UserControl
    {
        private WorkspaceParametersViewModel vm;
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
            using (Parameter p = new())
            {
                this.ConstantsTable.ItemsSource = p.GetConstants().DefaultView;
            }
        }

        private void AddConstantClick(object sender, RoutedEventArgs e)
        {
            ParameterConstant c = new();
            if (DialogsManager.ShowSimpleFormDialog(c, "Назначение константы") == true)
            {
                using (Parameter p = new())
                {
                    p.name = c.Name;
                    p.value = c.Value;
                    p.type = ParameterType.CONSTANT;
                    p.CreateParameter();
                }
                this.FillConstants();
            }
        }

        private void EditConstantClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.ConstantsTable.SelectedItems.Count == 0)
                {
                    return;
                }
                Guid id = Guid.Parse(((DataRowView)this.ConstantsTable.SelectedItems[0]).Row["Идентификатор"].ToString());
                ParameterConstant c = new();
                using (Parameter p = new())
                {
                    Parameter par = p.GetParameter(id);
                    c.Name = par.name;
                    c.Value = par.value;
                }
                DialogsManager.ShowSimpleFormDialog(c, "Редактирование константы");
                using (Parameter p = new())
                {
                    p.name = c.Name;
                    p.value = c.Value;
                    p.UpdateParameter(id);
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
            if (this.ConstantsTable.SelectedItems.Count == 0)
            {
                return;
            }
            try
            {
                object name = ((DataRowView)this.ConstantsTable.SelectedItems[0]).Row["Наименование константы"];
                if (DialogsManager.ShowQuestionDialog($"Вы действительно хотите удалить константу [{name}] из этого рабочего пространства?", "Удалить константу?", "Удалить", "Не удалять") == Core.Views.Windows.DialogStatus.No)
                {
                    return;
                }
                Guid id = Guid.Parse(((DataRowView)this.ConstantsTable.SelectedItems[0]).Row["Идентификатор"].ToString());
                using (Parameter p = new())
                {
                    p.RemoveParameterById(id);
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
            ParameterEnum en = new();
            en.Value = new();
            en.Value.Add("");
            if (DialogsManager.ShowSimpleFormDialog(en, "Назначение перечисления", Icon.NumericList) == true)
            {
                using (Parameter p = new())
                {
                    p.name = en.Name;
                    p.SetValue(en.Value);
                    p.type = ParameterType.ENUMERATION;
                    p.CreateParameter();
                }
                this.vm.UpdateEnumerations();
            }
        }

        private void RemoveEnumerationClick(object sender, RoutedEventArgs e)
        {
            if (this.EnumsTable.SelectedItems.Count == 0)
            {
                return;
            }
            try
            {
                object name = ((DataRowView)this.EnumsTable.SelectedItems[0]).Row["Наименование перечисления"];
                if (DialogsManager.ShowQuestionDialog($"Вы действительно хотите удалить перечисление [{name}] из этого рабочего пространства?", "Удалить константу?", "Удалить", "Не удалять") == Core.Views.Windows.DialogStatus.No)
                {
                    return;
                }
                Guid id = Guid.Parse(((DataRowView)this.EnumsTable.SelectedItems[0]).Row["Идентификатор"].ToString());
                using (Parameter p = new())
                {
                    p.RemoveParameterById(id);
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
                if (this.EnumsTable.SelectedItems.Count == 0)
                {
                    return;
                }
                Guid id = Guid.Parse(((DataRowView)this.EnumsTable.SelectedItems[0]).Row["Идентификатор"].ToString());
                ParameterEnum en = new();
                using (Parameter p = new())
                {
                    Parameter par = p.GetParameter(id);
                    en.Name = par.name;
                    en.Value = JsonConvert.DeserializeObject<List<string>>(par.value);
                }
                if (DialogsManager.ShowSimpleFormDialog(en, "Редактирование перечисления", Icon.NumericList))
                {
                    using (Parameter p = new())
                    {
                        p.name = en.Name;
                        p.SetValue(en.Value);
                        p.UpdateParameter(id);
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
            DialogsManager.ShowSimpleFormDialog(ws, "Редактирование настроек", Icon.GearWide);
        }

        private void AddClassClick(object sender, RoutedEventArgs e)
        {
            CreateClass cc = new();
            cc.ShowDialog();
        }

        private void EditClassClick(object sender, RoutedEventArgs e)
        {
            if (this.ClassesTable.SelectedItems.Count == 0)
            {
                return;
            }
            Guid id = Guid.Parse(((DataRowView)this.ClassesTable.SelectedItems[0]).Row["Идентификатор"].ToString());
            CreateClass cc = new(id);
            cc.ShowDialog();
        }

        private void RemoveClassClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
