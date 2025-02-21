using Incas.Objects.Components;
using Incas.Objects.Views.Controls;
using Incas.Objects.Views.Windows;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Interfaces;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Incas.Objects.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для PresetsListPage.xaml
    /// </summary>
    public partial class PresetsListPage : UserControl
    {
        public IClass SourceClass { get; set; }
        public delegate void ViewRequest(IClass sourceClass, Preset preset);
        public event ViewRequest OnViewRequested;
        public PresetsListPage(IClass source, List<PresetReference> presets)
        {
            this.InitializeComponent();
            this.SourceClass = source;
            this.FillContentPanel(presets);
        }
        public void FillContentPanel(List<PresetReference> presets)
        {
            //this.ContentPanel.Children.Clear();
            //foreach (PresetReference preset in presets)
            //{
            //    PresetElement pe = new(this.SourceClass, preset);
            //    pe.OnUpdateRequested += this.Pe_OnUpdateRequested;
            //    pe.OnViewRequested += this.Pe_OnViewRequested;
            //    this.ContentPanel.Children.Add(pe);
            //}
        }

        private void Pe_OnViewRequested(PresetReference preset)
        {
            this.OnViewRequested?.Invoke(this.SourceClass, Processor.GetPreset(this.SourceClass, preset));
        }
        private void UpdatePresets()
        {
            List<PresetReference> presets = Processor.GetPresetsReferences(this.SourceClass);
            this.FillContentPanel(presets);
        }
        private void Pe_OnUpdateRequested(PresetReference preset)
        {
            this.UpdatePresets();
        }

        private void AddPresetClick(object sender, System.Windows.RoutedEventArgs e)
        {
            AddPreset ap = new(this.SourceClass);
            ap.OnUpdateRequested += this.Ap_OnUpdateRequested;
            ap.ShowDialog();
        }

        private void Ap_OnUpdateRequested()
        {
            this.UpdatePresets();
        }
    }
}
