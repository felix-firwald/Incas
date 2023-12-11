using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace Incubator_2.Windows.ToolBar
{
    /// <summary>
    /// Логика взаимодействия для FilesManager.xaml
    /// </summary>
    public partial class FilesManager : Window
    {
        List<string> selection = new List<string>();
        public FilesManager()
        {
            InitializeComponent();
        }

        private void GetSelection(string path)
        {
            DirectoryInfo d = new DirectoryInfo(path); //Assuming Test is your Folder
            FileInfo[] Files = (bool)this.SubdirCheck.IsChecked? d.GetFiles("*.*", SearchOption.AllDirectories) : d.GetFiles(); //Getting Text files
            selection.Clear();
            this.ListSelection.Items.Clear();
            foreach (FileInfo File in Files)
            {
                if (Check(File.Name))
                {
                    selection.Add(File.FullName);
                    this.ListSelection.Items.Add(File.Name);
                }
            }
        }
        private bool Check(string path)
        {
            if (this.FilesStartWithCheck.IsChecked == true)
            {
                if (!path.StartsWith(this.StartsWithValue.Text))
                {
                    return false;
                }
            }
            if (this.FilesEndWithCheck.IsChecked == true)
            {
                if (this.IgnoreExtensionCheck.IsChecked == true)
                {
                    if (!Path.GetFileNameWithoutExtension(path).EndsWith(this.EndsWithValue.Text))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!path.EndsWith(this.EndsWithValue.Text))
                    {
                        return false;
                    }
                }
            }
            if (this.FilesContainsCheck.IsChecked == true)
            {
                if (!path.Contains(this.ContainsWithValue.Text))
                {
                    return false;
                }
            }
            return true;
        }

        private void Selection_Click(object sender, RoutedEventArgs e)
        {
            if (Path.Exists(this.Directory.Text))
            {
                GetSelection(this.Directory.Text);
            }
            
        }
    }
}
