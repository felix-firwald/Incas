using Incubator_2.Models;
using Incubator_2.ViewModels.VM_CustomDB;
using System;
using System.Collections.Generic;
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

namespace Incubator_2.Forms.Database
{
    /// <summary>
    /// Логика взаимодействия для CustomDatabaseMain.xaml
    /// </summary>
    public partial class CustomDatabaseMain : UserControl
    {
        private VM_CustomDatabase vm = new();
        public CustomDatabaseMain()
        {
            InitializeComponent();
            //FillTabesList();
            this.DataContext = vm;
        }
        public void FillTabesList()
        {
            this.TablesList.Items.Clear();
            using (CustomTable ct = new())
            {
                ct.GetTablesList().ForEach(t =>
                {
                    RadioButton rb = new();
                    rb.Content = t;
                    rb.Style = FindResource("CategoryButton") as Style;
                    this.TablesList.Items.Add(rb);
                });
            }
        }
    }
}
