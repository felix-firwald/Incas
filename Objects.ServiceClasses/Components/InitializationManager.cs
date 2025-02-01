using Incas.Core.Classes;
using Incas.Objects.Components;
using Incas.Objects.Engine;
using Incas.Objects.ServiceClasses.Groups.Components;
using Incas.Objects.ServiceClasses.Models;
using Incas.Objects.ServiceClasses.Users.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Incas.Objects.ServiceClasses.Components
{
    public static class InitializationManager
    {
        public async static void RunInitialization(WorkspacePrimarySettings wps, string password)
        {
            wps.ServiceGroups = InitializeGroupClass();
            wps.ServiceUsers = InitializeUserClass();
            Guid groupId = await InitializeFirstGroup(wps.ServiceGroups);
            InitializeFirstUser(wps.ServiceUsers, groupId, password);
        }
        private static ServiceClass InitializeGroupClass()
        {
            ServiceClass group = new();
            group.Id = Guid.NewGuid();
            group.Name = "Группа";
            group.Type = ClassType.ServiceClassGroup;
            group.Data = new();
            group.Data.ShowCard = true;
            group.Data.Fields = new();
            group.Data.Fields.Add(new() { Id = Guid.NewGuid(), Type = FieldType.Text, Name = "Описание", VisibleName = "Описание", Description = "Общая характеристика предоставляемых полномочий" });
            group.Data.Fields.Add(new() { Id = Guid.NewGuid(), Type = FieldType.Text, Name = "Отдел", VisibleName = "Наименование отдела", Description = "Если группа обобщает отдел" });
            Processor.InitializeObjectMap(group);
            return group;
        }
        private static ServiceClass InitializeUserClass()
        {
            ServiceClass user = new();
            user.Id = Guid.NewGuid();
            user.Name = "Пользователь";
            user.Type = ClassType.ServiceClassUser;
            user.Data = new();
            user.Data.ShowCard = true;
            user.Data.Fields = new();
            user.Data.Fields.Add(new() { Id = Guid.NewGuid(), Type = FieldType.Variable, Name = "Фамилия", VisibleName = "Фамилия", Description = "Фамилия сотрудника (обязательно)", NotNull = true });
            user.Data.Fields.Add(new() { Id = Guid.NewGuid(), Type = FieldType.Variable, Name = "Имя", VisibleName = "Имя", Description = "Имя сотрудника (обязательно)", NotNull = true });
            user.Data.Fields.Add(new() { Id = Guid.NewGuid(), Type = FieldType.Variable, Name = "Отчество", VisibleName = "Отчество", Description = "Отчество сотрудника (при наличии)" });
            user.Data.Fields.Add(new() { Id = Guid.NewGuid(), Type = FieldType.Variable, Name = "Инициалы", VisibleName = "Инициалы", Description = "Например, Иванов А. А." });
            user.Data.Fields.Add(new() { Id = Guid.NewGuid(), Type = FieldType.Variable, Name = "Телефон", VisibleName = "Номер телефона", Description = "В формате: X (XXX) XXX XX XX" });
            user.Data.Fields.Add(new() { Id = Guid.NewGuid(), Type = FieldType.Variable, Name = "Email", VisibleName = "Email", Description = "Адрес почты сотрудника" });
            user.Data.Fields.Add(new() { Id = Guid.NewGuid(), Type = FieldType.Variable, Name = "Должность", VisibleName = "Должность", Description = "Официальное наименование" });            
            Processor.InitializeObjectMap(user);
            return user;
        }
        private async static Task<Guid> InitializeFirstGroup(ServiceClass groupClass)
        {
            List<IObject> groups = new();
            #region Group Admin
            Group groupAdmin = new(groupClass)
            {
                Name = "Администраторы",
                Data = new()
                {
                    GeneralSettingsEditing = true,
                    CreatingClasses = true,
                    UpdatingClasses = true,
                    RemovingClasses = true,
                    Indestructible = true
                },
                Fields = new()
            };
            groupAdmin.Data.DefaultPermissionType = GroupPermissionType.Allowed;
            groupAdmin.Data.UserClassPermissions = new();
            groupAdmin.Data.UserClassPermissions.SetAll(GroupPermissionType.Allowed);
            groupAdmin.Data.GroupClassPermissions = new();
            groupAdmin.Data.GroupClassPermissions.SetAll(GroupPermissionType.Allowed);
            groupAdmin.Fields.Add(new() { ClassField = groupClass.Data.Fields[0], Value = "Пользователи этой группы обладают полным доступом ко всем функциям рабочего пространства" });
            groups.Add(groupAdmin);
            #endregion
            groups.Add(GenerateModeratorGroup(groupClass));
            groups.Add(GenerateOperatorGroup(groupClass));
            bool result = await Processor.WriteObjects(groupClass, groups);
            if (result)
            {
                return groupAdmin.Id;
            }
            return Guid.Empty;
        }

        private static Group GenerateModeratorGroup(ServiceClass groupClass)
        {
            Group groupModerator = new(groupClass)
            {
                Name = "Модераторы",
                Data = new()
                {
                    GeneralSettingsEditing = false,
                    CreatingClasses = false,
                    UpdatingClasses = true,
                    RemovingClasses = false
                },
                Fields = new()
            };
            groupModerator.Data.DefaultPermissionType = GroupPermissionType.Allowed;
            groupModerator.Data.UserClassPermissions = new();
            groupModerator.Data.UserClassPermissions.SetOnlyReadViewAllowed();
            groupModerator.Data.GroupClassPermissions = new();
            groupModerator.Data.GroupClassPermissions.SetOnlyReadViewAllowed();
            groupModerator.Fields.Add(new() { ClassField = groupClass.Data.Fields[0], Value = "Пользователи этой группы обладают повышенными правами доступа к рабочему пространству" });
            return groupModerator;
        }
        private static Group GenerateOperatorGroup(ServiceClass groupClass)
        {
            Group groupOperator = new(groupClass)
            {
                Name = "Операторы",
                Data = new()
                {
                    GeneralSettingsEditing = false,
                    CreatingClasses = false,
                    UpdatingClasses = false,
                    RemovingClasses = false
                },
                Fields = new()
            };
            groupOperator.Data.DefaultPermissionType = GroupPermissionType.Restricted;
            groupOperator.Data.UserClassPermissions = new();
            groupOperator.Data.UserClassPermissions.SetAll(GroupPermissionType.Restricted);
            groupOperator.Data.GroupClassPermissions = new();
            groupOperator.Data.GroupClassPermissions.SetAll(GroupPermissionType.Restricted);
            groupOperator.Fields.Add(new() { ClassField = groupClass.Data.Fields[0], Value = "Пользователи этой группы обладают стандартными правами доступа" });
            return groupOperator;
        }

        private static void InitializeFirstUser(ServiceClass userClass, Guid group, string password)
        {
            User user = new(userClass)
            {
                Group = group,
                Name = "Администратор",
                Data = new() { Password = password, Indestructible = true },
                Fields = new()
            };
            Processor.WriteObjects(userClass, user);
        }
    }
}
