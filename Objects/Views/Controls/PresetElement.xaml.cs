using Incas.Objects.Components;
using Incas.Objects.Models;
using Incas.Objects.Views.Windows;
using System.Windows.Controls;

namespace Incas.Objects.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для PresetElement.xaml
    /// </summary>
    public partial class PresetElement : UserControl
    {
        public delegate void PresetElementAction(PresetReference preset);
        public event PresetElementAction OnUpdateRequested;
        public event PresetElementAction OnViewRequested;
        public Class ClassSource { get; set; }
        public PresetReference Preset { get; set; }
        public PresetElement(Class source, PresetReference preset)
        {
            this.InitializeComponent();
            this.ClassSource = source;
            this.Preset = preset;
            this.PresetLabel.Content = preset.Name;
        }

        private void CreateObjectClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ObjectsEditor oe = new(this.ClassSource, ObjectProcessor.GetPreset(this.ClassSource, this.Preset));
            oe.Show();
        }

        private void OpenListClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.OnViewRequested?.Invoke(this.Preset);
        }

        private void EditClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AddPreset ap = new(this.ClassSource, ObjectProcessor.GetPreset(this.ClassSource, this.Preset));
            ap.ShowDialog();
            this.OnUpdateRequested?.Invoke(this.Preset);
        }

        private async void RemoveClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            await ObjectProcessor.RemovePreset(this.ClassSource, this.Preset);
            this.OnUpdateRequested?.Invoke(this.Preset);
        }

        private async void ApplyClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            await ObjectProcessor.ApplyPresetToRelevant(this.ClassSource, ObjectProcessor.GetPreset(this.ClassSource, this.Preset));
        }
    }
}
