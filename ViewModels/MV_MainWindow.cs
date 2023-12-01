using Common;
using Models;
using System.ComponentModel;
using System.Windows;

namespace Incubator_2.ViewModels
{
    public class MV_MainWindow : INotifyPropertyChanged
    {
        private string _surname = "Фамилия";
        private string _fullname = "Имя";
        private string _post = "Должность";
        private string _incubatorname = "Имя инкубатора";

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

        //public MV_MainWindow(string surname="NO DATA", string fullname = "NO DATA", string post = "NO DATA", string incubatorName = "NO DATA")
        //{
        //    Surname = surname;
        //    Fullname = fullname;
        //    Post = post;
        //    IncubatorName = incubatorName;
        //}

        public string IncubatorName
        {
            get { return _incubatorname; }
            set
            {
                if (_incubatorname != value)
                {
                    _incubatorname = value;
                    OnPropertyChanged("IncubatorName");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void LoadInfo()
        {
            using (User user = ProgramState.GetCurrentUser())
            {
                this.Fullname = user.fullname;
                this.Surname = user.surname;
                this.Post = user.post;
                this.IncubatorName = ProgramState.GetIncubatorInfo().name;
            }
        }

    }
}
