using Common;
using Incubator_2.Common;
using Incubator_2.Models;
using Incubator_2.ViewModels;
using Microsoft.Scripting.Hosting;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Incubator_2.Forms
{
    /// <summary>
    /// Логика взаимодействия для TableFiller.xaml
    /// </summary>
    public partial class TableFiller : UserControl
    {
        private VM_TableFiller vm;
        public Tag tag;
        private CommandSettings command;
        public DataTable DataTable { get { return this.vm.Grid; } }
        public TableFiller(Tag t)
        {
            InitializeComponent();
            this.tag = t;
            this.vm = new VM_TableFiller(t);
            this.DataContext = this.vm;
            this.command = t.GetCommand();
            this.MakeButton();
        }
        private void MakeButton()
        {
            if (this.command.ScriptType == ScriptType.Button)
            {
                this.CommandButtonIcon.Data = this.FindResource(this.command.Icon.ToString()) as PathGeometry;
                this.CommandButton.Visibility = Visibility.Visible;
                this.CommandButtonText.Content = this.command.Name;
            }
        }
        public void SetData(DataTable dt)
        {
            this.vm.Grid = dt;
        }
        public void SetData(string data)
        {
            this.vm.Grid = JsonConvert.DeserializeObject<DataTable>(data);
        }
        public string GetData()
        {
            return JsonConvert.SerializeObject(this.vm.Grid);
        }
        public SGeneratedTag GetAsGeneratedTag()
        {
            SGeneratedTag result = new();
            result.tag = this.tag.id;
            result.value = this.GetData();
            return result;
        }
        private void RunScript()
        {
            try
            {
                string json = JsonConvert.SerializeObject(this.vm.Grid);
                List<Dictionary<string, string>> data = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);
                ScriptScope scope = ScriptManager.GetEngine().CreateScope();
                scope.SetVariable("input_data", data);
                ScriptManager.Execute(this.command.Script, scope);
                List<Dictionary<string, string>> result = scope.GetVariable("output");
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(result));
                this.SetData(dt);
            }
            catch (Exception ex)
            {

                ProgramState.ShowErrorDialog("При обработке скрипта произошла ошибка:\n" + ex.Message);
            }

        }

        private void CommandClick(object sender, RoutedEventArgs e)
        {
            this.RunScript();
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            this.vm.Grid.Rows.Add();
        }

        private void RemoveClick(object sender, RoutedEventArgs e)
        {
            try
            {
                this.vm.Grid.Rows.RemoveAt(this.Table.SelectedIndex);
            }
            catch { }
        }
    }
}
