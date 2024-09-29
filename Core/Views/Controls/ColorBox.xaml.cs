using Incas.Core.ViewModels;
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

namespace Incas.Core.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для ColorBox.xaml
    /// </summary>
    public partial class ColorBox : UserControl
    {
        private ColorBoxViewModel vm;
        public Color SelectedColor
        {
            get
            {
                Color c = new()
                {
                    R = this.vm.R,
                    G = this.vm.G,
                    B = this.vm.B
                };
                return c;
            }
            set
            {
                this.vm.Apply(value);
            }
        }
        public ColorBox()
        {
            this.InitializeComponent();
            this.vm = new(0, 0, 0);
            this.DataContext = this.vm;
        }
    }
}
