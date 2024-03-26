using Common;
using Incubator_2.Common;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Incubator_2.ViewModels
{
    public class MV_MainWindow : VM_Base
    {
        private string _surname = "Фамилия";
        private string _fullname = "Имя";
        private string _post = "Должность";
        private string _workspaceName = "Имя инкубатора";
        private bool _processHandled = false;

        public MV_MainWindow()
        {
            SetCommands();
            LoadInfo();    
        }
        private void SetCommands()
        {
            TextCommand = new Command(DoTextCommand);
            CopyToClipBoard = new Command(DoCopyToClipBoard);
            CopyFile = new Command(DoCopyFile);
            OpenFile = new Command(DoOpenFile);
            OpenWeb = new Command(DoOpenWeb);
        }
        
        #region ICommands
        public ICommand TextCommand { get; private set; }
        public ICommand CopyToClipBoard { get; private set; }
        public ICommand CopyFile { get; private set; }
        public ICommand OpenFile { get; private set; }
        public ICommand OpenWeb { get; private set; }
        public static RoutedCommand CopyToClipBoard2 = new RoutedCommand("CopyToClipBoard", typeof(MainWindow), new InputGestureCollection { new KeyGesture(Key.F2) });
        #endregion
        #region Tools
        private void DoTextCommand(object obj)
        {
            CommandHandler.Handle(ProgramState.ShowInputBox("Введите команду"));
        }

        public void DoCopyToClipBoard(object parameter)
        {
            Session target;
            if (ProgramState.ShowActiveUserSelector(out target, "Выберите пользователя для копирования в буфер обмена."))
            {
                string result = ProgramState.ShowInputBox("Текст для буфера обмена", "Укажите текст для буфера обмена");
                ServerProcessor.SendCopyTextProcess(result, target.slug);
            }
        }
        public void DoCopyFile(object parameter)
        {
            Session target;
            if (ProgramState.ShowActiveUserSelector(out target, "Выберите пользователя для копирования файла."))
            {
                OpenFileDialog of = new();
                if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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
            if (ProgramState.ShowActiveUserSelector(out target, "Выберите пользователя для открытия файла."))
            {
                OpenFileDialog of2 = new();
                of2.Filter = filters[parameter.ToString()];
                if (of2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ServerProcessor.SendOpenFileProcess(of2.SafeFileName, of2.FileName, target.slug);
                }
            }
        }
        public void DoOpenWeb(object parameter)
        {
            Session target;
            if (ProgramState.ShowActiveUserSelector(out target, "Выберите пользователя для открытия страницы."))
            {
                string url = ProgramState.ShowInputBox("Укажите адрес");
                if (!url.StartsWith("https://"))
                {
                    ProgramState.ShowExclamationDialog("Введенный адрес либо не является адресом сети, либо небезопасен.", "Действие прервано");
                    return;
                }
                ServerProcessor.SendOpenWebProcess(url, target.slug);
            }
        }


        #endregion
        public Visibility AdminFunctionVisibility
        {
            get
            {
                if (Permission.CurrentUserPermission == PermissionGroup.Admin)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        public Visibility ModeratorFunctionVisibility
        {
            get
            {
                if (Permission.IsUserHavePermission(PermissionGroup.Moderator))
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        public bool ProcessHandled
        {
            get
            {
                return _processHandled;
            }
            set
            {
                _processHandled = value;
                OnPropertyChanged(nameof(ProcessHandled));
            }
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
        public string SectorName
        {
            get { return ProgramState.CurrentSector.name; }
            set
            {
                if (ProgramState.CurrentSector.name != value)
                {
                    ProgramState.CurrentSector.name = value;
                    OnPropertyChanged("SectorName");
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
