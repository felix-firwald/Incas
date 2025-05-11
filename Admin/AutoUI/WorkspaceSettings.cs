using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using IncasEngine.Workspace;
using System;
using System.ComponentModel;

namespace Incas.Admin.AutoUI
{
    public class WorkspaceSettings : StaticAutoUIBase
    {
        private WorkspaceDefinition data;
        [Description("Имя рабочего пространства")]
        public string Name
        {
            get => this.data.Name;
            set => this.data.Name = value;
        }
        [Description("Дата создания рабочего пространства")]
        public string Created
        {
            get
            {
                return this.data.Created.ToString("G");
            }
        }
        [Description("Версия рабочего пространства")]
        public string Version
        {
            get
            {
                if (string.IsNullOrEmpty(this.data.Version))
                {
                    return "1.0.0.0";
                }
                return this.data.Version;
            }
        }

        [Description("Версия IncasEngine")]
        public string FrameworkVersion
        {
            get
            {
                if (string.IsNullOrEmpty(this.data.FrameworkVersion))
                {
                    return "-";
                }
                return this.data.FrameworkVersion;
            }
        }

        [Description("Режим работы")]
        public Selector WorkspaceMode { get; set; }

        [Description("Режим фиксации изменений")]
        public Selector FixationMode { get; set; }

        [Description("Требовать пароль при существенных изменениях")]
        public bool RequirePassword
        {
            get => this.data.SignificantChangesRequirePassword;
            set => this.data.SignificantChangesRequirePassword = value;
        }

        public override void Load()
        {
            this.data = ProgramState.CurrentWorkspace.GetDefinition(true);
            this.WorkspaceMode = new(new()
            {
                { WorkspaceDefinition.WorkspaceMode.DesktopUsual, "Десктоп-клиент (SQLite)" },
                { WorkspaceDefinition.WorkspaceMode.DesktopWithPostgres, "Десктоп-клиент (PostgreSQL)" },
                { WorkspaceDefinition.WorkspaceMode.Server, "Клиент-сервер (IncasServer + SQLite)" },
                { WorkspaceDefinition.WorkspaceMode.ServerWithPostgres, "Клиент-сервер (IncasServer + PostgreSQL)" }
            });
            this.WorkspaceMode.SetSelection(this.data.Mode);
            this.FixationMode = new(new()
            {
                { WorkspaceDefinition.FixationMode.Instant, "Мгновенная фиксация" },
                { WorkspaceDefinition.FixationMode.Manual, "Ручная фиксация" },
            });
            this.FixationMode.SetSelection(this.data.FixationType);
        }
        public override void Save()
        {
            this.data.Mode = (WorkspaceDefinition.WorkspaceMode)this.WorkspaceMode.SelectedObject;
            this.data.FixationType = (WorkspaceDefinition.FixationMode)this.FixationMode.SelectedObject;
            ProgramState.CurrentWorkspace.UpdateDefinition(this.data);
        }
    }
}
