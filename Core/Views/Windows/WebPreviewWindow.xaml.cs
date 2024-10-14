using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace Incas.Core.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для WebPreviewWindow.xaml
    /// </summary>
    public partial class WebPreviewWindow : Window
    {
        private readonly string path = "";
        private readonly bool removeafter;
        public WebPreviewWindow(string name, string path, bool autoremove)
        {
            this.InitializeComponent();
            this.Title = name;
            this.path = path;
            this.removeafter = autoremove;
            this.View.Source = new Uri(path);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                if (this.removeafter)
                {
                    if (File.Exists(this.path))
                    {
                        File.Delete(this.path);
                    }
                }               
            }
            catch
            {

            }
        }
    }
}
