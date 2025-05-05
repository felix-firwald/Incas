using Incas.Core.ViewModels;
using IncasEngine.Scripting;
using IncasEngine.Workspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Incas.Core.ViewModels
{
    /// <summary>
    /// Класс ViewModel.
    /// Метод Save вызывается после применения изменений на форме и подтверждает сохранение изменений из ViewModel в Model.
    /// </summary>
    public class WorkspaceMenuCommandViewModel : BaseViewModel
    {
        /// <summary>
        /// Model
        /// </summary>
        public WorkspaceMenuCommand Source { get; set; }

        public WorkspaceMenuCommandViewModel(WorkspaceMenuCommand source)
        {
            this.Source = source;
            this.UserCommand = new Command(this.DoUserCommand);
        }

        private void DoUserCommand(object obj)
        {
            ObjectScriptManager.RunStaticCode(this.Source.Script);
        }

        public ICommand UserCommand { get; set; }
        public string Name
        {
            get
            {
                return this.Source.Name;
            }
        }

        public string Icon
        {
            get
            {
                return this.Source.Icon;
            }
        }

        public IncasEngine.Core.Color Color
        {
            get
            {
                return this.Source.Color;
            }
        }
        public string Description
        {
            get
            {
                return this.Source.Description;
            }
        }
    }
}
