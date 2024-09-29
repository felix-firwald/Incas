using Incas.Core.Views.Controls;
using Incas.Objects.Components;
using Incas.Objects.Models;
using Incas.Objects.Views.Controls;
using System.Windows;
using System.Windows.Controls;
using Windows.ApplicationModel.Email.DataProvider;

namespace Incas.Objects.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ObjectCard.xaml
    /// </summary>
    public partial class ObjectCard : UserControl
    {
        private Class Class { get; set; }
        private ClassData ClassData { get; set; }
        private bool first;
        public delegate void FieldDataAction(FieldData data);
        public event FieldDataAction OnFilterRequested;
        public ObjectCard(Class source, bool first = true)
        {
            this.InitializeComponent();
            this.first = first;
            if (!first)
            {
                this.MainBorder.BorderThickness = new Thickness(1);
                this.MainBorder.CornerRadius = new CornerRadius(0);
            }
            this.Class = source;
            this.ClassData = source.GetClassData();
        }
        public void SetEmpty()
        {
            this.FieldsContentPanel.Children.Clear();
            this.ObjectName.Text = "(не выбран)";
            NoContent nc = new();
            this.FieldsContentPanel.Children.Add(nc);
        }
        public void UpdateFor(Object obj)
        {
            this.FieldsContentPanel.Children.Clear();
            this.ObjectName.Text = obj.Name;
            if (this.first)
            {
                ObjectFieldViewer ofAuthor = new(obj.AuthorId);
                this.FieldsContentPanel.Children.Add(ofAuthor);
                if (this.ClassData.ClassType == ClassType.Document)
                {
                    ObjectFieldViewer ofDate = new(obj.CreationDate);
                    this.FieldsContentPanel.Children.Add(ofDate);
                }
            }                  
            foreach (FieldData field in obj.Fields)
            {
                ObjectFieldViewer of = new(field, this.first);
                of.OnFilterRequested += this.Of_OnFilterRequested;
                this.FieldsContentPanel.Children.Add(of);
            }         
        }

        private void Of_OnFilterRequested(FieldData data)
        {
            this.OnFilterRequested?.Invoke(data);
        }
    }
}
