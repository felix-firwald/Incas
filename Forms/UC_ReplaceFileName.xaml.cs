
using System.Windows.Controls;


namespace Incubator_2.Forms
{
    /// <summary>
    /// Логика взаимодействия для UC_ReplaceFileName.xaml
    /// </summary>
    public partial class UC_ReplaceFileName : UserControl
    {
        public UC_ReplaceFileName()
        {
            InitializeComponent();
        }
        public (string, string) GetValues()
        {
            return (this.Left.Text, this.Right.Text);
        }
    }
}
