using ClosedXML.Excel;
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

namespace Incas.Objects.Components
{
    public class FormDrawingManager
    {
        public struct DrawingOutputArgs
        {
            public Dictionary<Field, IFillerBase> Fillers { get; set; }
            public IServiceFieldFiller ServiceFiller { get; set; }
            public Dictionary<Button, Method> Buttons { get; set; }
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
                Fields = [], Methods = []
            };
            foreach (FieldViewModel field in cvm.Fields)
            {
                data.Fields.Add(field.Source);
            }
            foreach (MethodViewModel field in cvm.Methods)
            {
                data.Methods.Add(field.Source);
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
            Dictionary<Guid, IFillerBase> fillersDict = [];
            this.drawingOutputArgs.Buttons = new();
            this.drawingOutputArgs.Fillers = new();
            if (serviceFiller != null)
            {
                root.Children.Add((UserControl)serviceFiller);
            }
            foreach (Field f in data.Fields)
            {
                switch (f.Type)
                {
                    default:
                        FieldFiller ff = new(f)
                        {
                            Uid = f.Id.ToString()
                        };
                        this.drawingOutputArgs.Fillers.Add(f, ff);
                        fillersDict.Add(f.Id, ff);
                        break;
                    case FieldType.Table:
                        FieldTableFiller ft = new(f)
                        {
                            Uid = f.Id.ToString()
                        };
                        this.drawingOutputArgs.Fillers.Add(f, ft);
                        fillersDict.Add(f.Id, ft);
                        break;

                }
            }

            if (data.EditorView is null || data.EditorView.Controls is null || data.EditorView.Controls.Count == 0)
            {
                foreach (KeyValuePair<Field, IFillerBase> ff in this.drawingOutputArgs.Fillers)
                {
                    root.Children.Add((UIElement)ff.Value);
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
        private void DrawControl(ViewControl vc, FrameworkElement currentParent, Dictionary<Guid, IFillerBase> fillers)
        {
            FrameworkElement element = null;
            switch (vc.Type)
            {
                case ControlType.FieldFiller:
                    this.AddChild(currentParent, (FrameworkElement)fillers[vc.Field]);
                    return;
                case ControlType.Button:
                    Button btn = new()
                    {
                        Content = vc.Name,
                        Style = ResourceStyleManager.FindStyle(ResourceStyleManager.ButtonRectangle)
                    };
                    foreach (Method m in this.classData.Methods)
                    {
                        if (m.Id == vc.RunMethod)
                        {
                            drawingOutputArgs.Buttons.Add(btn, m);
                            this.AddChild(currentParent, btn);
                            break;
                        }
                    }                  
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
