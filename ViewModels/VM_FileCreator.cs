
using Incas.Core.ViewModels;
using Incas.Templates.Models;
using System.Collections.Generic;

namespace Incas.ViewModels
{
    public class VM_FileCreator : BaseViewModel
    {
        private Template _template;
        private string _filename;
        private string _child;
        private List<string> _childs;

        public VM_FileCreator(Template template, List<string> childs)
        {
            this._template = template;
        }
        public string FileName
        {
            get => this._filename;
            set
            {
                this._filename = value;
                this.OnPropertyChanged(nameof(this.FileName));
            }
        }
        public List<string> Children
        {
            get => this._childs;
            set
            {
                this._childs = value;
                this.OnPropertyChanged(nameof(this.Children));
            }
        }
        public string SelectedChild
        {
            get => this._child;
            set
            {
                this._child = value;
                this._template.GetChild(value);
                this.OnPropertyChanged(nameof(this.SelectedChild));
            }
        }
        public string FilePath
        {
            get => this._template.path;
            set
            {
                this._template.path = value;
                this.OnPropertyChanged(nameof(this.FilePath));
            }
        }
    }
}
