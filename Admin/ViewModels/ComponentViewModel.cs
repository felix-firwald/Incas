using Incas.Core.ViewModels;
using IncasEngine.Core;
using IncasEngine.Workspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Incas.Admin.ViewModels
{
    public class ComponentViewModel : BaseViewModel
    {
        public WorkspaceComponent Component { get; set; }
        public ComponentViewModel(WorkspaceComponent comp)
        {
            this.Component = comp;
        }
        public string Name
        {
            get
            {
                return this.Component.Name;
            }
            set
            {
                this.Component.Name = value;
                this.OnPropertyChanged(nameof(this.Name));
            }
        }
        public string SelectedIconPath
        {
            get
            {
                return this.Component.Icon;
            }
            set
            {
                this.Component.Icon = value;
                this.OnPropertyChanged(nameof(this.SelectedIconPath));
            }
        }
        public string Description
        {
            get
            {
                return this.Component.Description;
            }
            set
            {
                this.Component.Description = value;
                this.OnPropertyChanged(nameof(this.Description));
            }
        }
        public Color Color
        {
            get
            {
                return this.Component.Color;
            }
            set
            {
                this.Component.Color = value;
                this.OnPropertyChanged(nameof(this.Color));
            }
        }
        public byte ColorR
        {
            get
            {
                return this.Component.Color.R;
            }
            set
            {
                Color color = Color.FromRGB(value, this.Component.Color.G, this.Component.Color.B);
                this.OnPropertyChanged(nameof(this.ColorR));
                this.Color = color;
            }
        }
        public byte ColorG
        {
            get
            {
                return this.Component.Color.G;
            }
            set
            {
                Color color = Color.FromRGB(this.Component.Color.R, value, this.Component.Color.B);
                this.OnPropertyChanged(nameof(this.ColorG));
                this.Color = color;
            }
        }
        public byte ColorB
        {
            get
            {
                return this.Component.Color.B;
            }
            set
            {
                Color color = Color.FromRGB(this.Component.Color.R, this.Component.Color.G, value);
                this.OnPropertyChanged(nameof(this.ColorB));
                this.Color = color;
            }
        }
        public bool IsVisibleToEveryone
        {
            get
            {
                return this.Component.IsPublic;
            }
            set
            {
                this.Component.IsPublic = value;
                this.OnPropertyChanged(nameof(this.IsVisibleToEveryone));
            }
        }
        public bool IsActive
        {
            get
            {
                return this.Component.IsActive;
            }
            set
            {
                this.Component.IsActive = value;
                this.OnPropertyChanged(nameof(this.IsActive));
                this.OnPropertyChanged(nameof(this.NotActiveVisibility));
            }
        }
        public bool IsIsolated
        {
            get
            {
                return this.Component.IsIsolated;
            }
            set
            {
                this.Component.IsIsolated = value;
                this.OnPropertyChanged(nameof(this.IsIsolated));
            }
        }
        public bool StateEditingEnabled
        {
            get
            {
                return !this.Component.Indestructible;
            }
        }
        
        public Visibility WarnVisibility
        {
            get
            {
                return this.FromBool(this.Component.Indestructible);
            }
        }
        public Visibility NotActiveVisibility
        {
            get
            {
                return this.FromBool(!this.Component.IsActive);
            }
        }
    }
}
