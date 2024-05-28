using Incas.Core.Classes;
using System.Windows;

namespace Incubator_2.Windows.AdminWindows
{
    /// <summary>
    /// Логика взаимодействия для MakeExplicit.xaml
    /// </summary>
    public partial class MakeExplicit : Window
    {
        public ExplicitMessage message = new();
        public MakeExplicit()
        {
            this.InitializeComponent();
        }

        private void SendClick(object sender, RoutedEventArgs e)
        {
            this.message.header = this.Header.Text;
            this.message.message = this.MainText.Text;
            this.message.message_type = this.GetEnumFromCombo();
            this.DialogResult = true;
        }
        private AdminMessageType GetEnumFromCombo()
        {
            switch (this.Combo.SelectedIndex)
            {
                case 0:
                default:
                    return AdminMessageType.NOTIFY;
                case 1:
                    return AdminMessageType.WARNING;
                case 2:
                    return AdminMessageType.QUESTION;
            }
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
