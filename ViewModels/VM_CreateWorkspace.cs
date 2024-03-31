using Common;

namespace Incubator_2.ViewModels
{
    class VM_CreateWorkspace : VM_Base
    {
        private FirstWorkspaceData maindata;
        public VM_CreateWorkspace()
        {

        }
        public string WorkspaceName
        {
            get { return maindata.workspaceName; }
            set
            {
                maindata.workspaceName = value;
                OnPropertyChanged(nameof(WorkspaceName));
            }
        }
        public string WorkspacePath
        {
            get { return maindata.workspacePath; }
            set
            {
                maindata.workspacePath = value;
                OnPropertyChanged(nameof(WorkspacePath));
            }
        }
        public string UserSurname
        {
            get { return maindata.userSurname; }
            set
            {
                maindata.userSurname = value;
                OnPropertyChanged(nameof(UserSurname));
            }
        }
        public string UserFullname
        {
            get { return maindata.userFullname; }
            set
            {
                maindata.userFullname = value;
                OnPropertyChanged(nameof(UserFullname));
            }
        }
        public string UserPassword
        {
            get { return maindata.userPassword; }
            set
            {
                maindata.userPassword = value;
                OnPropertyChanged(nameof(UserPassword));
            }
        }
        public void RunInitializing()
        {
            ProgramState.InitWorkspace(maindata);
        }
    }
}
