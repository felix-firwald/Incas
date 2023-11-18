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
    /// Логика взаимодействия для UC_ListOfDocuments.xaml
    /// </summary>
    public partial class UC_ListOfDocuments : UserControl
    {
        public UC_ListOfDocuments()
        {
            InitializeComponent();
            LoadCategories();
        }
        private void LoadCategories()
        {
            Template mt = new Template();
            mt.GetCategories().ForEach(c =>
            {
                Console.WriteLine(c);
                RadioButton rb = new RadioButton();
                rb.Style = FindResource("CategoryButton") as Style;
                rb.Content = c;
                this.Categories.Children.Add(rb);
            });
        }
    }
}
