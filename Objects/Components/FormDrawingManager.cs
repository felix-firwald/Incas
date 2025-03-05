using Incas.DialogSimpleForm.Components;
using Incas.Objects.Interfaces;
using Incas.Objects.Views.Controls;
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
        //private static List<IFillerBase> DrawControls(IObject obj)
        //{
        //    List<IFillerBase> fillers = new();
        //    IServiceFieldFiller serviceFiller = Components.ServiceExtensionFieldsManager.GetFillerByType(obj);
        //    if (serviceFiller != null)
        //    {
        //        root.Children.Add((UserControl)serviceFiller);
        //    }
        //    foreach (Field f in obj.Class.GetClassData().Fields)
        //    {
        //        switch (f.Type)
        //        {
        //            default:
        //                FieldFiller ff = new(f)
        //                {
        //                    Uid = f.Id.ToString()
        //                };
        //                //ff.OnInsert += this.Tf_OnInsert;
        //                //ff.OnFillerUpdate += this.Tf_OnFieldUpdate;
        //                //ff.OnScriptRequested += this.Ff_OnScriptRequested;
        //                //ff.OnDatabaseObjectCopyRequested += this.Tf_OnDatabaseObjectCopyRequested;
        //                root.Children.Add(ff);
        //                fillers.Add(ff);
        //                break;
        //            case FieldType.Table:
        //                FieldTableFiller ft = new(f)
        //                {
        //                    Uid = f.Id.ToString()
        //                };
        //                //ft.OnInsert += this.Tf_OnInsert;
        //                //ft.OnFillerUpdate += this.Tf_OnFieldUpdate;
        //                //ft.OnDatabaseObjectCopyRequested += this.Tf_OnDatabaseObjectCopyRequested;
        //                root.Children.Add(ft);
        //                fillers.Add(ft);
        //                break;

        //        }
        //    }
        //    return fillers;
        //}
        public static DrawingOutputArgs DrawForm(IObject obj, StackPanel root)
        {
            DrawingOutputArgs result = new();
            root.Children.Clear();
            IClassData data = obj.Class.GetClassData();
            Dictionary<Guid, IFillerBase> fillersDict = new();
            List<IFillerBase> fillers = new();
            IServiceFieldFiller serviceFiller = Components.ServiceExtensionFieldsManager.GetFillerByType(obj);
            if (serviceFiller != null)
            {
                root.Children.Add((UserControl)serviceFiller);
            }
            foreach (Field f in obj.Class.GetClassData().Fields)
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
            if (data.EditorView is null || data.EditorView.Controls is null)
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
