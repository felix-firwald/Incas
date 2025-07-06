using Incas.Objects.ServiceClasses.Groups.ViewModels;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Groups.Components;
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

namespace Incas.Objects.ServiceClasses.Groups.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для MethodsRestrictions.xaml
    /// </summary>
    public partial class MethodsRestrictions : Window
    {
        public MethodsConstraintsViewModel vm { get; set; }
        public MethodsRestrictions(IncasEngine.ObjectiveEngine.Models.Class cl, GroupClassPermissionSettings settings)
        {
            this.InitializeComponent();
            this.vm = new(cl, settings);
            this.DataContext = this.vm;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            this.vm.Save();
            this.Close();
        }
    }
}
