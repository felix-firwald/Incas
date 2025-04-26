using Incas.Core.Classes;
using Incas.Core.Extensions;
using Incas.Core.Views.Controls;
using Incas.DialogSimpleForm.Components;
using Incas.Objects.Components;
using Incas.Objects.Interfaces;
using Incas.Objects.Views.Controls;
using Incas.Objects.Views.Windows;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Groups.Components;
using IncasEngine.Scripting;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using static IncasEngine.ObjectiveEngine.Models.State;

namespace Incas.Objects.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ObjectCard.xaml
    /// </summary>
    public partial class ObjectCard : UserControl
    {
        private IClass Class { get; set; }
        private IClassData ClassData { get; set; }
        private bool first;
        private Guid id;
        private Dictionary<Guid, UIElement> elements;
        private Dictionary<Button, Method> buttons;
        //private byte status;
        private GroupClassPermissionSettings permissionSettings { get; set; }
        public delegate void FieldDataAction(FieldData data);
        public event FieldDataAction OnFilterRequested;
        public ObjectCard(IClass source, bool first = true)
        {
            this.InitializeComponent();
            this.elements = new();
            this.first = first;
            this.buttons = new();
            if (!first)
            {
                this.ObjectName.FontSize = 12;
            }
            else
            {
                this.TitleBorder.MinHeight = 100;
            }
            this.SetClass(source);
        }
        public ObjectCard(bool first = true)
        {
            this.InitializeComponent();
            this.first = first;
            if (!first)
            {
                this.ObjectName.FontSize = 12;
            }
            else
            {
                this.TitleBorder.MinHeight = 100;
            }
        }
        public void SetClass(IClass source)
        {
            this.Class = source;
            this.ClassData = source.GetClassData();
            this.permissionSettings = this.GetPermissionSettings();
        }
        private GroupClassPermissionSettings GetPermissionSettings()
        {
            return ProgramState.CurrentWorkspace.CurrentGroup.GetClassPermissions(this.Class.Id);
        }
        private SolidColorBrush GetColor(IncasEngine.Core.Color color, byte a = 255)
        {
            return new SolidColorBrush(System.Windows.Media.Color.FromArgb(a, color.R, color.G, color.B));
        }
        private void SetButtonsByClass()
        {
            this.ButtonsPanel.Children.Clear();
            this.buttons.Clear();
            if (this.ClassData.Methods is null)
            {
                return;
            }
            foreach (Method targetMethod in this.ClassData.Methods)
            {
                Path p = new();
                if (targetMethod.Icon != null)
                {
                    p.Data = Geometry.Parse(targetMethod.Icon);
                }
                p.Fill = targetMethod.Color.AsBrush();
                p.VerticalAlignment = VerticalAlignment.Center;
                p.Stretch = Stretch.Uniform;
                p.Height = 16;
                Button btn = new()
                {
                    Content = p,
                    ToolTip = targetMethod.Description,
                    Style = ResourceStyleManager.FindStyle(ResourceStyleManager.ButtonSquare)
                };
                btn.Click += this.ButtonWithMethodClicked;
                this.ButtonsPanel.Children.Add(btn);
                this.elements.Add(targetMethod.Id, btn);
                this.buttons.Add(btn, targetMethod);
            }
        }

        private async void ButtonWithMethodClicked(object sender, RoutedEventArgs e)
        {
            Method method = this.buttons[(Button)sender];
            IObject obj = Processor.GetObject(this.Class, this.id);
            CodeOutputArgs output = obj.RunMethod(method);
            await Processor.WriteObjects(this.Class, obj);
            this.UpdateFor(obj);
        }

        public void SetEmpty()
        {
            this.Dispatcher.Invoke(() =>
            {
                this.LinkIcon.Visibility = Visibility.Collapsed;
                this.StatusBorder.Visibility = Visibility.Collapsed;
                this.FieldsContentPanel.Children.Clear();
                this.ObjectName.Text = "(не выбран)";
                this.EditIcon.Visibility = Visibility.Collapsed;
                this.id = Guid.Empty;
                NoContent nc = new();
                this.FieldsContentPanel.Children.Add(nc);
            });           
        }
        private void SetProtected()
        {
            this.Dispatcher.Invoke(() =>
            {
                this.LinkIcon.Visibility = Visibility.Collapsed;
                this.StatusBorder.Visibility = Visibility.Collapsed;
                this.FieldsContentPanel.Children.Clear();
                this.ObjectName.Text = "...";
                this.EditIcon.Visibility = Visibility.Collapsed;
                this.id = Guid.Empty;
                NoPermission np = new();
                this.FieldsContentPanel.Children.Add(np);
            });
        }
        private void SetComponentDisabled()
        {
            this.Dispatcher.Invoke(() =>
            {
                this.LinkIcon.Visibility = Visibility.Collapsed;
                this.StatusBorder.Visibility = Visibility.Collapsed;
                this.FieldsContentPanel.Children.Clear();
                this.ObjectName.Text = "...";
                this.EditIcon.Visibility = Visibility.Collapsed;
                this.id = Guid.Empty;
                ComponentNotActive np = new();
                this.FieldsContentPanel.Children.Add(np);
            });
        }
        private bool CheckAuthor(IObject obj)
        {
            if (obj is IHasAuthor objWithAuthor)
            {
                return objWithAuthor.AuthorId == ProgramState.CurrentWorkspace.CurrentUser.Id;
            }
            return true;
        }
        public void UpdateFor(IObjectEdition edit)
        {
            if (edit is null)
            {
                return;
            }
            if (!this.permissionSettings.ReadOperations)
            {
                this.SetProtected();
                return;
            }
            this.Dispatcher.Invoke(() =>
            {
                this.FieldsContentPanel.Children.Clear();
                this.ObjectName.Text = "Версия по состоянию на " + edit.EditionDate.ToString();
                this.EditIcon.Visibility = Visibility.Collapsed;
                this.LinkIcon.Visibility = Visibility.Collapsed;
                this.id = edit.Id;
                ServiceExtensionFieldsManager.AppendServiceFieldViewers(edit, this.FieldsContentPanel);
                if (edit.Fields is null)
                {
                    return;
                }
                foreach (FieldData field in edit.Fields)
                {
                    ObjectFieldViewer of = new(field, this.first);
                    of.OnFilterRequested += this.Of_OnFilterRequested;
                    this.FieldsContentPanel.Children.Add(of);
                }
                ((IObjectFieldViewer)this.FieldsContentPanel.Children[^1]).HideSeparator();
            });
        }
        public void UpdateFor(IObject obj)
        {
            if (obj is null)
            {
                return;
            }
            if (ProgramState.CurrentWorkspace.CurrentGroup.IsComponentInAccess(this.Class.Component))
            {
                if (!this.permissionSettings.ReadOperations)
                {
                    this.SetProtected();
                    return;
                }
            }
            else
            {
                this.SetComponentDisabled();
                return;
            }
            this.Dispatcher.Invoke(() =>
            {             
                this.LinkIcon.Visibility = Visibility.Visible;
                if (this.ClassData.EditByAuthorOnly == true && !this.CheckAuthor(obj))
                {
                    this.EditIcon.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.EditIcon.Visibility = Visibility.Visible;
                }
                this.StatusBorder.Visibility = Visibility.Visible;
                this.FieldsContentPanel.Children.Clear();
                this.ObjectName.Text = obj.Name;
                this.id = obj.Id;
                ServiceExtensionFieldsManager.AppendServiceFieldViewers(obj, this.FieldsContentPanel);
                if (obj.Fields is null)
                {
                    return;
                }
                this.elements = new();
                this.SetButtonsByClass();
                foreach (FieldData field in obj.Fields)
                {
                    ObjectFieldViewer of = new(field, this.first);                   
                    of.OnFilterRequested += this.Of_OnFilterRequested;
                    this.FieldsContentPanel.Children.Add(of);
                    this.elements.Add(field.ClassField.Id, of);
                }
                ((IObjectFieldViewer)this.FieldsContentPanel.Children[^1]).HideSeparator();
                this.ApplyState(obj);
            });         
        }

        public void ApplyState(IObject obj)
        {
            if (this.ClassData.States is null)
            {
                return;
            }
            foreach (IncasEngine.ObjectiveEngine.Models.State state in this.ClassData.States)
            {
                if (state.Id == obj.State)
                {
                    foreach (KeyValuePair<Guid, UIElement> element in this.elements)
                    {
                        MemberState memberState = state.Settings[element.Key];
                        element.Value.IsEnabled = memberState.IsEnabled;
                        element.Value.Visibility = memberState.CardVisibility? Visibility.Visible : Visibility.Collapsed;
                    }
                    break;
                }
            }
        }

        private void Box_OnTerminateRequested()
        {
            this.StatusBorder.IsEnabled = false;
            this.EditIcon.Visibility = Visibility.Collapsed;
            Processor.SetObjectAsTerminated(this.Class, (ITerminable)Processor.GetObject(this.Class, this.id));
        }

        private void Of_OnFilterRequested(FieldData data)
        {
            this.OnFilterRequested?.Invoke(data);
        }

        private void EditClick(object sender, RoutedEventArgs e)
        {
            if (this.id == Guid.Empty)
            {
                DialogsManager.ShowExclamationDialog("Объект не выбран!", "Действие невозможно");
                return;
            }
            List<IObject> objects = [Processor.GetObject(this.Class, this.id)];
            ObjectsEditor oe = new(this.Class, objects);
            oe.OnUpdateRequested += this.Oe_OnUpdateRequested;
            oe.ShowDialog();
        }
        private void Oe_OnUpdateRequested()
        {
            IObject obj = Processor.GetObject(this.Class, this.id);
            this.UpdateFor(obj);
        }

        private void GoBackStatusClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IObject obj = Processor.GetObject(this.Class, this.id);
            //if (obj.Status > 1)
            //{
            //    obj.Status = (byte)(obj.Status - 1);
            //    ObjectProcessor.WriteObjects(this.Class, obj);
            //    this.UpdateFor(ObjectProcessor.GetObject(this.Class, this.id));
            //}
            //else
            //{
            //    DialogsManager.ShowExclamationDialog("Понижение статуса невозможно.", "Действие прервано");
            //}
        }

        private void GoForwardStatusClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IObject obj = Processor.GetObject(this.Class, this.id);
            //if (obj.Status < this.ClassData.Statuses.Count)
            //{
            //    obj.Status = obj.Status == 0 ? (byte)(obj.Status + 2) : (byte)(obj.Status + 1);
            //    ObjectProcessor.WriteObjects(this.Class, obj);
            //    this.UpdateFor(ObjectProcessor.GetObject(this.Class, this.id));
            //}
            //else
            //{
            //    DialogsManager.ShowExclamationDialog("Повышение статуса невозможно.", "Действие прервано");
            //}
        }

        private void OnFilesDrop(object sender, DragEventArgs e)
        {
            //if (e.Data.GetDataPresent(DataFormats.FileDrop))
            //{
            //    if (this.ClassData.ClassType != ClassType.Document)
            //    {
            //        DialogsManager.ShowExclamationDialog("Прикрепление файлов доступно только для документов.", "Действие невозможно");
            //        return;
            //    }
            //    DialogsManager.ShowWaitCursor();
            //    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            //    Components.Object obj = new();
            //    obj.Id = this.id;
            //    foreach (string file in files)
            //    {
            //        try
            //        {
            //            ObjectComment comment = new();
            //            string filename = System.IO.Path.GetFileName(file);
            //            comment.Data = filename;
            //            File.Copy(file, Processor.GetPathToAttachmentsFolder(this.Class.Id, obj.Id) + filename);
            //            Processor.WriteComment(this.Class, obj, comment);
            //        }
            //        catch (IOException ex)
            //        {
            //            DialogsManager.ShowErrorDialog("Похоже, что файл с таким именем уже существует: " + ex.Message);
            //        }
            //        catch (Exception ex)
            //        {
            //            DialogsManager.ShowErrorDialog(ex);
            //        }
            //    }
            //    DialogsManager.ShowWaitCursor(false);
            //    this.UpdateFor(Processor.GetObject(this.Class, this.id));
            //}
        }

        private void GetObjectReferenceClick(object sender, RoutedEventArgs e)
        {
            ObjectReference reference = new(this.Class.Id, this.id);
            Clipboard.SetText(reference.ToString());
        }
    }
}
