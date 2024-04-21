using Models;
using System.Collections.Generic;

namespace Incubator_2.ViewModels
{
    public class VM_ChildTemplate : VM_Base
    {
        public Template childTemplate;
        public List<Tag> parentTags;
        public List<Tag> childTags;
        public VM_ChildTemplate(int parent, Template child = null)
        {
            //if (child != null)
            //{
            //    childTemplate = child;
            //    childTemplate.parent = parent;
            //    GetChildrenTag(childTemplate.id);
            //}
            //else
            //{
            //    childTemplate = new Template();
            //    childTemplate.parent = parent;
            //    childTags = new List<Tag>();
            //}

            //GetParentTag(parent);
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

        public void SaveTemplate()
        {
            if (childTemplate.id != 0)
            {
                childTemplate.UpdateTemplate();
            }
            else
            {
                childTemplate.AddTemplate();
            }

        }


        public string FilePath
        {
            get
            {
                return childTemplate.path;
            }
            set
            {
                childTemplate.path = value;
                OnPropertyChanged(nameof(FilePath));
            }
        }
        public string TemplateName
        {
            get { return childTemplate.name; }
            set
            {
                childTemplate.name = value;
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
