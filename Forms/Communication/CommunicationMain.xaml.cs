using Incubator_2.ViewModels.VM_Communication;
using System.Windows.Controls;

namespace Incubator_2.Forms.Communication
{
    /// <summary>
    /// Логика взаимодействия для CommunicationMain.xaml
    /// </summary>
    public partial class CommunicationMain : UserControl
    {
        VM_CommunicationMain vm;
        public CommunicationMain()
        {
            InitializeComponent();
            vm = new();
            this.DataContext = vm;
        }

    }
}
