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
        public string Fullname
        {
            get { return ProgramState.CurrentUser.fullname; }
            set
            {
                if (ProgramState.CurrentUser.fullname != value)
                {
                    ProgramState.CurrentUser.fullname = value;
                    OnPropertyChanged(nameof(Fullname));
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
