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
            get { return _surname; } 
            set
            {
                if (_surname != value)
                {
                    _surname = value;
                    OnPropertyChanged("Surname");
                }      
            }
        }
        public string Fullname
        {
            get { return _fullname; }
            set
            {
                if (_fullname != value)
                {
                    _fullname = value;
                    OnPropertyChanged("Fullname");
                }
            }
        }

        public string Post
        {
            get { return _post; }
            set
            {
                if (_post != value)
                {
                    _post = value;
                    OnPropertyChanged("Post");
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
            using (User user = ProgramState.GetCurrentUser())
            {
                this.Fullname = user.fullname;
                this.Surname = user.surname;
                this.Post = user.post;
            }
            this.WorkspaceName = ProgramState.GetWorkspaceName();
        }

    }
}
