using Common;
using Incas.Core.Views.Windows;
using Incubator_2.ViewModels.VM_Templates;
using Incubator_2.Windows.CustomDatabase;
using Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateTagCommand.xaml
    /// </summary>
    public partial class CreateTagCommand : Window
    {
        public CommandSettings Command;
        public DialogStatus Result = DialogStatus.Undefined;
        public VM_TagCommand vm;
        public CreateTagCommand(CommandSettings cs)
        {
            InitializeComponent();
            this.Command = cs;
            this.vm = new(cs);
            this.CodeEditor.Text = this.vm.Script;
            this.DataContext = this.vm;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            this.vm.Script = this.CodeEditor.Text;
            this.Command = this.vm.GetData();
            this.Result = DialogStatus.Yes;
            this.Close();
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            this.Result = DialogStatus.No;
            this.Close();
        }

        private void GetMoreInfoClick(object sender, MouseButtonEventArgs e)
        {
            ProgramState.OpenWebPage("https://teletype.in/@incas/kOef49wr3J4");
        }

        private void Append(string text)
        {
            this.CodeEditor.Text += "\n" + text;
        }

        private void HelpScriptClick(object sender, RoutedEventArgs e)
        {
            switch (((MenuItem)sender).Tag)
            {
                case "IncasLibrary":
                    this.Append("from Incas import Service");
                    break;
                case "AffectsOther":
                    this.Append("# [affects other]");
                    break;
                case "CurrentDate":
                    this.Append("import datetime\n\ndatetime.datetime.strftime(datetime.datetime.now(), \"%d.%m.%Y\")");
                    break;
                case "CurrentUserFullname":
                    this.Append("Service.GetUserFullname()");
                    break;
                case "ShowInfo":
                    this.Append("Service.ShowInfoDialog(\"Описание\", \"Заголовок\")");
                    break;
                case "ShowInputBox":
                    this.Append("Service.ShowInputBox(\"Описание\", \"Заголовок\")");
                    break;
                case "ShowDatabaseSelection":
                    BindingSelector bs = ProgramState.ShowBindingSelector();
                    this.Append($"Service.ShowDatabaseSelection(\"{bs.SelectedDatabase}\", \"{bs.SelectedTable}\", \"{bs.SelectedField}\")");
                    break;
            }
        }
    }
}
