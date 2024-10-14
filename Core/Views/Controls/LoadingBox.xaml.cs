using Incas.Core.Interfaces;
using Incas.Core.ViewModels;
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

namespace Incas.Core.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для LoadingBox.xaml
    /// </summary>
    public partial class LoadingBox : UserControl, IStatusBar
    {
        private LoadingBoxViewModel vm;
        public LoadingBox()
        {
            this.InitializeComponent();
            this.vm = new();
            this.DataContext = this.vm;         
        }
        public void SetText(string text)
        {
            //this.Dispatcher.Invoke(() =>
            //{
            //    this.vm.Text = text;
            //});
            this.vm.Text = text;
        }
    }
}
