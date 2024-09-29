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

namespace Incas.Objects.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для TemplateClassElement.xaml
    /// </summary>
    public partial class TemplateClassElement : UserControl
    {
        private string name;
        private string path;
        public delegate void TemplateElementAction(string name, string path);
        public event TemplateElementAction OnEdit;
        public event TemplateElementAction OnRemove;
        public TemplateClassElement(string name, string path)
        {
            this.InitializeComponent();
            this.MainLabel.Content = name;
            this.name = name;
            this.path = path;
        }

        private void EditClick(object sender, MouseButtonEventArgs e)
        {
            this.OnEdit?.Invoke(this.name, this.path);
        }

        private void RemoveClick(object sender, MouseButtonEventArgs e)
        {
            this.OnRemove?.Invoke(this.name, this.path);
        }
    }
}
