using Incas.Core.Classes;
using Incas.Help.Interfaces;
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

namespace Incas.Help.Controls
{
    /// <summary>
    /// Логика взаимодействия для Classes.xaml
    /// </summary>
    public partial class Classes : UserControl, IControlHelper
    {
        public Classes()
        {
            this.InitializeComponent();
        }

        public FlowDocument GetDocument()
        {
            return this.Root;
        }

        private void Useless_Click(object sender, RoutedEventArgs e)
        {
            DialogsManager.ShowExclamationDialog("Это справка по программе, а не сама программа. Перестаньте, пожалуйста, страдать ерундой. На кнопки в программе жать будете.", "Не нажимать");
        }
    }
}
