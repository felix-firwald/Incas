using Incas.Objects.Interfaces;
using Incas.Objects.ViewModels;
using IncasEngine.ObjectiveEngine.Interfaces;
using System.Windows.Controls;

namespace Incas.Objects.Processes.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProcessClassPart.xaml
    /// </summary>
    public partial class ProcessClassPart : UserControl, IClassPartSettings
    {
        public string ItemName => "Настройка процессов";

        public ProcessClassPart()
        {
#if !E_FREE
            this.InitializeComponent();
#endif
        }     

        public IClassPartSettings SetUp(ClassViewModel classViewModel)
        {
            return this;
        }

        public void Save()
        {
            throw new System.NotImplementedException();
        }
    }
}
