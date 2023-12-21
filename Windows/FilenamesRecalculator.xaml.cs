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

namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для FilenamesRecalculator.xaml
    /// </summary>
    public partial class FilenamesRecalculator : Window
    {
        public DialogStatus status = DialogStatus.Undefined;
        public string SelectedTag
        {
            get { return this.resultedTag.Items[this.resultedTag.SelectedIndex].ToString(); }
        }
        public string Prefix
        {
            get { return this.prefixValue.Text; }
        }
        public string Postfix
        {
            get { return this.postfixValue.Text; }
        }

        public FilenamesRecalculator(List<string> tags)
        {
            InitializeComponent();
            this.resultedTag.ItemsSource = tags;
            this.resultedTag.SelectedIndex = 0;
        }

        private void Apply(object sender, RoutedEventArgs e)
        {
            this.status = DialogStatus.Yes;
            this.Close();
        }

        private void Decline(object sender, RoutedEventArgs e)
        {
            this.status = DialogStatus.No;
            this.Close();
        }
    }
}
