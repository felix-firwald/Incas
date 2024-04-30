
using Models;

namespace Incubator_2.ViewModels
{
    public class VM_TagFiller : VM_Base
    {
        private Tag tag;
        public VM_TagFiller(Tag t)
        {
            this.tag = t;
        }
        public string NameView
        {
            get { return this.tag.name; }
            set
            {
                this.tag.name = value;
                this.OnPropertyChanged(nameof(this.NameView));
            }
        }
        public string Name
        {
            get { return this.tag.name; }
            set
            {
                this.tag.name = value;
                this.OnPropertyChanged(nameof(this.Name));
            }
        }
        public string DefaultValue
        {
            get { return this.tag.value; }
            set
            {
                this.tag.value = value;
                this.OnPropertyChanged(nameof(this.DefaultValue));
            }
        }
        public string[] Enumeration
        {
            get { return this.tag.value.Split(';'); }
            set
            {
                this.OnPropertyChanged(nameof(this.Enumeration));
            }
        }
        public TypeOfTag TypeOf
        {
            get
            {
                return this.tag.type;
            }
        }
        public void UpdateTagAsChild()
        {
            this.tag.GetChild();
        }
    }
}
