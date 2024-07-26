using Common;
using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using Incas.Users.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Incas.Core.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private string _workspaceName = "Имя инкубатора";
        private bool _processHandled = false;

        public MainWindowViewModel()
        {
            this.SetCommands();
            this.LoadInfo();
        }
        private void SetCommands()
        {
            this.TextCommand = new Command(this.DoTextCommand);
            this.CopyToClipBoard = new Command(this.DoCopyToClipBoard);
            this.CopyFile = new Command(this.DoCopyFile);
            this.OpenFile = new Command(this.DoOpenFile);
            this.OpenWeb = new Command(this.DoOpenWeb);
        }

        #region ICommands
        public ICommand TextCommand { get; private set; }
        public ICommand CopyToClipBoard { get; private set; }
        public ICommand CopyFile { get; private set; }
        public ICommand OpenFile { get; private set; }
        public ICommand OpenWeb { get; private set; }
        public static RoutedCommand CopyToClipBoard2 = new("CopyToClipBoard", typeof(MainWindow), [new KeyGesture(Key.F2)]);
        #endregion
        #region Tools
        private void DoTextCommand(object obj)
        {
            CommandHandler.Handle(DialogsManager.ShowInputBox("Введите команду"));
        }

        public void DoCopyToClipBoard(object parameter)
        {
            Session target;
            if (DialogsManager.ShowActiveUserSelector(out target, "Выберите пользователя для копирования в буфер обмена."))
            {
                string result = DialogsManager.ShowInputBox("Текст для буфера обмена", "Укажите текст для буфера обмена");
                ServerProcessor.SendCopyTextProcess(result, target.slug);
            }
        }
        public void DoCopyFile(object parameter)
        {
            Session target;
            if (DialogsManager.ShowActiveUserSelector(out target, "Выберите пользователя для копирования файла."))
            {
                OpenFileDialog of = new();
                if (of.ShowDialog() == DialogResult.OK)
                {
                    ServerProcessor.SendCopyFileProcess(of.SafeFileName, of.FileName, target.slug);
                }
            }
        }
        public void DoOpenFile(object parameter)
        {
            Dictionary<string, string> filters = new()
            {
                { "Excel", "Файлы Excel|*.xls;*.xlsx;*.xlsm" },
                { "Word", "Файлы Word|*.doc;*.docx" },
                { "Pdf", "Файлы PDF|*.pdf" }
            };
            Session target;
            if (DialogsManager.ShowActiveUserSelector(out target, "Выберите пользователя для открытия файла."))
            {
                OpenFileDialog of2 = new()
                {
                    Filter = filters[parameter.ToString()]
                };
                if (of2.ShowDialog() == DialogResult.OK)
                {
                    ServerProcessor.SendOpenFileProcess(of2.SafeFileName, of2.FileName, target.slug);
                }
            }
        }
        public void DoOpenWeb(object parameter)
        {
            Session target;
            if (DialogsManager.ShowActiveUserSelector(out target, "Выберите пользователя для открытия страницы."))
            {
                string url = DialogsManager.ShowInputBox("Укажите адрес");
                if (!url.StartsWith("https://"))
                {
                    DialogsManager.ShowExclamationDialog("Введенный адрес либо не является адресом сети, либо небезопасен.", "Действие прервано");
                    return;
                }
                ServerProcessor.SendOpenWebProcess(url, target.slug);
            }
        }

        #endregion
        public Visibility AdminFunctionVisibility => Permission.CurrentUserPermission == PermissionGroup.Admin ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ModeratorFunctionVisibility => Permission.IsUserHavePermission(PermissionGroup.Moderator) ? Visibility.Visible : Visibility.Collapsed;
        public bool ProcessHandled
        {
            get => this._processHandled;
            set
            {
                this._processHandled = value;
                this.OnPropertyChanged(nameof(this.ProcessHandled));
            }
        }
        public string Version
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string Surname
        {
            get => ProgramState.CurrentUser.surname;
            set
            {
                if (ProgramState.CurrentUser.surname != value)
                {
                    ProgramState.CurrentUser.surname = value;
                    this.OnPropertyChanged(nameof(this.Surname));
                }
            }
        }
        public string SecondName
        {
            get => ProgramState.CurrentUser.secondName;
            set
            {
                if (ProgramState.CurrentUser.secondName != value)
                {
                    ProgramState.CurrentUser.secondName = value;
                    this.OnPropertyChanged(nameof(this.SecondName));
                }
            }
        }

        public string Post
        {
            get => ProgramState.CurrentUser.post;
            set
            {
                if (ProgramState.CurrentUser.post != value)
                {
                    ProgramState.CurrentUser.post = value;
                    this.OnPropertyChanged(nameof(this.Post));
                }
            }
        }

        public string WorkspaceName
        {
            get => this._workspaceName;
            set
            {
                if (this._workspaceName != value)
                {
                    this._workspaceName = value;
                    this.OnPropertyChanged("WorkspaceName");
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

        //public Visibility TasksVisibility => this.VisibilityConverter(ProgramState.CurrentUserParameters.tasks_visibility);
        //public Visibility CommunicationVisibility => this.VisibilityConverter(ProgramState.CurrentUserParameters.communication_visibility);
        //public Visibility DatabaseVisibility => this.VisibilityConverter(ProgramState.CurrentUserParameters.database_visibility);

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
