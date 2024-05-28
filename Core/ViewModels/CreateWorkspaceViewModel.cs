using Incas.Core.Classes;

namespace Incas.Core.ViewModels
{
    internal class CreateWorkspaceViewModel : BaseViewModel
    {
        private FirstWorkspaceData maindata;
        public CreateWorkspaceViewModel()
        {

        }
        public string WorkspaceName
        {
            get => this.maindata.workspaceName;
            set
            {
                this.maindata.workspaceName = value;
                this.OnPropertyChanged(nameof(this.WorkspaceName));
            }
        }
        public string WorkspacePath
        {
            get => this.maindata.workspacePath;
            set
            {
                this.maindata.workspacePath = value;
                this.OnPropertyChanged(nameof(this.WorkspacePath));
            }
        }
        public string UserSurname
        {
            get => this.maindata.userSurname;
            set
            {
                this.maindata.userSurname = value;
                this.OnPropertyChanged(nameof(this.UserSurname));
            }
        }
        public string UserFullname
        {
            get => this.maindata.userFullname;
            set
            {
                this.maindata.userFullname = value;
                this.OnPropertyChanged(nameof(this.UserFullname));
            }
        }
        public string UserPassword
        {
            get => this.maindata.userPassword;
            set
            {
                this.maindata.userPassword = value;
                this.OnPropertyChanged(nameof(this.UserPassword));
            }
        }
        public void RunInitializing()
        {
            ProgramState.InitWorkspace(this.maindata);
        }
    }
}
