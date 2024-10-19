using Incas.Objects.Components;
using Incas.Objects.Models;
using Incas.Objects.Views.Pages;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incas.Objects.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для ObjectElement.xaml
    /// </summary>
    public partial class ObjectElement : UserControl
    {
        private bool visible = false;
        public Class Class { get; set; }
        public string Id { get; set; }
        public ObjectElement(Class cl, string id, string name)
        {
            this.InitializeComponent();
            this.Class = cl;
            this.Id = id;
            this.ObjectName.Text = name;
        }

        private void ObjectName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.visible)
            {
                this.visible = false;
                this.HideCard();
            }
            else
            {
                this.visible = true;
                this.ShowCard();
            }
        }
        private void ShowCard()
        {
            ObjectCard oc = new(this.Class, false);
            Components.Object obj = ObjectProcessor.GetObject(this.Class, Guid.Parse(this.Id));
            oc.UpdateFor(obj);
            this.Border.Child = oc;
        }
        private void HideCard()
        {
            this.Border.Child = null;
        }
    }
}
