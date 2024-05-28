using Incas.Core.ViewModels;
using IncasEngine.Database;

namespace Incas.CustomDatabases.ViewModels
{
    public class FieldEditorViewModel : BaseViewModel
    {
        private FieldCreator creator;
        private bool _fk;
        public FieldEditorViewModel()
        {

        }
        public bool IsPK
        {
            get => this.creator.IsPK;
            set
            {
                this.creator.IsPK = value;
                this.OnPropertyChanged(nameof(this.IsPK));
            }
        }
        public bool IsFK
        {
            get => this._fk;
            set
            {
                this._fk = value;
                this.OnPropertyChanged(nameof(this.IsFK));
            }
        }
        public bool IsUnique
        {
            get => this.creator.IsUNIQUE;
            set
            {
                this.creator.IsUNIQUE = value;
                this.OnPropertyChanged(nameof(this.IsUnique));
            }
        }
        public bool IsNotNull
        {
            get => this.creator.NotNULL;
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
