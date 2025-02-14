using Incas.Help.Components;
using Incas.Help.Controls;
using System.Windows;

namespace Incas.Help.Windows
{
    /// <summary>
    /// Логика взаимодействия для HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow(HelpType helpType)
        {
            this.InitializeComponent();
            this.PlaceHelper(helpType);
        }
        public void PlaceHelper(HelpType helpType)
        {
            HelpGeneralControl control = new(helpType);
            this.Root.Child = control;
            this.Title = control.HelperName;
        }
    }
}
