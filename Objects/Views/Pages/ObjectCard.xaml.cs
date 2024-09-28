using Incas.Objects.Components;
using Incas.Objects.Models;
using Incas.Objects.Views.Controls;
using System.Windows.Controls;

namespace Incas.Objects.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ObjectCard.xaml
    /// </summary>
    public partial class ObjectCard : UserControl
    {
        private Class Class { get; set; }
        private ClassData ClassData { get; set; }
        public delegate void FieldDataAction(FieldData data);
        public event FieldDataAction OnFilterRequested;
        public ObjectCard(Class source)
        {
            this.InitializeComponent();
            this.Class = source;
            this.ClassData = source.GetClassData();
        }
        public void UpdateFor(Object obj)
        {
            this.FieldsContentPanel.Children.Clear();
            this.ObjectName.Text = obj.Name;
            ObjectFieldViewer ofAuthor = new(obj.AuthorId);
            this.FieldsContentPanel.Children.Add(ofAuthor);
            if (this.ClassData.ClassType == ClassType.Document)
            {
                ObjectFieldViewer ofDate = new(obj.CreationDate);
                this.FieldsContentPanel.Children.Add(ofDate);
            }          
            foreach (FieldData field in obj.Fields)
            {
                ObjectFieldViewer of = new(field);
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
