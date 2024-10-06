using Incas.Core.ViewModels;
using System.Windows.Controls;
using System.Windows.Media;

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
            set => this.vm.Apply(value);
        }
        public ColorBox()
        {
            this.InitializeComponent();
            this.vm = new(0, 0, 0);
            this.DataContext = this.vm;
        }
    }
}
