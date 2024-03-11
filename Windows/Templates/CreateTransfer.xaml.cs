using Incubator_2.Models.Auxiliary;
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
using System.Windows.Shapes;

namespace Incubator_2.Windows.Templates
{
    /// <summary>
    /// Логика взаимодействия для CreateTransfer.xaml
    /// </summary>
    public partial class CreateTransfer : Window
    {
        public VM_Transfer vm;
        DialogStatus result = DialogStatus.Undefined;
        public CreateTransfer(List<Tag> tags) // new
        {
            InitializeComponent();
            vm = new(new(), tags);
            this.DataContext = vm;
            LoadMatches(tags);
        }
        public CreateTransfer(Transfer transfer, List<Tag> tags) // edit
        {
            InitializeComponent();
            vm = new(transfer, tags);
        }
        private void LoadMatches(List<Tag> tags)
        {
            List<Tag> choices = GetAvailableTags();
            foreach (Tag tag in tags)
            {
                TransferMatch m = new TransferMatch();
                m.From = tag.id;
                this.Matches.Children.Add(new Forms.Templates.TransferMatch(m, choices));
            }
        }
        private List<Tag> GetAvailableTags()
        {
            using (Tag t = new())
            {
                return t.GetAllTagsByTemplate(44); //vm.SelectedTemplate
            }
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {

        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            result = DialogStatus.No;
            this.Close();
        }
    }
}
