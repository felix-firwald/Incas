using Incas.Core.Classes;
using Incas.Objects.Documents.Views.Pages;
using Incas.Objects.Interfaces;
using Incas.Objects.Processes.Views.Pages;
using Incas.Objects.ServiceClasses.Groups.Views.Controls;
using Incas.Objects.ServiceClasses.Users.Views.Controls;
using Incas.Objects.ViewModels;
using Incas.Objects.Views.Controls;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Groups.Components;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Users.Components;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Incas.Objects.Components
{
    public static class ServiceExtensionFieldsManager
    {
        public static IClassPartSettings GetPartSettingsByType(ClassViewModel classViewModel)
        {
            switch (classViewModel.Type)
            {
                case IncasEngine.ObjectiveEngine.Classes.ClassType.Document:
                    return new DocumentClassPart().SetUp(classViewModel);
                case IncasEngine.ObjectiveEngine.Classes.ClassType.Process:
                    return new ProcessClassPart().SetUp(classViewModel);
            }
            return null;
        }
        public static IServiceFieldFiller GetFillerByType(IObject obj)
        {
            if (obj is Group objGroup)
            {
                return new GroupSettings().SetUp(objGroup);
            }
            else if (obj is User objUser)
            {
                return new UserSettings().SetUp(objUser);
            }
            return null;
        }
        public static void AppendServiceFieldViewers(IObject obj, StackPanel contentPanel)
        {
            #if !E_FREE
            if (obj is IncasEngine.ObjectiveEngine.Types.Objects.Object objUsual)
            {
                ObjectFieldViewer ofAuthor = new(ProgramState.CurrentWorkspace.GetDefinition().ServiceUsers, objUsual.AuthorId, "Автор");
                contentPanel.Children.Add(ofAuthor);
                return;
            }
            #endif
            if (obj is IncasEngine.ObjectiveEngine.Types.Documents.Document objDoc)
            {
                #if !E_FREE
                contentPanel.Children.Add(new ObjectFieldViewer(ProgramState.CurrentWorkspace.GetDefinition().ServiceUsers, objDoc.AuthorId, "Автор"));
                #endif
                contentPanel.Children.Add(new ObjectFieldViewer(objDoc.CreationDate, "Дата создания"));
                if (objDoc.Terminated)
                {
                    contentPanel.Children.Add(new ObjectFieldViewer("Процесс завершен", 152, 255, 0));
                    contentPanel.Children.Add(new ObjectFieldViewer(objDoc.TerminatedDate, "Дата завершения"));
                }
                return;
            }
            if (obj is IncasEngine.ObjectiveEngine.Types.Processes.Process objProc)
            {
                contentPanel.Children.Add(new ObjectFieldViewer(ProgramState.CurrentWorkspace.GetDefinition().ServiceUsers, objProc.AuthorId, "Инициатор"));
                contentPanel.Children.Add(new ObjectFieldViewer(objProc.CreationDate, "Дата инициализации"));
                if (objProc.OpenDate != DateTime.MinValue)
                {
                    contentPanel.Children.Add(new ObjectFieldViewer(objProc.OpenDate, "Дата открытия"));
                    if (objProc.Terminated)
                    {                       
                        contentPanel.Children.Add(new ObjectFieldViewer(objProc.TerminatedDate, "Дата завершения"));
                    }
                    else
                    {
                        contentPanel.Children.Add(new ObjectFieldViewer(objProc.CloseDate, "Открыт до"));
                    }
                }                          
                return;
            }
            if (obj is IncasEngine.ObjectiveEngine.Types.ServiceClasses.Users.Components.User objUser)
            {
                contentPanel.Children.Add(new ObjectFieldViewer(ProgramState.CurrentWorkspace.GetDefinition().ServiceGroups, objUser.Group, "Группа"));
                return;
            }
            if (obj is IncasEngine.ObjectiveEngine.Types.ServiceClasses.Groups.Components.Group objGroup)
            {
                string description = "<Описание отсутствует>";
                if (objGroup.Data.Indestructible)
                {
                    description = "Полный доступ к управлению (владельцы).";
                }
                else if (objGroup.Data.CanGroupViewingWorkspaceTab())
                {
                    description = "Частичный доступ к управлению.";
                }
                else
                {
                    description = "Стандартный доступ к управлению. Административных полномочий нет.";
                }
                contentPanel.Children.Add(new ObjectFieldViewer(description, 67, 70, 80));
                return;
            }
            if (obj is IncasEngine.ObjectiveEngine.Types.StaticModels.StaticObject objStatic)
            {
                contentPanel.Children.Add(new ObjectFieldViewer(objStatic.StartPeriod, "Начало действия"));
                contentPanel.Children.Add(new ObjectFieldViewer(objStatic.EndPeriod, "Конец действия"));
                return;
            }
        }
        public static void AppendServiceFieldViewers(IObjectEdition edit, StackPanel contentPanel)
        {
            ObjectFieldViewer ofAuthor = new(ProgramState.CurrentWorkspace.GetDefinition().ServiceUsers, edit.Editor, "Редактор");
            contentPanel.Children.Add(ofAuthor);
            contentPanel.Children.Add(new ObjectFieldViewer(edit.EditionDate, "Дата версии"));
            //if (obj is IncasEngine.ObjectiveEngine.Types.Documents.DocumentEdition objDoc)
            //{
            //    contentPanel.Children.Add(new ObjectFieldViewer(ProgramState.CurrentWorkspace.GetDefinition().ServiceUsers, objDoc.AuthorId, "Автор"));
                
            //    if (objDoc.Terminated)
            //    {
            //        contentPanel.Children.Add(new ObjectFieldViewer("Процесс завершен", 152, 255, 0));
            //        contentPanel.Children.Add(new ObjectFieldViewer(objDoc.TerminatedDate, "Дата завершения"));
            //    }
            //    return;
            //}
            //if (obj is IncasEngine.ObjectiveEngine.Types.Processes.ProcessEdition objProc)
            //{
            //    contentPanel.Children.Add(new ObjectFieldViewer(ProgramState.CurrentWorkspace.GetDefinition().ServiceUsers, objProc.AuthorId, "Инициатор"));
            //    contentPanel.Children.Add(new ObjectFieldViewer(objProc.CreationDate, "Дата инициализации"));
            //    if (objProc.OpenDate != DateTime.MinValue)
            //    {
            //        contentPanel.Children.Add(new ObjectFieldViewer(objProc.OpenDate, "Дата открытия"));
            //        if (objProc.Terminated)
            //        {
            //            contentPanel.Children.Add(new ObjectFieldViewer(objProc.TerminatedDate, "Дата завершения"));
            //        }
            //        else
            //        {
            //            contentPanel.Children.Add(new ObjectFieldViewer(objProc.CloseDate, "Открыт до"));
            //        }
            //    }
            //    return;
            //}
            //if (obj is IncasEngine.ObjectiveEngine.Types.ServiceClasses.Users.Components.UserEdition objUser)
            //{
            //    contentPanel.Children.Add(new ObjectFieldViewer(ProgramState.CurrentWorkspace.GetDefinition().ServiceGroups, objUser.Group, "Группа"));
            //    return;
            //}
            //if (obj is IncasEngine.ObjectiveEngine.Types.StaticModels.StaticObjectEdition objStatic)
            //{
            //    contentPanel.Children.Add(new ObjectFieldViewer(objStatic.StartPeriod, "Начало действия"));
            //    contentPanel.Children.Add(new ObjectFieldViewer(objStatic.EndPeriod, "Конец действия"));
            //    return;
            //}
        }
    }
}
