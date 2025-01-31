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

namespace Incas.Core.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для DialogLoading.xaml
    /// </summary>
    public partial class DialogLoading : Window
    {
        public DialogLoading(string name, string description)
        {
            this.InitializeComponent();
            this.TitleText.Content = name;
            this.SetDescription(description);
        }
        public void SetDescription(string description)
        {
            this.Description.Text = description;
        }
    }
}
