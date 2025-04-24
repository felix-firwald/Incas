using Incas.Core.Classes;
using Incas.Objects.Processes.ViewModels;
using Incas.Objects.Views.Pages;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.Processes;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incas.Objects.Processes.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProcessViewer.xaml
    /// </summary>
    public partial class ProcessViewer : UserControl
    {
        public Class Source { get; set; }
        public Process Object { get; set; }
        public ProcessViewerViewModel vm { get; set; }
        public ProcessViewer(Class source, Process process)
        {
            this.InitializeComponent();
            this.vm = new(source, process);
            this.DataContext = this.vm;
            this.Source = source;
            this.Object = process;
            this.PlaceCard();
            DialogsManager.ShowWaitCursor(false);
        }
        private void PlaceCard()
        {
            ObjectCard oc = new(this.Source, false);
            oc.UpdateFor(this.Object);
            this.MainGrid.Children.Add(oc);
            Grid.SetRowSpan(oc, 4);
        }
        private void GoForwardStatusClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void GoBackStatusClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
