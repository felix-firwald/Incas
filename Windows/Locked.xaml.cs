using System.Windows;

namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для Locked.xaml
    /// </summary>
    public partial class Locked : Window
    {
        public Locked()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
