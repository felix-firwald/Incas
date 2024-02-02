using Common;
using Incubator_2.ViewModels;
using Incubator_2.Windows.CustomDatabase;
using Models;
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
        public DataTable DataTable { get { return vm.Grid; } }
        public TableFiller(Tag t)
        {
            InitializeComponent();
            tag = t;
            vm = new VM_TableFiller(t);
            this.DataContext = vm;
        }
        public void ApplyTable(DataTable dt)
        {

        }

        private void InsertClick(object sender, RoutedEventArgs e)
        {
            BindingSelector bs = ProgramState.ShowBindingSelector();
            DatabaseSelection s = new(bs.SelectedDatabase, bs.SelectedTable, "");
            s.
        }
    }
}
