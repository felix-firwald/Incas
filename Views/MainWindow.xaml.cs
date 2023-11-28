using Common;
using DocumentFormat.OpenXml.InkML;
using Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Incubator_2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            
            this.Fullname = "!";
            this.Surname = "!";
            this.Post = "!";
            DataContext = this;
        }

        public void LoadBio()
        {
            using (User user = ProgramState.GetCurrentUser())
            {              
                this.Fullname = user.fullname.ToString();
                this.Surname = user.surname.ToString();
                this.Post = user.post.ToString();


                //MessageBox.Show(this.dc.Surname);
                //Data data = new Data("Не работает", user.fullname, user.post);
                //this.DataContext = dc;
                this.LSurname.Text = this.surname;
            }
            this.Fullname = "УДАЛИТЬ СТРОКУ !";
        }

        private void OnClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
        
        private string surname;
        private string fullname;
        private string post;
        public string Surname
        {
            get { return surname; }
            set
            {
                if (value != surname)
                {
                    surname = value;
                    OnPropertyChanged(nameof(Surname));
                }
                
            }
        }

        public string Fullname
        {
            get { return fullname; }
            set
            {
                if (value != fullname)
                {
                    fullname = value;
                    OnPropertyChanged(nameof(Fullname));
                }
                
            }
        }
        public string Post
        {
            get { return post; }
            set
            {
                if (value != post)
                {
                    post = value;
                    OnPropertyChanged(nameof(Post));
                    //OnPropertyChanged(nameof(LabelContent));
                }
                
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            //if (PropertyChanged != null)
            //    PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
}
