using Incas.Help.Components;
using Incas.Help.Interfaces;
using System.Windows.Controls;

namespace Incas.Help.Controls
{
    /// <summary>
    /// Логика взаимодействия для HelpGeneralControl.xaml
    /// </summary>
    public partial class HelpGeneralControl : UserControl
    {
        public string HelperName { get; set; }
        public HelpGeneralControl(HelpType helpType)
        {
            this.InitializeComponent();
            this.PlaceHelpDocument(helpType);
        }
        public void PlaceHelpDocument(HelpType helpType)
        {
            IControlHelper control;
            switch (helpType)
            {
                case HelpType.Core:
                    control = new Core();
                    this.HelperName = "Справка: главная страница";
                    break;
                case HelpType.Classes_General:
                    control = new Classes();
                    this.HelperName = "Справка: классы";
                    break;
                default:
                    control = new NotFound();
                    this.HelperName = "Справка: (не найдено)";
                    break;
            }
            this.ContentPanel.Document = control.GetDocument();
        }
    }
}
