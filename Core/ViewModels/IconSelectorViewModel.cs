using Incas.Core.Classes;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Incas.Core.ViewModels
{
    public class IconSelectorViewModel : BaseViewModel
    {
        public IconSelectorViewModel()
        {
            
        }
        public Dictionary<Icon, string> Icons
        {
            get
            {
                return IconsManager.Icons;
            }
        }
        private KeyValuePair<Icon, string> selected;
        public KeyValuePair<Icon, string> SelectedIcon
        {
            get
            {
                return this.selected;
            }
            set
            {
                this.selected = value;
                this.OnPropertyChanged(nameof(this.SelectedIcon));
            }
        }
    }
}
