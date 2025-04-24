using Incas.Objects.Events.ViewModels;
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

namespace Incas.Objects.Events.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для EventClassPart.xaml
    /// </summary>
    public partial class EventClassPart : UserControl, IClassPartSettings
    {
        public event IClassPartSettings.OpenAdditionalSettings OnAdditionalSettingsOpenRequested;
        public EventClassPartViewModel vm { get; set; }
        public EventClassPart()
        {
            this.InitializeComponent();
        }

        public string ItemName => "Правила маршрутизации";

        public void Save()
        {
            throw new NotImplementedException();
        }

        public IClassPartSettings SetUp(ClassViewModel classViewModel)
        {
            this.vm = new(classViewModel);
            this.DataContext = this.vm;
            return this;
        }
    }
}
