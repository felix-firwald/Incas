using Incas.Templates.Models;
using System.Windows;
using System.Windows.Controls;

namespace Incubator_2.Forms
{
    /// <summary>
    /// Логика взаимодействия для NotOverridenTag.xaml
    /// </summary>
    public partial class NotOverridenTag : UserControl
    {
        public delegate void MethodContainer(Tag t);
        public event MethodContainer onOverride;
        public Tag tag;
        public NotOverridenTag(Tag t)
        {
            InitializeComponent();
            this.tag = t;
            this.TagName.Content = this.tag.name;
        }

        public void OverrideTag(bool notify = true)
        {
            this.IsEnabled = false;
            this.Override.Visibility = Visibility.Collapsed;
            if (notify) { onOverride(this.tag); }
        }
        public void DeOverrideTag()
        {
            this.IsEnabled = true;
            this.Override.Visibility = Visibility.Visible;
        }

        private void OverrideClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.OverrideTag();
        }
    }
}
