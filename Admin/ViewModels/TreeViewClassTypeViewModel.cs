using Incas.Core.ViewModels;
using System.Collections.ObjectModel;

namespace Incas.Admin.ViewModels
{
    public class TreeViewClassTypeViewModel : BaseViewModel
    {
        public TreeViewClassTypeViewModel()
        {

        }

        private string name;
        public string Name
        {
            get => this.name;
            set
            {
                this.name = value;
                this.OnPropertyChanged(nameof(this.Name));
            }
        }
        private string icon;
        public string Icon
        {
            get
            {
                return this.icon;
            }
            set
            {
                this.icon = value;
                this.OnPropertyChanged(nameof(this.Icon));
            }
        }
        public ObservableCollection<TreeViewClassViewModel> Elements
        {
            get
            {
                return new();
            }
        }
    }
}