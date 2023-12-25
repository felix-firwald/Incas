
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
        public List<Tag> parentTags;
        public List<Tag> childTags;
        public VM_ChildTemplate(int parent, Template childTemplate = null)
        {
            if (childTemplate != null)
            {
                _childTemplate = childTemplate;
                GetChildrenTag(childTemplate.id);
            }
            else
            {
                _childTemplate = new Template();
                childTags = new List<Tag>();
            }
            
            GetParentTag(parent);
        }

        private void GetChildrenTag(int template)
        {
            using (Tag t = new Tag())
            {
                childTags = t.GetAllTagsByTemplate(template);
            }
        }
        private void GetParentTag(int parent)
        {
            this.parentTags = new List<Tag>();
            using (Tag t = new Tag())
            {
                t.GetAllTagsByTemplate(parent).ForEach(tag =>    // для каждого тега родительского шаблона
                {
                    int overridencount = 0;
                    foreach (Tag child in childTags) // для каждого тега наследника
                    {
                        if (child.parent == tag.id) // если он переопределен
                        {
                            break;
                        }
                    }
                    if (overridencount == 0)
                    {
                        this.parentTags.Add(tag);
                    }
                });
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
