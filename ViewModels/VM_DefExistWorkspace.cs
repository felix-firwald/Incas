namespace Incubator_2.ViewModels
{
    public class VM_DefExistWorkspace : VM_Base
    {
        private string _path;
        private string _workspacename;
        public VM_DefExistWorkspace()
        {

        }
        public string WorkspacePath
        {
            get
            {
                return this._path;
            }
            set
            {
                this._path = value;
                this.OnPropertyChanged(nameof(this.WorkspacePath));
            }
        }
        public string WorkspaceName
        {
            get
            {
                return this._workspacename;
            }
            set
            {
                this._workspacename = value;
                this.OnPropertyChanged(nameof(this.WorkspaceName));
            }
        }
    }
}
