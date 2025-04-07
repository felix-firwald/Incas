using Incas.Objects.Interfaces;
using Incas.Objects.ViewModels;
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

namespace Incas.Objects.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для TableEditor.xaml
    /// </summary>
    public partial class TableEditor : UserControl, IClassDetailsSettings
    {
        public string ItemName { get; private set; }
        public TableViewModel vm { get; private set; }
        public TableEditor(TableViewModel vm)
        {
            this.InitializeComponent();
            this.ItemName = $"Настройка таблицы [{vm.Name}]";
            this.vm = vm;
            this.DataContext = this.vm;
        }

        public void SetUpContext(ClassViewModel vm)
        {
            
        }
    }
}
