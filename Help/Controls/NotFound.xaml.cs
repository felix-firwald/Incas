using Incas.Help.Interfaces;
using System.Windows.Controls;
using System.Windows.Documents;
using Xceed.Wpf.AvalonDock.Controls;

namespace Incas.Help.Controls
{
    /// <summary>
    /// Логика взаимодействия для NotFound.xaml
    /// </summary>
    public partial class NotFound : UserControl, IControlHelper
    {
        public NotFound()
        {
            this.InitializeComponent();
        }

        public FlowDocument GetDocument()
        {
            return this.Root;
        }
    }
}
