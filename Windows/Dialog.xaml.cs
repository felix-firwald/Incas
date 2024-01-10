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
    /// Логика взаимодействия для Error.xaml
    /// </summary>
    public partial class Dialog : Window
    {
        public enum DialogIcon
        {
            Error,
            Exlamation,
            Info
        }
        public Dialog(string text, string title="Неизвестная ошибка", DialogIcon ic = DialogIcon.Error)
        {
            InitializeComponent();
            this.Title.Content = title;
            this.Description.Text = text;
            switch (ic)
            {
                case DialogIcon.Error:
                    this.IconError.Visibility = Visibility.Visible;
                    break;
                case DialogIcon.Exlamation:
                    this.IconExlamation.Visibility = Visibility.Visible;
                    break;
                case DialogIcon.Info:
                    this.IconInfo.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
