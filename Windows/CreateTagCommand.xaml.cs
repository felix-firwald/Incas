using Common;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using ICSharpCode.AvalonEdit.Highlighting;
using Incubator_2.ViewModels.VM_Templates;
using Incubator_2.Windows.CustomDatabase;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            this.DataContext = vm;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            this.vm.Script = this.CodeEditor.Text;
            this.Command = this.vm.GetData();
            Result = DialogStatus.Yes;
            this.Close();
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            Result = DialogStatus.No;
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
                    Append("from Incas import Service");
                    break;
                case "AffectsOther":
                    Append("# [affects other]");
                    break;
                case "CurrentDate":
                    Append("import datetime\n\ndatetime.datetime.strftime(datetime.datetime.now(), \"%d.%m.%Y\")");
                    break;
                case "CurrentUserFullname":
                    Append("Service.GetUserFullname()");
                    break;
                case "ShowInfo":
                    Append("Service.ShowInfoDialog(\"Описание\", \"Заголовок\")");
                    break;
                case "ShowInputBox":
                    Append("Service.ShowInputBox(\"Описание\", \"Заголовок\")");
                    break;
                case "ShowDatabaseSelection":
                    BindingSelector bs = ProgramState.ShowBindingSelector();
                    Append($"Service.ShowDatabaseSelection(\"{bs.SelectedDatabase}\", \"{bs.SelectedTable}\", \"{bs.SelectedField}\")");
                    break;
            }
        }
    }
}
