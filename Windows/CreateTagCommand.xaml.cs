using DocumentFormat.OpenXml.Bibliography;
using ICSharpCode.AvalonEdit.Highlighting;
using Incubator_2.ViewModels.VM_Templates;
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
    }
}
