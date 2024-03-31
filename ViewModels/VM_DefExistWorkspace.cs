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
                return _path;
            }
            set
            {
                _path = value;
                OnPropertyChanged(nameof(WorkspacePath));
            }
        }
        public string WorkspaceName
        {
            get
            {
                return _workspacename;
            }
            set
            {
                _workspacename = value;
                OnPropertyChanged(nameof(WorkspaceName));
            }
        }
    }
}
