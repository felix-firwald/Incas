using Incas.Core.Classes;
using Incas.Objects.Files.ViewModels;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Storage;
using IncasEngine.Workspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Incas.Objects.Files.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для FilesList.xaml
    /// </summary>
    public partial class FilesList : UserControl
    {
        public FilesListViewModel vm { get; set; }
        public FilesList()
        {
            this.InitializeComponent();
            this.vm = new();
            this.DataContext = this.vm;
        }

        private void AddFileClick(object sender, RoutedEventArgs e)
        {
            string file = "";
            if (DialogsManager.ShowOpenFileDialog(ref file, ""))
            {
                WorkspaceDefinition wd = ProgramState.CurrentWorkspace.GetDefinition();
                StorageObject so = (StorageObject)Helpers.CreateObjectByType(wd.ServiceStorage);
                so.SaveFile(this.vm.SelectedPath?.Source, file);
            }
        }

        private void AddFolderClick(object sender, RoutedEventArgs e)
        {

        }

        private void RefreshFilesClick(object sender, RoutedEventArgs e)
        {

        }

        private void FindInCurrentClick(object sender, RoutedEventArgs e)
        {

        }

        private void RefreshFoldersClick(object sender, RoutedEventArgs e)
        {

        }

        private void CancelSearchClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
