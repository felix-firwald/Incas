using Incubator_2.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;


namespace Forms
{
    /// <summary>
    /// Логика взаимодействия для UC_TemplateElement.xaml
    /// </summary>
    public partial class UC_TemplateElement : UserControl
    {
        public UC_TemplateElement()
        {
            InitializeComponent();
        }

        private void AddClick(object sender, MouseButtonEventArgs e)
        {
            UseTemplate ut = new UseTemplate();
            ut.ShowDialog();
        }
    }
}
