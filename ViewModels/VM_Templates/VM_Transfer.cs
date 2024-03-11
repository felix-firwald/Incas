using Common;
using Incubator_2.Models.Auxiliary;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.ViewModels.VM_Templates
{
    public class VM_Transfer : VM_Base
    {
        private Transfer _transfer;
        private List<TransferMatch> _matches;
        private List<Tag> _tags;
        public VM_Transfer(Transfer transfer, List<Tag> tags)
        {
            _transfer = transfer;
            _tags = tags;
        }
        public string Name
        {
            get
            {
                return _transfer.name;
            }
            set
            {
                _transfer.name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        
        public List<STemplate> Templates
        {
            get
            {
                using (Template t = new())
                {
                    return t.GetAllWordTemplates();
                }
            }
        }
        public int SelectedTemplate
        {
            get
            {
                return _transfer.template;
            }
            set
            {
                _transfer.template = value;
                OnPropertyChanged(nameof(SelectedTemplate));
            }
        }
    }
}
