using Incubator_2.Common;

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
                return this.creator.IsPK;
            }
            set
            {
                this.creator.IsPK = value;
                this.OnPropertyChanged(nameof(this.IsPK));
            }
        }
        public bool IsFK
        {
            get
            {
                return this._fk;
            }
            set
            {
                this._fk = value;
                this.OnPropertyChanged(nameof(this.IsFK));
            }
        }
        public bool IsUnique
        {
            get
            {
                return this.creator.IsUNIQUE;
            }
            set
            {
                this.creator.IsUNIQUE = value;
                this.OnPropertyChanged(nameof(this.IsUnique));
            }
        }
        public bool IsNotNull
        {
            get
            {
                return this.creator.NotNULL;
            }
            set
            {
                this.creator.NotNULL = value;
                this.OnPropertyChanged(nameof(this.IsNotNull));
            }
        }
        public string GetDefinition()
        {
            return this.creator.ToString();
        }
    }
}
