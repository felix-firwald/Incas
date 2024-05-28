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
            get => this._path;
            set
            {
                this._path = value;
                this.OnPropertyChanged(nameof(this.WorkspacePath));
            }
        }
        public string WorkspaceName
        {
            get => this._workspacename;
            set
            {
                this._workspacename = value;
                this.OnPropertyChanged(nameof(this.WorkspaceName));
            }
        }
    }
}
