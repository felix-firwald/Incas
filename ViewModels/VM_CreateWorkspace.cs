using Common;

namespace Incubator_2.ViewModels
{
    internal class VM_CreateWorkspace : VM_Base
    {
        private FirstWorkspaceData maindata;
        public VM_CreateWorkspace()
        {

        }
        public string WorkspaceName
        {
            get { return this.maindata.workspaceName; }
            set
            {
                this.maindata.workspaceName = value;
                this.OnPropertyChanged(nameof(this.WorkspaceName));
            }
        }
        public string WorkspacePath
        {
            get { return this.maindata.workspacePath; }
            set
            {
                this.maindata.workspacePath = value;
                this.OnPropertyChanged(nameof(this.WorkspacePath));
            }
        }
        public string UserSurname
        {
            get { return this.maindata.userSurname; }
            set
            {
                this.maindata.userSurname = value;
                this.OnPropertyChanged(nameof(this.UserSurname));
            }
        }
        public string UserFullname
        {
            get { return this.maindata.userFullname; }
            set
            {
                this.maindata.userFullname = value;
                this.OnPropertyChanged(nameof(this.UserFullname));
            }
        }
        public string UserPassword
        {
            get { return this.maindata.userPassword; }
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
