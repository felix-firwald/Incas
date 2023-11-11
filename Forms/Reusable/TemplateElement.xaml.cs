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

namespace Incubator_2.Forms.Reusable
{
    /// <summary>
    /// Логика взаимодействия для TemplateElement.xaml
    /// </summary>
    public partial class TemplateElement : UserControl
    {
        Template template;
        public TemplateElement(Template t)
        {
            InitializeComponent();
            template = t;
            this.NameOfTemplate.Content = template.name;
        }
    }
}
