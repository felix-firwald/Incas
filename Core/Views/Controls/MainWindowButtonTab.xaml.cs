using Incas.Admin.Views.Pages;
using Incas.Core.Classes;
using Incas.Objects.Views.Pages;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incas.Core.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для MainWindowButtonTab.xaml
    /// </summary>
    public partial class MainWindowButtonTab : UserControl
    {
        public const string CustomDatabase = "$DATABASE";
        public const string WorkspaceSettings = "$WORKSPACE";
        public const string GroupsSettings = "$GROUPS";
        public const string UsersSettings = "$USERS";
        public const string SessionsSettings = "$SESSIONS";
        public const string ClassPrefix = "$CLASS:SOLO/";
        public const string ClassCategoryPrefix = "$CLASS:CATEGORY/";
        private string internalPath = "";
        private string name = "(нет имени)";
        public delegate void NewTabAction(Control item, string id, string name);
        public event NewTabAction OnNewTabRequested;
        public MainWindowButtonTab(string path, Icon icon, string name)
        {
            this.InitializeComponent();
            this.internalPath = path;
            this.Text.Text = name;
            this.name = name;
            this.Icon.Data = IconsManager.GetGeometryIconByName(icon);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.OnNewTabRequested?.Invoke(this.GenerateControl(), this.internalPath, this.name);
        }
        private Control GenerateControl()
        {
            GroupBox gb = new();
            switch (this.internalPath)
            {
                case CustomDatabase:
                    return new CustomDatabaseMain();
                case GroupsSettings:
                    ObjectsList listGroup = new(ProgramState.CurrentWorkspace.GetDefinition().ServiceGroups);
                    gb.Content = listGroup;
                    gb.Header = "Управление группами";
                    return gb;
                case UsersSettings:
                    ObjectsList listUser = new(ProgramState.CurrentWorkspace.GetDefinition().ServiceUsers);
                    gb.Content = listUser;
                    gb.Header = "Управление пользователями";
                    return gb;
                case WorkspaceSettings:
                    return new WorkspaceManager();
                default:
                    if (this.internalPath.Contains(MainWindowButtonTab.ClassCategoryPrefix))
                    {
                        return new CustomDatabaseMain(this.internalPath.Replace(MainWindowButtonTab.ClassCategoryPrefix, ""));
                    }
                    return new();
            }
        }

        private void bbb_MouseUp(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}
