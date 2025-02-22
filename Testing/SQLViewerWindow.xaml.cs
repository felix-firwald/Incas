using IncasEngine.Core;
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
using System.Windows.Shapes;

namespace Incas.Testing
{
    /// <summary>
    /// Логика взаимодействия для SQLViewerWindow.xaml
    /// </summary>
    public partial class SQLViewerWindow : Window
    {
        public SQLViewerWindow(Query query)
        {
            this.InitializeComponent();
            this.QueryText.Text = query.GetResult();
        }
    }
}
