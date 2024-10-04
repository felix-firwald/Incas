using Incas.Objects.Components;
using Incas.Objects.Models;
using Incas.Objects.Views.Pages;
using Incas.Users.Models;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace Incas.Objects.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для ObjectFieldViewer.xaml
    /// </summary>
    public partial class ObjectFieldViewer : UserControl, IObjectFieldViewer
    {
        private Class relationClass;
        private Components.Object relationObject;
        private bool isNestedObjectShowed = false;
        private ObjectCard card;
        public delegate void FieldDataAction(FieldData data);
        public event FieldDataAction OnFilterRequested;
        public FieldData Data { get; private set; }
        public ObjectFieldViewer(FieldData data, bool first = true)
        {
            this.InitializeComponent();
            this.Data = data;
            this.FieldName.Text = data.ClassField.VisibleName + ":";
            if (!first)
            {
                this.FilterButton.Visibility = Visibility.Collapsed;
            }
            if (data.ClassField.Type == Templates.Components.TagType.Relation)
            {
                try
                {
                    Guid id = Guid.Parse(data.Value);
                    if (id != Guid.Empty)
                    {
                        BindingData bd = data.ClassField.GetBindingData();
                        this.relationClass = new(bd.Class);
                        this.relationObject = ObjectProcessor.GetObject(this.relationClass, id);
                        this.FieldValue.Text = this.relationObject.GetFieldValue(bd.Field);
                        this.ColorizeField(130, 113, 239);
                        this.FieldValue.Cursor = System.Windows.Input.Cursors.Hand;
                        this.FieldValue.MouseDown += this.FieldValue_MouseDown;
                        this.FieldValue.ToolTip = "Кликнуть для просмотра объекта";
                    }
                }
                catch
                {
                    this.FieldValue.Text = "(объект не выбран)";
                    this.ColorizeField(200, 0, 90);
                    this.FilterButton.IsEnabled = false;
                }
            }
            else
            {
                this.FieldValue.Text = data.Value;
            }
        }

        private void FieldValue_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.isNestedObjectShowed == false)
            {
                this.isNestedObjectShowed = true;
                this.card = new(this.relationClass, false);
                this.card.UpdateFor(this.relationObject);
                this.MainGrid.Children.Add(this.card);
                Grid.SetRow(this.card, 1);
                Grid.SetColumnSpan(this.card, 3);
            }
            else
            {
                this.isNestedObjectShowed = false;
                this.MainGrid.Children.Remove(this.card);
            }
        }

        public ObjectFieldViewer(Guid data) // author
        {
            this.InitializeComponent();
            this.FieldName.Text = "Автор:";
            using (User user = new(data))
            {
                this.FieldValue.Text = user.fullname;
            }
            this.ColorizeField(243, 74, 147);
            this.FilterButton.IsEnabled = false;
        }
        public ObjectFieldViewer(DateTime date, string name) // date
        {
            this.InitializeComponent();
            this.FieldName.Text = name + ":";
            this.FieldValue.Text = date.ToString("f");
            this.ColorizeField(74, 243, 170);
            this.FilterButton.IsEnabled = false;
        }
        public ObjectFieldViewer(string value, byte r, byte g, byte b) // custom
        {
            this.InitializeComponent();
            this.FieldName.Text = value;
            this.FieldValue.Text = "";
            Grid.SetColumnSpan(this.FieldName, 2);
            this.ColorizeName(r, g, b);
            this.FilterButton.Visibility = Visibility.Collapsed;
        }
        public void HideSeparator()
        {
            this.Separator.Visibility = Visibility.Collapsed;
        }
        private void ColorizeField(byte r, byte g, byte b)
        {
            this.FieldValue.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(r, g, b));
        }
        private void ColorizeName(byte r, byte g, byte b)
        {
            this.FieldName.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(r, g, b));
        }

        private void FilterClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FieldData data = new()
            {
                Value = this.FieldValue.Text,
                ClassField = this.Data.ClassField
            };
            this.OnFilterRequested?.Invoke(data);
        }
    }
}
