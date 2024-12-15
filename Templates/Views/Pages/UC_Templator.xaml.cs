using Incas.Core.Interfaces;
using System.Windows.Controls;
using static Incas.Core.Interfaces.ITabItem;

namespace Incas.Templates.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для UC_Templator.xaml
    /// </summary>
    public partial class UC_Templator : UserControl, ITabItem
    {
        public event TabAction OnClose;
        public string Id { get; set; }
        public UC_Templator()
        {
            this.InitializeComponent();
        }
    }
}
