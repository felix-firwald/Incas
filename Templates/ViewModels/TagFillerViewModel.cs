
using Incas.Core.ViewModels;
using Incas.Templates.Models;
using IncasEngine.TemplateManager;

namespace Incas.Templates.ViewModels
{
    public class TagFillerViewModel : BaseViewModel
    {
        private Tag tag;
        public TagFillerViewModel(Tag t)
        {
            this.tag = t;
        }
        public string NameView
        {
            get => this.tag.name;
            set
            {
                this.tag.name = value;
                this.OnPropertyChanged(nameof(this.NameView));
            }
        }
        public string Name
        {
            get => this.tag.name;
            set
            {
                this.tag.name = value;
                this.OnPropertyChanged(nameof(this.Name));
            }
        }
        public string DefaultValue
        {
            get => this.tag.value;
            set
            {
                this.tag.value = value;
                this.OnPropertyChanged(nameof(this.DefaultValue));
            }
        }
        public string[] Enumeration
        {
            get => this.tag.value.Split(';');
            set => this.OnPropertyChanged(nameof(this.Enumeration));
        }
        public TagType TypeOf => this.tag.type;
        public void UpdateTagAsChild()
        {
            this.tag.GetChild();
        }
    }
}
