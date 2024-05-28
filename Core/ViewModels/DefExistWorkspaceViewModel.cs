namespace Incas.Core.ViewModels
{
    public class DefExistWorkspaceViewModel : BaseViewModel
    {
        private string _path;
        private string _workspacename;
        public DefExistWorkspaceViewModel()
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
