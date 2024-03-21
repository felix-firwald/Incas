using Common;
using Incubator_2.Common;
using Incubator_2.Models;
using Incubator_2.ViewModels;
using Incubator_2.Windows.CustomDatabase;
using Microsoft.Scripting.Hosting;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        public DataTable DataTable { get { return vm.Grid; } }
        public TableFiller(Tag t)
        {
            InitializeComponent();
            tag = t;
            vm = new VM_TableFiller(t);
            this.DataContext = vm;
            command = t.GetCommand();
            this.Description.Content = t.description;
            MakeButton();
        }
        private void MakeButton()
        {
            if (command.ScriptType == ScriptType.Button)
            {
                this.CommandButtonIcon.Data = FindResource(command.Icon.ToString()) as PathGeometry;
                this.CommandButton.Visibility = Visibility.Visible;
                this.CommandButtonText.Content = command.Name;
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
            result.value = GetData();
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
                ScriptManager.Execute(command.Script, scope);
                List<Dictionary<string, string>> result = scope.GetVariable("output");
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(result));
                SetData(dt);
            }
            catch (Exception ex)
            {

                ProgramState.ShowErrorDialog("При обработке скрипта произошла ошибка:\n" + ex.Message);
            }

        }

        private void CommandClick(object sender, RoutedEventArgs e)
        {
            RunScript();
        }

    }
}
