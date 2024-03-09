
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.ViewModels
{
    class VM_Template : VM_Base
    {
        private Template template_main;
        public VM_Template(Template templ)
        {
            this.template_main = templ;
        }
        public bool IsEdit
        {
            get
            {
                if (template_main.id == 0)
                {
                    return false;
                }
                return true;
            }
        }
        public string NameOfTemplate 
        {
            get { return this.template_main.name; }
            set
            {
                if (this.template_main.name != value)
                {
                    this.template_main.name = value;
                    OnPropertyChanged("NameOfTemplate");
                }
            }
        }
        public string SourceFile
        {
            get { return this.template_main.path; }
            set
            {
                if (this.template_main.path != value)
                {
                    this.template_main.path = value;
                    OnPropertyChanged("SourceFile");
                }
            }
        }
        public string Category
        {
            get { return this.template_main.suggestedPath; }
            set
            {
                if (this.template_main.suggestedPath != value)
                {
                    this.template_main.suggestedPath = value;
                    OnPropertyChanged("Category");
                }
            }
        }
    }
}
