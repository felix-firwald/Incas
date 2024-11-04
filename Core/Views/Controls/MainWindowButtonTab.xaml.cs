using Incas.Admin.Views.Pages;
using Incas.Core.Classes;
using Incas.Objects.Views.Pages;
using Incas.Templates.Views.Pages;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
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
        public const string Templates = "$TEMPLATES";
        public const string CustomDatabase = "$DATABASE";
        public const string WorkspaceSettings = "$WORKSPACE";
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
            switch (this.internalPath)
            {
                case Templates:
                    return new UC_Templator();
                case CustomDatabase:
                    return new CustomDatabaseMain();
                case WorkspaceSettings:
                    return new WorkspaceManager();
                case UsersSettings:
                    return new UsersManager();
                case SessionsSettings:
                    return new SessionsManager();
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
