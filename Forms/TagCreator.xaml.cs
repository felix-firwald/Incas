using Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;


namespace Incubator_2.Forms
{
    /// <summary>
    /// Логика взаимодействия для TagCreator.xaml
    /// </summary>
    public partial class TagCreator : UserControl
    {
        Tag tag;
        private static Dictionary<TypeOfTag, int> typesOfTag = new Dictionary<TypeOfTag, int>
        {
            {TypeOfTag.Variable, 1},
            {TypeOfTag.Text, 2},
            {TypeOfTag.LocalConstant, 3},
            {TypeOfTag.LocalEnumeration, 4},
            {TypeOfTag.GlobalEnumeration, 5},
        };
        public TagCreator(Tag t, bool isNew = false)
        {
            InitializeComponent();
            this.DataContext = t;
            if (!isNew)
            {
                DeserializeType();
            }
        }
        private void DeserializeType()
        {
            this.ComboType.SelectedIndex = typesOfTag[tag.type];
        }
        private TypeOfTag SerializeType()
        {
            return typesOfTag.FirstOrDefault(x => x.Value == this.ComboType.SelectedIndex).Key;
        }
        public void SaveTag(int templ, bool isEdit=false)
        {
            tag.type = SerializeType();
            if (isEdit)
            {
                tag.UpdateTag();
            }
            else
            {
                tag.AddTag();
            }
        }
    }
}
