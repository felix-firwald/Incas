using Incas.Objects.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Interfaces
{
    public interface IMembersContainerViewModel
    {
        public ObservableCollection<FieldViewModel> Fields { get; set; }
        public ObservableCollection<MethodViewModel> StaticMethods { get; set; }
        public ObservableCollection<MethodViewModel> Methods { get; set; }
        public ObservableCollection<TableViewModel> Tables { get; set; }
        public ObservableCollection<IClassMemberViewModel> Members { get; }
    }
}
