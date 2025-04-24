using Incas.Objects.Processes.ViewModels;
using Incas.Objects.ServiceClasses.Groups.ViewModels;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Types.Processes;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Incas.Objects.Processes.Views.Pages
{
    /// <summary>
    /// OBJECT ONLY! Логика взаимодействия для ProcessSettings.xaml
    /// </summary>
    public partial class ProcessSettings : UserControl, IServiceFieldFiller
    {
        public IViewModel ViewModel { get; set; }
        public IObject TargetObject { get; set; }
        public ProcessSettings()
        {
            this.InitializeComponent();
        }

        public IObject GetResult()
        {
            ((ProcessViewModel)this.ViewModel).Save();
            return this.TargetObject;
        }

        public IServiceFieldFiller SetUp(IObject obj)
        {
            this.TargetObject = obj;
            Process proc = (Process)this.TargetObject;
            if (obj.Id == Guid.Empty)
            {
                proc.Data = new();
            }
            this.ViewModel = new ProcessViewModel(proc);
            this.DataContext = this.ViewModel;
            return this;
        }
    }
}
