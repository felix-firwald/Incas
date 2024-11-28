using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using Incas.Miniservices.TextEditor.Views.Pages;
using Incas.Server.AutoUI;
using Incas.Users.Models;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Incas.Core.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private string _workspaceName = "Имя рабочего пространства";
        private bool _processHandled = false;

        public MainWindowViewModel()
        {
            this.SetCommands();
            this.LoadInfo();
        }
        private void SetCommands()
        {
            this.OpenClipBoard = new Command(this.DoOpenClipBoard);
            this.OpenTasks = new Command(this.DoOpenTasks);
            this.OpenTextEditor = new Command(this.DoOpenTextEditor);
            this.CopyFile = new Command(this.DoCopyFile);
            this.OpenFileManager = new Command(this.DoOpenFileManager);
            this.OpenWeb = new Command(this.DoOpenWeb);
        }

        #region ICommands
        public ICommand OpenClipBoard { get; private set; }
        public ICommand OpenTasks { get; private set; }
        public ICommand OpenTextEditor { get; private set; }
        public ICommand CopyFile { get; private set; }
        public ICommand OpenFileManager { get; private set; }
        public ICommand OpenWeb { get; private set; }
        public static RoutedCommand CopyToClipBoard2 = new("CopyToClipBoard", typeof(MainWindow), [new KeyGesture(Key.F2)]);
        #endregion
        #region Tools

        public void DoOpenClipBoard(object parameter)
        {
            DialogsManager.ShowClipboardManager();
        }
        public void DoOpenTextEditor(object parameter)
        {
            TextEditorPage tep = new();
            DialogsManager.ShowPageWithGroupBox(tep, "Редактор текста", "$TextEditor", TabType.Tool);
        }
        public void DoOpenTasks(object parameter)
        {
            TestSignal ts = new();
            ts.ShowDialog("345", Classes.Icon.Pin);
            //DialogsManager.ShowTasksManager();
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
        public void DoOpenFileManager(object parameter)
        {
            Miniservices.FileManager.Views.Pages.FileManagerPage fmp = new();
            DialogsManager.ShowPageWithGroupBox(fmp, "Обработчик файлов и каталогов", "$FileManager", TabType.Tool);
        }
        public void DoOpenWeb(object parameter)
        {
            ProgramState.OpenWebPage("https://teletype.in/@incas/main");
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
        public string Version => "Release " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

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
        public string Title => "Рабочее пространство: " + this.WorkspaceName;

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

        public void LoadInfo()
        {
            this.WorkspaceName = ProgramState.GetWorkspaceName();
        }

    }
}
