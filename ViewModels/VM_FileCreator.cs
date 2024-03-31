
using Models;
using System.Collections.Generic;


namespace Incubator_2.ViewModels
{
    public class VM_FileCreator : VM_Base
    {
        private Template _template;
        private string _filename;
        private string _child;
        private List<string> _childs;

        public VM_FileCreator(Template template, List<string> childs)
        {
            _template = template;
        }
        public string FileName
        {
            get { return _filename; }
            set
            {
                _filename = value;
                OnPropertyChanged(nameof(FileName));
            }
        }
        public List<string> Children
        {
            get { return _childs; }
            set
            {
                _childs = value;
                OnPropertyChanged(nameof(Children));
            }
        }
        public string SelectedChild
        {
            get { return _child; }
            set
            {
                _child = value;
                _template.GetChild(value);
                OnPropertyChanged(nameof(SelectedChild));
            }
        }
        public string FilePath
        {
            get { return _template.path; }
            set
            {
                _template.path = value;
                OnPropertyChanged(nameof(FilePath));
            }
        }
    }
}
