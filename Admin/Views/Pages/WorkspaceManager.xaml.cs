using Incas.Admin.ViewModels;
using Incas.Admin.Views.Controls;
using Incas.Core.Classes;
using Incas.Core.Models;
using Incubator_2.Windows.AdminWindows;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Data;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace Incas.Admin.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для WorkspaceManager.xaml
    /// </summary>
    public class ParameterConstant
    {
        [Description("Наименование константы")]
        public string Name;

        [Description("Значение константы")]
        public string Value;
    }
    public partial class WorkspaceManager : UserControl
    {
        private WorkspaceParametersViewModel vm;
        public WorkspaceManager()
        {
            this.InitializeComponent();
            this.vm = new WorkspaceParametersViewModel();
            this.DataContext = this.vm;
            this.FillConstants();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            this.vm.SaveParameters();
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
                long id = (long)((DataRowView)this.ConstantsTable.SelectedItems[0]).Row["Идентификатор"];
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

        }

        private void UpdateConstantsClick(object sender, RoutedEventArgs e)
        {
            this.FillConstants();
        }
    }
}
