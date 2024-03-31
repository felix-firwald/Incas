
using Models;

namespace Incubator_2.ViewModels
{
    public class VM_TagFiller : VM_Base
    {
        Tag tag;
        public VM_TagFiller(Tag t)
        {
            tag = t;
        }
        public string NameView
        {
            get { return tag.name; }
            set
            {
                tag.name = value;
                OnPropertyChanged(nameof(NameView));
            }
        }
        public string Name
        {
            get { return tag.name; }
            set
            {
                tag.name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string DefaultValue
        {
            get { return tag.value; }
            set
            {
                tag.value = value;
                OnPropertyChanged(nameof(DefaultValue));
            }
        }
        public string[] Enumeration
        {
            get { return tag.value.Split(';'); }
            set
            {
                OnPropertyChanged(nameof(Enumeration));
            }
        }
        public TypeOfTag TypeOf
        {
            get
            {
                return tag.type;
            }
        }
        public void UpdateTagAsChild()
        {
            tag.GetChild();
        }
    }
}
