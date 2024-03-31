using System;


namespace Common
{
    public enum PermissionGroup
    {
        Admin = 0,  // администрирование программой и базой данных, включая выдачу ролей (видит админку)
        Moderator = 1,  // модератор может все, что может администратор, кроме управления пользователями и рабочим пространством (видит админку)
        Editor = 2, // редактор наделен большими правами, чем оператор (например,
        Operator = 3,   // обычный сотрудник
    }
    public enum PermissionMode
    {
        Monopoly,   // монопольный режим - только эта группа имеет право
        Rising,     // восходящий режим - эта группа и те, что ВЫШЕ, имеют право
        Cascade,    // каскадный режим - эта группа и те, что НИЖЕ, имеют право
    }
    static class Permission
    {

        public static PermissionGroup CurrentUserPermission = PermissionGroup.Operator;

        public static void SetPermissionGroup(PermissionGroup group)
        {
            CurrentUserPermission = group;
        }

        private static bool InMonopolyAccess(PermissionGroup required)
        {
            return required == CurrentUserPermission;  // true, если пользователь член этой группы прав
        }
        private static bool InCascadeAccess(PermissionGroup required)
        {
            // поскольку енум идет наоборот (админ = 0), то сравнение идет наоборот
            return CurrentUserPermission >= required;    // true, если пользователь член этой группы прав или ниже
        }
        private static bool InRisingAccess(PermissionGroup required)
        {
            // поскольку енум идет наоборот (админ = 0), то сравнение идет наоборот
            return CurrentUserPermission <= required;    // true, если пользователь член этой группы прав или ниже
        }

        public static bool IsUserHavePermission(
            PermissionGroup requiredGroup,
            PermissionMode mode = PermissionMode.Rising
        )
        {
            switch (mode)
            {
                case PermissionMode.Rising: return InRisingAccess(requiredGroup);
                case PermissionMode.Monopoly: return InMonopolyAccess(requiredGroup);
                case PermissionMode.Cascade: return InCascadeAccess(requiredGroup);
                default: throw (new Exception($"Unknown type of permission mode: {mode} got."));
            }
        }

    }
}
