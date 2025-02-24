using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using Incas.Miniservices.TextEditor.Views.Pages;
using Incas.Objects.AutoUI;
using Incas.Objects.Views.Pages;
using Incas.Objects.Views.Windows;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.Workspace;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
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
            this.FindObject = new Command(this.DoFindObject);
            this.OpenClipBoard = new Command(this.DoOpenClipBoard);
            this.OpenTasks = new Command(this.DoOpenTasks);
            this.OpenTextEditor = new Command(this.DoOpenTextEditor);
            this.CopyFile = new Command(this.DoCopyFile);
            this.OpenFileManager = new Command(this.DoOpenFileManager);
            this.OpenWeb = new Command(this.DoOpenWeb);
            this.UserCommand = new Command(this.DoUserCommand);
        }

        private void DoUserCommand(object obj)
        {
            MenuCommand command = obj as MenuCommand;
            switch (command.Type)
            {
                case MenuCommand.CommandType.Add:
                    Class cl = new(command.TargetClass);
                    ObjectsEditor editor = new(cl);
                    editor.Show();
                    break;
                case MenuCommand.CommandType.Parse:
                    break;
                case MenuCommand.CommandType.Find:
                    Class classFind = new(command.TargetClass);
                    IClassData cd = classFind.GetClassData();
                    DataSearchPredefined search = new(cd, command.TargetField);
                    if (search.ShowDialog(command.Name))
                    {
                        ObjectsList ol = new(classFind);
                        if (search.OnlyEqual)
                        {
                            ol.UpdateViewWithSearch(search.GetData());
                        }
                        else
                        {
                            ol.UpdateViewWithFilter(search.GetData());
                        }
                        DialogsManager.ShowPageWithGroupBox(ol, "Результат поиска", command.TargetField.ToString());
                    }
                    break;
                case MenuCommand.CommandType.FindAndOpen:
                    break;
            }
        }

        #region ICommands
        public ICommand FindObject { get; private set; }
        public ICommand OpenClipBoard { get; private set; }
        public ICommand OpenTasks { get; private set; }
        public ICommand OpenTextEditor { get; private set; }
        public ICommand CopyFile { get; private set; }
        public ICommand OpenFileManager { get; private set; }
        public ICommand OpenWeb { get; private set; }
        public ICommand UserCommand { get; private set; }
        public static RoutedCommand CopyToClipBoard2 = new("CopyToClipBoard", typeof(MainWindow), [new KeyGesture(Key.F2)]);
        #endregion
        #region Tools
        public void DoFindObject(object parameter)
        {
            FindObjectByReference fo = new();
            fo.ShowDialog("Найти объект по ссылке", Icon.Search);
        }
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
            //TestSignal ts = new();
            //ts.ShowDialog("345", Classes.Icon.Pin);
        }
        public void DoCopyFile(object parameter)
        {
            
        }
        public void DoOpenFileManager(object parameter)
        {
            Miniservices.FileManager.Views.Pages.FileManagerPage fmp = new();
            DialogsManager.ShowPageWithGroupBox(fmp, "Обработчик файлов и каталогов", "$FileManager", TabType.Tool);
        }
        public void DoOpenWeb(object parameter)
        {
            DialogsManager.ShowHelp(Help.Components.HelpType.Core);
        }

        #endregion
        public List<MenuCommand> Commands
        {
            get
            {
                return ProgramState.CurrentWorkspace.GetDefinition().Commands;
            }
        }
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
            get => ProgramState.CurrentWorkspace.CurrentUser.Name;
        }
        public string Group
        {
            get => ProgramState.CurrentWorkspace.CurrentGroup.Name;
        }

        private bool testFunctionEnabled = false;
        public Visibility TestFunctionVisibility
        {
            get
            {
                return this.FromBool(this.testFunctionEnabled);
            }
            set
            {
                this.testFunctionEnabled = true;
                this.OnPropertyChanged(nameof(this.TestFunctionVisibility));
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
                    this.OnPropertyChanged(nameof(this.WorkspaceName));
                }
            }
        }
        public string Title => "Рабочее пространство: " + this.WorkspaceName;

        public void LoadInfo()
        {
            this.WorkspaceName = ProgramState.CurrentWorkspace.GetDefinition(true).Name;
            this.OnPropertyChanged(nameof(this.Surname));
        }
    }
}
