using Incas.Help.Interfaces;
using System;
using System.Windows.Controls;
using System.Windows.Documents;
using Xceed.Wpf.AvalonDock.Controls;

namespace Incas.Help.Controls
{
    /// <summary>
    /// Логика взаимодействия для Core.xaml
    /// </summary>
    public partial class Core : UserControl, IControlHelper
    {
        public Core()
        {
            this.InitializeComponent();
        }

        public FlowDocument GetDocument()
        {
            return this.Root;
        }
    }
}
