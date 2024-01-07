using Common;
using Models;
using System.ComponentModel;
using System.Windows;

namespace Incubator_2.ViewModels
{
    public class MV_MainWindow : VM_Base
    {
        private string _surname = "Фамилия";
        private string _fullname = "Имя";
        private string _post = "Должность";
        private string _workspaceName = "Имя инкубатора";

        public MV_MainWindow()
        {
            LoadInfo();
        }

        public string Surname
        {
            get { return ProgramState.CurrentUser.surname; } 
            set
            {
                if (ProgramState.CurrentUser.surname != value)
                {
                    ProgramState.CurrentUser.surname = value;
                    OnPropertyChanged(nameof(Surname));
                }      
            }
        }
        public string SecondName
        {
            get { return ProgramState.CurrentUser.secondName; }
            set
            {
                if (ProgramState.CurrentUser.secondName != value)
                {
                    ProgramState.CurrentUser.secondName = value;
                    OnPropertyChanged(nameof(SecondName));
                }
            }
        }

        public string Post
        {
            get { return ProgramState.CurrentUser.post; }
            set
            {
                if (ProgramState.CurrentUser.post != value)
                {
                    ProgramState.CurrentUser.post = value;
                    OnPropertyChanged(nameof(Post));
                }
            }
        }

        public string WorkspaceName
        {
            get { return _workspaceName; }
            set
            {
                if (_workspaceName != value)
                {
                    _workspaceName = value;
                    OnPropertyChanged("WorkspaceName");
                }
            }
        }

        private Visibility VisibilityConverter(bool b)
        {
            switch (b)
            {
                case true:
                    return Visibility.Visible;
                case false:
                    return Visibility.Collapsed;
            }
        }

        public Visibility TasksVisibility
        {
            get
            {
                return VisibilityConverter(ProgramState.CurrentUserParameters.tasks_visibility);
            }
        }
        public Visibility CommunicationVisibility
        {
            get
            {
                return VisibilityConverter(ProgramState.CurrentUserParameters.communication_visibility);
            }
        }
        public Visibility DatabaseVisibility
        {
            get
            {
                return VisibilityConverter(ProgramState.CurrentUserParameters.database_visibility);
            }
        }

        //public event PropertyChangedEventHandler PropertyChanged;
        //protected virtual void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        public void LoadInfo()
        {
            this.WorkspaceName = ProgramState.GetWorkspaceName();
        }

    }
}
