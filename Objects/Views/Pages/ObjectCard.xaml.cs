using Incas.Core.Classes;
using Incas.Core.Views.Controls;
using Incas.Objects.Components;
using Incas.Objects.Interfaces;
using Incas.Objects.Models;
using Incas.Objects.Views.Controls;
using Incas.Objects.Views.Windows;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        private Guid id;
        private byte status;
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
                this.ObjectName.FontSize = 12;
            }
            else
            {
                this.TitleBorder.MinHeight = 100;
            }
            this.Class = source;
            this.ClassData = source.GetClassData();
        }
        private SolidColorBrush GetColor(Color color, byte a = 255)
        {
            return new SolidColorBrush(Color.FromArgb(a, color.R, color.G, color.B));
        }
        private void ShowStatus(Components.Object obj)
        {
            if (this.ClassData?.Statuses?.Count > 0)
            {
                if (obj.Status == 0)
                {
                    obj.Status = 1;
                }
                this.status = obj.Status;
                int count = this.ClassData.Statuses.Count;
                this.StatusBorder.Visibility = Visibility.Visible;
                StatusData data = new();
                try
                {
                    data = this.ClassData.Statuses[obj.Status];
                }
                catch
                {
                    data = this.ClassData.Statuses[count];
                }
                this.StatusText.Text = data.Name;
                this.StatusDescription.Text = data.Description;
                this.StatusBackground.Background = this.GetColor(data.Color, 50);
                this.StatusText.Foreground = this.GetColor(data.Color);
                this.Progress.Maximum = count;
                this.Progress.Value = obj.Status;
                if (obj.Status >= count)
                {
                    this.StatusForwardButton.Visibility = Visibility.Hidden;
                    this.StatusBackButton.Visibility = Visibility.Visible;
                }
                else if (obj.Status == 1)
                {
                    this.StatusBackButton.Visibility = Visibility.Hidden;
                    this.StatusForwardButton.Visibility = Visibility.Visible;
                }
                else
                {
                    this.StatusBackButton.Visibility = Visibility.Visible;
                    this.StatusForwardButton.Visibility = Visibility.Visible;
                }
                this.Progress.Foreground = this.GetColor(data.Color);
            }
            else
            {
                this.StatusBorder.Visibility = Visibility.Collapsed;
            }
        }
        public void SetEmpty()
        {
            this.StatusBorder.Visibility = Visibility.Collapsed;
            this.FieldsContentPanel.Children.Clear();
            this.ObjectName.Text = "(не выбран)";
            this.EditIcon.Visibility = Visibility.Collapsed;
            this.id = Guid.Empty;
            NoContent nc = new();
            this.FieldsContentPanel.Children.Add(nc);
        }
        public void UpdateFor(Components.Object obj)
        {
            this.ShowStatus(obj);
            if (this.ClassData.EditByAuthorOnly == true && obj.AuthorId != ProgramState.CurrentUser.id)
            {
                this.StatusBorder.IsEnabled = false;
                this.EditIcon.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.StatusBorder.IsEnabled = true;
                this.EditIcon.Visibility = Visibility.Visible;
                this.StatusBackButton.IsEnabled = true;
                this.StatusForwardButton.IsEnabled = true;
            }
            this.FieldsContentPanel.Children.Clear();
            this.ObjectName.Text = obj.Name;
            this.id = obj.Id;
            if (this.first)
            {
                ObjectFieldViewer ofAuthor = new(obj.AuthorId);
                this.FieldsContentPanel.Children.Add(ofAuthor);
                if (this.ClassData.ClassType == ClassType.Document)
                {
                    ObjectFieldViewer ofDate = new(obj.CreationDate, "Дата создания");
                    this.FieldsContentPanel.Children.Add(ofDate);
                    if (obj.Terminated)
                    {
                        this.StatusBorder.IsEnabled = false;
                        this.EditIcon.Visibility = Visibility.Collapsed;
                        ObjectFieldViewer terminatedCheck = new("Процесс был завершен.", 52, 201, 36);
                        this.FieldsContentPanel.Children.Insert(0, terminatedCheck);
                        ObjectFieldViewer ofTerminatedDate = new(obj.TerminatedDate, "Дата завершения процесса");
                        this.FieldsContentPanel.Children.Add(ofTerminatedDate);
                    }
                    else
                    {
                        if (this.ClassData.Statuses?.Count == obj.Status)
                        {
                            TerminateObjectProcessMessage box = new();
                            box.OnTerminateRequested += this.Box_OnTerminateRequested;
                            this.FieldsContentPanel.Children.Insert(0, box);
                        }
                    }
                }
            }
            foreach (FieldData field in obj.Fields)
            {
                ObjectFieldViewer of = new(field, this.first);
                of.OnFilterRequested += this.Of_OnFilterRequested;
                this.FieldsContentPanel.Children.Add(of);
            }
            if (this.first)
            {
                ObjectBackReferenceViewer ob = new(this.Class, this.id);
                this.FieldsContentPanel.Children.Add(ob);
            }
            ((IObjectFieldViewer)this.FieldsContentPanel.Children[this.FieldsContentPanel.Children.Count - 1]).HideSeparator();
        }

        private void Box_OnTerminateRequested()
        {
            this.StatusBorder.IsEnabled = false;
            this.EditIcon.Visibility = Visibility.Collapsed;
            ObjectProcessor.SetObjectAsTerminated(this.Class, ObjectProcessor.GetObject(this.Class, this.id));
        }

        private void Of_OnFilterRequested(FieldData data)
        {
            this.OnFilterRequested?.Invoke(data);
        }

        private void FontAwesome_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.id == Guid.Empty)
            {
                DialogsManager.ShowExclamationDialog("Объект не выбран!", "Действие невозможно");
                return;
            }
            List<Components.Object> objects = [ObjectProcessor.GetObject(this.Class, this.id)];
            ObjectsEditor oe = new(this.Class, objects);
            oe.OnUpdateRequested += this.Oe_OnUpdateRequested;
            oe.ShowDialog();
        }
        private void Oe_OnUpdateRequested()
        {
            Incas.Objects.Components.Object obj = ObjectProcessor.GetObject(this.Class, this.id);
            this.UpdateFor(obj);
        }

        private void GoBackStatusClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Incas.Objects.Components.Object obj = ObjectProcessor.GetObject(this.Class, this.id);
            if (obj.Status > 1)
            {
                obj.Status = (byte)(obj.Status - 1);
                ObjectProcessor.WriteObjects(this.Class, obj);
                this.UpdateFor(ObjectProcessor.GetObject(this.Class, this.id));
            }
            else
            {
                DialogsManager.ShowExclamationDialog("Понижение статуса невозможно.", "Действие прервано");
            }
        }

        private void GoForwardStatusClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Incas.Objects.Components.Object obj = ObjectProcessor.GetObject(this.Class, this.id);
            if (obj.Status < this.ClassData.Statuses.Count)
            {
                obj.Status = obj.Status == 0 ? (byte)(obj.Status + 2) : (byte)(obj.Status + 1);
                ObjectProcessor.WriteObjects(this.Class, obj);
                this.UpdateFor(ObjectProcessor.GetObject(this.Class, this.id));
            }
            else
            {
                DialogsManager.ShowExclamationDialog("Повышение статуса невозможно.", "Действие прервано");
            }
        }
    }
}
