using Incas.Core.ViewModels;
using IncasEngine.Workspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Admin.ViewModels
{
    /// <summary>
    /// Класс ViewModel.
    /// Метод Save вызывается после применения изменений на форме и подтверждает сохранение изменений из ViewModel в Model.
    /// </summary>
    public class CommandViewModel : BaseViewModel
    {
        /// <summary>
        /// Model
        /// </summary>
        public WorkspaceMenuCommand Source { get; set; }

        public CommandViewModel(WorkspaceMenuCommand source)
        {
            this.Source = source;
        }
        public string Name
        {
            get
            {
                return this.Source.Name;
            }
            set
            {
                this.Source.Name = value;
                this.OnPropertyChanged(nameof(this.Name));
            }
        }
        public string SelectedIconPath
        {
            get
            {
                return this.Source.Icon;
            }
            set
            {
                this.Source.Icon = value;
                this.OnPropertyChanged(nameof(this.SelectedIconPath));
            }
        }
        public string Description
        {
            get
            {
                return this.Source.Description;
            }
            set
            {
                this.Source.Description = value;
                this.OnPropertyChanged(nameof(this.Description));
            }
        }
        public IncasEngine.Core.Color Color
        {
            get
            {
                return this.Source.Color;
            }
            set
            {
                this.Source.Color = value;
                this.OnPropertyChanged(nameof(this.Color));
            }
        }
        public byte ColorR
        {
            get
            {
                return this.Source.Color.R;
            }
            set
            {
                IncasEngine.Core.Color color = IncasEngine.Core.Color.FromRGB(value, this.Source.Color.G, this.Source.Color.B);
                this.OnPropertyChanged(nameof(this.ColorR));
                this.Color = color;
            }
        }
        public byte ColorG
        {
            get
            {
                return this.Source.Color.G;
            }
            set
            {
                IncasEngine.Core.Color color = IncasEngine.Core.Color.FromRGB(this.Source.Color.R, value, this.Source.Color.B);
                this.OnPropertyChanged(nameof(this.ColorG));
                this.Color = color;
            }
        }
        public byte ColorB
        {
            get
            {
                return this.Source.Color.B;
            }
            set
            {
                IncasEngine.Core.Color color = IncasEngine.Core.Color.FromRGB(this.Source.Color.R, this.Source.Color.G, value);
                this.OnPropertyChanged(nameof(this.ColorB));
                this.Color = color;
            }
        }
        /// <summary>
        /// Сохраняет изменения в Model
        /// </summary>

    }
}
