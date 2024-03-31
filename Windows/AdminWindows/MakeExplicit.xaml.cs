using Incubator_2.Common;
using System.Windows;

namespace Incubator_2.Windows.AdminWindows
{
    /// <summary>
    /// Логика взаимодействия для MakeExplicit.xaml
    /// </summary>
    public partial class MakeExplicit : Window
    {
        public ExplicitMessage message = new ExplicitMessage();
        public MakeExplicit()
        {
            InitializeComponent();
        }

        private void SendClick(object sender, RoutedEventArgs e)
        {
            message.header = this.Header.Text;
            message.message = this.MainText.Text;
            message.message_type = GetEnumFromCombo();
            DialogResult = true;
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
            DialogResult = false;
        }
    }
}
