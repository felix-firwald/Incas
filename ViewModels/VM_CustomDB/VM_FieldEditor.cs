using DocumentFormat.OpenXml.VariantTypes;
using Incubator_2.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.ViewModels.VM_CustomDB
{
    public class VM_FieldEditor : VM_Base
    {
        private FieldCreator creator;
        private bool _fk;
        public VM_FieldEditor()
        {
            
        }
        public bool IsPK
        {
            get
            {
                return creator.IsPK;
            }
            set
            {
                creator.IsPK = value;
                OnPropertyChanged(nameof(IsPK));
            }
        }
        public bool IsFK
        {
            get
            {
                return _fk;
            }
            set
            {
                _fk = value;
                OnPropertyChanged(nameof(IsFK));
            }
        }
        public bool IsUnique
        {
            get
            {
                return creator.IsUNIQUE;
            }
            set
            {
                creator.IsUNIQUE = value;
                OnPropertyChanged(nameof(IsUnique));
            }
        }
        public bool IsNotNull
        {
            get
            {
                return creator.NotNULL;
            }
            set
            {
                creator.NotNULL = value;
                OnPropertyChanged(nameof(IsNotNull));
            }
        }
    }
}
