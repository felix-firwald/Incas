using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.ViewModels.VM_Templates
{
    public class VM_TransferMatch : VM_Base
    {
        private Models.Auxiliary.TransferMatch tm;
        private List<Tag> tags;
        
        public VM_TransferMatch(Models.Auxiliary.TransferMatch match)
        {
            tm = match;
        }
        public List<Tag> AvailableTags
        {
            get
            {
                return tags;
            }
            set
            {
                tags = value;
                OnPropertyChanged(nameof(AvailableTags));
            }
        }
        public int SourceTag;
        public int SelectedTag
        {
            get
            {
                return tm.To;
            }
            set
            {
                tm.To = value;
                OnPropertyChanged(nameof(SelectedTag));
            }
        }
    }
}
