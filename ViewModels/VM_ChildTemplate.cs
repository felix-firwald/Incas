
using Incubator_2.Forms;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.ViewModels
{
    public class VM_ChildTemplate : VM_Base
    {
        private Template _childTemplate;
        public VM_ChildTemplate(int parent, Template childTemplate = null)
        {
            if (childTemplate != null)
            {
                _childTemplate = childTemplate;
            }
            else
            {
                _childTemplate = new Template();
            }
        }

        public string FilePath
        {
            get
            {
                return _childTemplate.path;
            }
            set
            {
                _childTemplate.path = value;
                OnPropertyChanged(nameof(FilePath));
            }
        }
        public string TemplateName
        {
            get { return _childTemplate.name; }
            set
            {
                _childTemplate.name = value;
                OnPropertyChanged(nameof(TemplateName));
            }
        }

        public List<Tag> NotOverridenTags
        {
            get
            {
                return null;
            }
            set
            {

            }
        }
    }
}
