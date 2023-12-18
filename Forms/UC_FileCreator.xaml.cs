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

namespace Incubator_2.Forms
{
    /// <summary>
    /// Логика взаимодействия для UC_FileCreator.xaml
    /// </summary>
    public partial class UC_FileCreator : UserControl
    {
        private bool IsCollapsed = false;
        List<Tag> tags;
        public UC_FileCreator(List<Tag> tagsList)
        {
            InitializeComponent();
            this.tags = tagsList;
            FillContentPanel();
        }
        private void FillContentPanel()
        {
            this.tags.ForEach(t =>
            {
                this.ContentPanel.Children.Add(new UC_TagFiller(t));
            });
        }

        private void Minimize(object sender, MouseButtonEventArgs e)
        {
            if (this.IsCollapsed)
            {
                this.MainBorder.Height = this.ContentPanel.Height + 40;
            }
            else
            {
                this.MainBorder.Height = 40;
            }
            this.IsCollapsed = !this.IsCollapsed;
        }
    }
}
