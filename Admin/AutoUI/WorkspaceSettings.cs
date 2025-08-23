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

        [Description("Тип рабочего пространства")]
        public Selector WorkspaceType { get; private set; }

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
                { IncasEngine.Workspace.Enums.WorkspaceMode.DesktopUsual, "Десктоп-клиент (SQLite)" },
                { IncasEngine.Workspace.Enums.WorkspaceMode.DesktopWithPostgres, "Десктоп-клиент (PostgreSQL)" },
                { IncasEngine.Workspace.Enums.WorkspaceMode.DesktopWithMySQL, "Десктоп-клиент (MySQL)" },
                { IncasEngine.Workspace.Enums.WorkspaceMode.DesktopWithTSQL, "Десктоп-клиент (T-SQL)" },
                { IncasEngine.Workspace.Enums.WorkspaceMode.Server, "Клиент-сервер (IncasServer + SQLite)" },
                { IncasEngine.Workspace.Enums.WorkspaceMode.ServerWithPostgres, "Клиент-сервер (IncasServer + PostgreSQL)" },
                { IncasEngine.Workspace.Enums.WorkspaceMode.ServerWithMySQL, "Клиент-сервер (IncasServer + MySQL)" },
                { IncasEngine.Workspace.Enums.WorkspaceMode.ServerWithTSQL, "Клиент-сервер (IncasServer + TSQL)" }
            });
            this.WorkspaceMode.SetSelection(this.data.Mode);

            this.WorkspaceType = new(new()
            {
                { IncasEngine.Workspace.Enums.WorkspaceType.Dynamic, "DWS (динамическое)" },
                { IncasEngine.Workspace.Enums.WorkspaceType.Static, "CWS (статическое)" },
                { IncasEngine.Workspace.Enums.WorkspaceType.Project, "WSP (проект)" },
            });
            this.WorkspaceType.SetSelection(this.data.Type);
            this.FixationMode = new(new()
            {
                { IncasEngine.Workspace.Enums.FixationMode.Instant, "Мгновенная фиксация" },
                { IncasEngine.Workspace.Enums.FixationMode.Manual, "Ручная фиксация" },
            });
            this.FixationMode.SetSelection(this.data.FixationType);
        }
        public override void Save()
        {
            this.data.Mode = (IncasEngine.Workspace.Enums.WorkspaceMode)this.WorkspaceMode.SelectedObject;
            this.data.FixationType = (IncasEngine.Workspace.Enums.FixationMode)this.FixationMode.SelectedObject;
            ProgramState.CurrentWorkspace.UpdateDefinition(this.data);
        }
    }
}
