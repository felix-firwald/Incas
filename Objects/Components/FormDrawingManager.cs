using ClosedXML.Excel;
using Incas.Core.Extensions;
using Incas.DialogSimpleForm.Components;
using Incas.Objects.Interfaces;
using Incas.Objects.ViewModels;
using Incas.Objects.Views.Controls;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Common.FunctionalityUtils.CustomForms;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Incas.Objects.Components
{
    public class FormDrawingManager
    {
        public struct DrawingOutputArgs
        {
            public Dictionary<Field, IFillerBase> Fillers { get; set; }
            public IServiceFieldFiller ServiceFiller { get; set; }
            public Dictionary<Button, Method> Buttons { get; set; }
            public Dictionary<Table, FieldTableFiller> Tables { get; set; }
        }
        private DrawingOutputArgs drawingOutputArgs = new();
        private IClassData classData;

        public static FormDrawingManager Start()
        {
            return new();
        }
        public DrawingOutputArgs DrawDebugForm(ClassViewModel cvm, StackPanel root)
        {
            ClassDataBase data = new()
            {
                Fields = [], Methods = [], Tables = []
            };
            foreach (FieldViewModel field in cvm.Fields)
            {
                data.Fields.Add(field.Source);
            }
            foreach (MethodViewModel method in cvm.Methods)
            {
                data.Methods.Add(method.Source);
            }
            foreach (TableViewModel table in cvm.Tables)
            {
                table.Save();
                data.Tables.Add(table.Source);
            }
            data.EditorView = new();

            foreach (ViewControlViewModel vm in cvm.ViewControls)
            {
                vm.Save();
                data.EditorView.Controls.Add(vm.Source);
            }

            return this.DrawFormBase(data, null, root);
        }

        public DrawingOutputArgs DrawForm(IObject obj, StackPanel root)
        {
            return this.DrawFormBase(obj.Class.GetClassData(), ServiceExtensionFieldsManager.GetFillerByType(obj), root);
        }
        private DrawingOutputArgs DrawFormBase(IClassData data, IServiceFieldFiller serviceFiller, StackPanel root)
        {
            root.Children.Clear();
            this.classData = data;
            this.drawingOutputArgs = new();
            Dictionary<Guid, FrameworkElement> fillersDict = [];
            this.drawingOutputArgs.Buttons = new();
            this.drawingOutputArgs.Fillers = new();
            this.drawingOutputArgs.Tables = new();
            if (serviceFiller != null)
            {
                root.Children.Add((UserControl)serviceFiller);
            }
            foreach (Field f in data.Fields)
            {
                FieldFiller ff = new(f)
                {
                    Uid = f.Id.ToString()
                };
                this.drawingOutputArgs.Fillers.Add(f, ff);
                fillersDict.Add(f.Id, ff);
            }
            if (data.Tables is not null)
            {
                foreach (Table table in data.Tables)
                {
                    FieldTableFiller ft = new(table)
                    {
                        Uid = table.Id.ToString()
                    };
                    this.drawingOutputArgs.Tables.Add(table, ft);
                    fillersDict.Add(table.Id, ft);
                }
            }            

            if (data.EditorView is null || data.EditorView.Controls is null || data.EditorView.Controls.Count == 0)
            {
                foreach (KeyValuePair<Field, IFillerBase> ff in this.drawingOutputArgs.Fillers)
                {
                    root.Children.Add((UIElement)ff.Value);
                }
                foreach (KeyValuePair<Table, FieldTableFiller> ff in this.drawingOutputArgs.Tables)
                {
                    root.Children.Add(ff.Value);
                }
            }
            else
            {
                foreach (ViewControl control in data.EditorView.Controls)
                {
                    this.DrawControl(control, root, fillersDict);
                }
            }
            drawingOutputArgs.ServiceFiller = serviceFiller;
            return drawingOutputArgs;
        }
        private void DrawControl(ViewControl vc, FrameworkElement currentParent, Dictionary<Guid, FrameworkElement> fillers)
        {
            FrameworkElement element = null;
            switch (vc.Type)
            {
                case ControlType.FieldFiller:
                    this.AddChild(currentParent, fillers[vc.Field]);
                    if (vc.Children is not null)
                    {
                        foreach (ViewControl control in vc.Children)
                        {
                            this.DrawControl(control, fillers[vc.Field], fillers);
                        }
                    }
                    return;
                case ControlType.Table:
                    this.AddChild(currentParent, fillers[vc.Table]);
                    if (vc.Children is not null)
                    {
                        foreach (ViewControl control in vc.Children)
                        {
                            this.DrawControl(control, fillers[vc.Table], fillers);
                        }
                    }
                    return;
                case ControlType.Button:
                    if (currentParent is FieldTableFiller fieldTableFiller)
                    {
                        this.PlaceButton(vc, fieldTableFiller);
                    }
                    else if (currentParent is FieldFiller fieldFiller)
                    {
                        this.PlaceButton(vc, fieldFiller);
                    }
                    else
                    {
                        this.PlaceButton(vc, currentParent);
                    }                   
                    return;
                case ControlType.Splitter:
                    this.MakeRectangle(currentParent);
                    return;
                case ControlType.Text:
                    this.MakeText(currentParent, vc.TextSettings);
                    return;
                case ControlType.VerticalStack:
                    element = new StackPanel() { Orientation = Orientation.Vertical };
                    break;
                case ControlType.HorizontalStack:
                    element = new WrapPanel();
                    break;
                case ControlType.Grid:
                    element = new UniformGrid();
                    break;
                case ControlType.Tab:
                    element = new TabControl() { Style = ResourceStyleManager.FindStyle("TabControlMain") };
                    break;
                case ControlType.TabItem:
                    element = new TabItem() { Style = ResourceStyleManager.FindStyle("TabItemMain"), Header = vc.Name };
                    break;
                case ControlType.Group:
                    element = new GroupBox() { Header = vc.Name };
                    break;
            }
            this.AddChild(currentParent, element);
            if (vc.Children is not null)
            {
                foreach (ViewControl control in vc.Children)
                {
                    this.DrawControl(control, element, fillers);
                }
            }
        }

        private void MakeRectangle(FrameworkElement currentParent)
        {
            Rectangle rect = new()
            {
                Fill = new SolidColorBrush(Color.FromRgb(67, 70, 80)),
                Margin = new Thickness(5)
            };
            if (currentParent is StackPanel)
            {
                rect.Height = 1;
            }
            else if (currentParent is WrapPanel)
            {
                rect.Width = 1;
            }
            else
            {
                return;
            }
            this.AddChild(currentParent, rect);
        }

        private void PlaceButton(ViewControl vc, FrameworkElement currentParent)
        {           
            this.AddChild(currentParent, this.MakeButton(vc));
        }
        private void PlaceButton(ViewControl vc, FieldTableFiller currentParent)
        {
            currentParent.AddButton(this.MakeButton(vc));
        }
        private void PlaceButton(ViewControl vc, FieldFiller currentParent)
        {
            currentParent.AddButton(this.MakeButton(vc));
        }
        private Button MakeButton(ViewControl vc)
        {
            Method targetMethod = new();
            foreach (Method m in this.classData.Methods)
            {
                if (m.Id == vc.RunMethod)
                {
                    targetMethod = m;
                    break;
                }
            }
            StackPanel sp = new()
            {
                Orientation = Orientation.Horizontal
            };
            Label l = new()
            {
                Content = vc.Name,
                Foreground = new SolidColorBrush(System.Windows.Media.Colors.White),
                FontFamily = ResourceStyleManager.FindFontFamily(ResourceStyleManager.FontRubik),
                VerticalAlignment = VerticalAlignment.Center
            };
            Path p = new();
            if (targetMethod.Icon != null)
            {
                p.Data = Geometry.Parse(targetMethod.Icon);
            }            
            p.Fill = targetMethod.Color.AsBrush();
            p.VerticalAlignment = VerticalAlignment.Center;
            p.Stretch = Stretch.Uniform;
            p.Height = 15;
            sp.Children.Add(p);
            sp.Children.Add(l);
            Button btn = new()
            {
                Content = sp,
                ToolTip = targetMethod.Description,
                Style = ResourceStyleManager.FindStyle(ResourceStyleManager.ButtonRectangle)
            };
            drawingOutputArgs.Buttons.Add(btn, targetMethod);
            return btn;           
        }

        private TextBlock MakeText(FrameworkElement currentParent, ViewControl.ViewControlTextSettings settings)
        {
            TextBlock tb = new();
            if (settings is null)
            {
                return tb;
            }
            tb.Text = settings.Text;
            tb.Margin = new Thickness(10);
            if (settings.Bold)
            {
                tb.FontWeight = FontWeights.Bold;
            }
            if (settings.Italic)
            {
                tb.FontStyle = FontStyles.Italic;
            }
            switch (settings.Size)
            {
                case ViewControl.ViewControlTextSettings.TextSettingsSize.Small:
                    tb.FontSize = 12;
                    break;
                case ViewControl.ViewControlTextSettings.TextSettingsSize.Medium:
                    tb.FontSize = 14;
                    break;
                case ViewControl.ViewControlTextSettings.TextSettingsSize.Large:
                    tb.FontSize = 16;
                    break;
            }
            tb.Foreground = settings.Color.AsBrush();
            tb.FontFamily = ResourceStyleManager.FindFontFamily(ResourceStyleManager.FontRubik);
            this.AddChild(currentParent, tb);
            return tb;
        }

        private void AddChild(FrameworkElement container, FrameworkElement child)
        {
            if (container is ContentControl control)
            {
                control.Content = child;
            }
            if (container is Panel panel)
            {
                panel.Children.Add(child);
            }
            if (container is ItemsControl itemsControl)
            {
                itemsControl.Items.Add(child);
            }
        }
    }
}
