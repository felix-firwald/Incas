using Incas.DialogSimpleForm.Components;
using Incas.Objects.Interfaces;
using Incas.Objects.ViewModels;
using Incas.Objects.Views.Controls;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Common.FunctionalityUtils.CustomForms;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Incas.Objects.Components
{
    public static class FormDrawingManager
    {
        public struct DrawingOutputArgs
        {
            public List<IFillerBase> Fillers { get; set; }
            public IServiceFieldFiller ServiceFiller { get; set; }
        }

        public static DrawingOutputArgs DrawDebugForm(ClassViewModel cvm, StackPanel root)
        {
            ClassDataBase data = new();
            data.Fields = new();
            foreach (FieldViewModel field in cvm.Fields)
            {
                data.Fields.Add(field.Source);
            }
            data.EditorView = new();
            
            foreach (ViewControlViewModel vm in cvm.ViewControls)
            {
                vm.Save();
                data.EditorView.Controls.Add(vm.Source);
            }

            return DrawFormBase(data, null, root);
        }

        public static DrawingOutputArgs DrawForm(IObject obj, StackPanel root)
        {            
            return DrawFormBase(obj.Class.GetClassData(), ServiceExtensionFieldsManager.GetFillerByType(obj), root);
        }
        private static DrawingOutputArgs DrawFormBase(IClassData data, IServiceFieldFiller serviceFiller, StackPanel root)
        {
            DrawingOutputArgs result = new();
            root.Children.Clear();
            Dictionary<Guid, IFillerBase> fillersDict = new();
            List<IFillerBase> fillers = new();
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
                        fillers.Add(ff);
                        fillersDict.Add(f.Id, ff);
                        break;
                    case FieldType.Table:
                        FieldTableFiller ft = new(f)
                        {
                            Uid = f.Id.ToString()
                        };
                        fillers.Add(ft);
                        fillersDict.Add(f.Id, ft);
                        break;

                }
            }
            if (data.EditorView is null || data.EditorView.Controls is null || data.EditorView.Controls.Count == 0)
            {
                foreach (IFillerBase ff in fillers)
                {
                    root.Children.Add((UIElement)ff);
                }
            }
            else
            {
                foreach (ViewControl control in data.EditorView.Controls)
                {
                    DrawControl(control, root, fillersDict);
                }
            }
            result.Fillers = fillers;
            result.ServiceFiller = serviceFiller;
            return result;
        }
        private static void DrawControl(ViewControl vc, FrameworkElement currentParent, Dictionary<Guid, IFillerBase> fillers)
        {
            FrameworkElement element = null;
            switch (vc.Type)
            {
                case ControlType.FieldFiller:
                    AddChild(currentParent, (FrameworkElement)fillers[vc.Field]);
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
                    element = new TabControl() { Style = ResourceInstance.FindStyle("TabControlMain") };                    
                    break;
                case ControlType.TabItem:
                    element = new TabItem() { Style = ResourceInstance.FindStyle("TabItemMain"), Header = vc.Name };
                    break;
                case ControlType.Group:
                    element = new GroupBox() { Header = vc.Name };
                    break;
            } 
            AddChild(currentParent, element);
            if (vc.Children is not null)
            {
                foreach (ViewControl control in vc.Children)
                {
                    DrawControl(control, element, fillers);
                }
            }
        }
        private static void AddChild(FrameworkElement container, FrameworkElement child)
        {
            //if (container is TabControl tab)
            //{
            //    TabItem ti = new();
            //    ti.Content = child;
            //    tab.Items.Add(ti);
            //}
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
