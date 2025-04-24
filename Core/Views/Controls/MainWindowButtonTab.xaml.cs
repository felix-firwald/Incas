using Incas.Admin.ViewModels;
using Incas.Admin.Views.Pages;
using Incas.Core.Classes;
using Incas.Core.Extensions;
using Incas.Objects.Files.Views.Pages;
using Incas.Objects.Views.Pages;
using IncasEngine.Workspace;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Incas.Core.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для MainWindowButtonTab.xaml
    /// </summary>
    public partial class MainWindowButtonTab : UserControl
    {
        public const string CustomDatabase = "$DATABASE";
        public const string Files = "$FILES";
        public const string WorkspaceSettings = "$WORKSPACE";
        public const string GroupsSettings = "$GROUPS";
        public const string UsersSettings = "$USERS";
        public const string SessionsSettings = "$SESSIONS";
        public const string ClassPrefix = "$CLASS:SOLO/";
        public const string ClassCategoryPrefix = "$CLASS:CATEGORY/";
        private string internalPath = "";
        private WorkspaceComponent component;
        private WorkspaceComponentButtonViewModel vm { get; set; }
        private string name = "(нет имени)";
        public delegate void NewTabAction(Control item, string id, string name);
        public event NewTabAction OnNewTabRequested;
        public MainWindowButtonTab(WorkspaceComponent component)
        {
            this.InitializeComponent();
            this.vm = new(component);
            this.DataContext = this.vm;
            this.internalPath = component.Id.ToString();
            this.name = component.Name;
            this.component = component;
            this.Text.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        }
        public MainWindowButtonTab(string visibleName, Classes.Icon icon, string path, string description)
        {
            this.InitializeComponent();
            this.internalPath = path;
            this.Text.Text = visibleName;
            this.name = visibleName;
            this.ToolTip = description;
            this.vm = new(visibleName, description, IconsManager.GetIconByName(icon), IncasEngine.Core.Color.FromRGB(255, 255, 255));
            this.DataContext = this.vm;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DialogsManager.ShowWaitCursor();
            this.OnNewTabRequested?.Invoke(this.GenerateControl(), this.internalPath, this.name);
            DialogsManager.ShowWaitCursor(false);
        }
        private Control GenerateControl()
        {
            GroupBox gb = new();
            if (this.component != null)
            {
                return new CustomDatabaseMain(this.component);
            }
            switch (this.internalPath)
            {
                case CustomDatabase:
                    return new CustomDatabaseMain();
                case Files:
                    FilesList fl = new();
                    gb.Content = fl;
                    gb.Header = "Каталог файлов";
                    return gb;
                case GroupsSettings:
                    ObjectsList listGroup = new(ProgramState.CurrentWorkspace.GetDefinition().ServiceGroups);
                    listGroup.OpenInNewTabButton.Visibility = System.Windows.Visibility.Collapsed;
                    listGroup.JumpToBackReferenceButton.Visibility = System.Windows.Visibility.Collapsed;
                    gb.Content = listGroup;
                    gb.Header = "Управление группами";
                    return gb;
                case UsersSettings:
                    ObjectsList listUser = new(ProgramState.CurrentWorkspace.GetDefinition().ServiceUsers);
                    listUser.OpenInNewTabButton.Visibility = System.Windows.Visibility.Collapsed;
                    listUser.JumpToBackReferenceButton.Visibility = System.Windows.Visibility.Collapsed;
                    gb.Content = listUser;
                    gb.Header = "Управление пользователями";
                    return gb;
                case WorkspaceSettings:
                    return new WorkspaceManager();
                default:                   
                    return new CustomDatabaseMain();
            }
        }

        public void Minimize()
        {            
            this.Text.Visibility = System.Windows.Visibility.Collapsed;
            Grid.SetColumnSpan(this.Icon, 2);
            ToolTipService.SetInitialShowDelay(this, 100);
        }
        public void Maximize()
        {
            this.Text.Visibility = System.Windows.Visibility.Visible;
            Grid.SetColumnSpan(this.Icon, 1);
            ToolTipService.SetInitialShowDelay(this, 200);
        }

        private void bbb_MouseUp(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}
