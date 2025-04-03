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
    public class WorkspaceComponentButtonViewModel : BaseViewModel
    {

        /// <summary>
        /// For user buttons
        /// </summary>
        public WorkspaceComponentButtonViewModel(WorkspaceComponent source)
        {
            this.name = source.Name;
            this.description = source.Description;
            this.icon = source.Icon;
            this.color = source.Color;
        }

        /// <summary>
        /// For service buttons
        /// </summary>
        public WorkspaceComponentButtonViewModel(string name, string description, string icon, IncasEngine.Core.Color color)
        {
            this.name = name;
            this.description = description;
            this.icon = icon;
            this.color = color;
        }

        private string name { get; set; }
        public string Name
        {
            get
            {
                return this.name;
            }
        }
        private string description { get; set; }
        public string Description
        {
            get
            {
                return this.description;
            }
        }
        private string icon { get; set; }
        public string Icon
        {
            get
            {
                return this.icon;
            }
        }
        private IncasEngine.Core.Color color { get; set; }
        public IncasEngine.Core.Color Color
        {
            get
            {
                return this.color;
            }
        }
    }
}
