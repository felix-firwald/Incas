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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Incubator_2.Forms.Templates
{
    /// <summary>
    /// Логика взаимодействия для TransferMatch.xaml
    /// </summary>
    public partial class TransferMatch : UserControl
    {
        public VM_TransferMatch vm;
        public TransferMatch(Models.Auxiliary.TransferMatch tm, List<Tag> choices)
        {
            InitializeComponent();
            vm = new(tm);
            this.DataContext = vm;
        }
        public void UpdateAvailableTags(List<Tag> tags)
        {
            vm.AvailableTags = tags;
        }
    }
}
