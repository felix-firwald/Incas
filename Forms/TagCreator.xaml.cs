using Models;
using System.Windows.Controls;


namespace Incubator_2.Forms
{
    /// <summary>
    /// Логика взаимодействия для TagCreator.xaml
    /// </summary>
    public partial class TagCreator : UserControl
    {
        Tag tag;
        public TagCreator(Tag t)
        {
            InitializeComponent();
            this.DataContext = t;
        }
    }
}
