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
    /// Логика взаимодействия для FieldEditor.xaml
    /// </summary>
    public partial class FieldEditor : UserControl
    {
        VM_FieldEditor vm;
        public FieldEditor()
        {
            InitializeComponent();
            vm = new();
            this.DataContext = vm;
        }
    }
}
