using Incas.Core.ViewModels;
using Incas.Objects.Documents.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Documents.ViewModels
{
    /// <summary>
    /// Класс ViewModel.
    /// Метод Save вызывается после применения изменений на форме и подтверждает сохранение изменений из ViewModel в Model.
    /// </summary>
    public class FoundTemplateTagViewModel : BaseViewModel
    {
        public FoundTemplateTagViewModel(string source)
        {
            this.name = source;
        }
        private string name;
        public string Name
        {
            get
            {
                return this.name;
            }
        }
        private TagTarget target;
        public TagTarget Target
        {
            get
            {
                return this.target;
            }
            set
            {
                this.target = value;
                this.OnPropertyChanged(nameof(this.Target));
            }
        }
        public static List<TagTarget> AvailableTargets {
            get
            { 
                return
                    [
                        TagTarget.Property,
                        TagTarget.Field,
                        TagTarget.Useless
                    ];
            }
        }
    }
}
