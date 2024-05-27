using Common;
using Incas.Core.Views.Windows;
using System.Collections.Generic;
using System.Windows;

namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для FilenamesRecalculator.xaml
    /// </summary>
    public partial class FilenamesRecalculator : Window
    {
        public DialogStatus status = DialogStatus.Undefined;
        public string SelectedTag
        {
            get { return this.resultedTag.Items[this.resultedTag.SelectedIndex].ToString(); }
        }
        public string Prefix
        {
            get { return this.prefixValue.Text; }
        }
        public string Postfix
        {
            get { return this.postfixValue.Text; }
        }
        public bool IsAdditive
        {
            get { return (bool)this.Additive.IsChecked; }
        }
        private int template;

        public FilenamesRecalculator(int templ, List<string> tags)
        {
            InitializeComponent();
            this.template = templ;
            this.prefixValue.Text = RegistryData.GetTemplatePreferredPrefix(this.template.ToString());
            this.postfixValue.Text = RegistryData.GetTemplatePreferredPostfix(this.template.ToString());
            this.resultedTag.ItemsSource = tags;
            this.resultedTag.SelectedIndex = 0;
        }

        private void Apply(object sender, RoutedEventArgs e)
        {
            RegistryData.SetTemplatePreferredPrefix(this.template.ToString(), this.prefixValue.Text);
            RegistryData.SetTemplatePreferredPostfix(this.template.ToString(), this.postfixValue.Text);
            this.status = DialogStatus.Yes;
            this.Close();
        }

        private void Decline(object sender, RoutedEventArgs e)
        {
            this.status = DialogStatus.No;
            this.Close();
        }
    }
}
