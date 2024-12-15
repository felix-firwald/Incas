using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using Incas.Users.Models;
using System;
using System.ComponentModel;

namespace Incas.Users.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для GroupSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class GroupSettings : AutoUIBase
    {
        #region Data
        private Group group;

        [Description("Имя группы")]
        public string Name { get; set; }

        [Description("Разрешено все, что не запрещено")]
        public bool IsSuper { get; set; }
        #endregion

        public GroupSettings(Group group)
        {
            this.group = group;
            if (group.Id != Guid.Empty)
            {
                this.Name = group.Name;
                GroupData gs = group.GetData();
                this.IsSuper = gs.IsSuper;
            }
        }

        #region Functionality
        public override void Load()
        {
            
        }

        public override void Validate()
        {

        }

        public override void Save()
        {
            this.group.Name = this.Name;
            GroupData data = new()
            {
                IsSuper = this.IsSuper
            };
            this.group.SetData(data);
        }
        #endregion
    }
}
