using Common;
using Incas.Core.ViewModels;
using Models;
using System.Collections.Generic;

namespace Incubator_2.ViewModels
{
    public class VM_ActiveUserSelector : BaseViewModel
    {
        private Session selectedSession;
        private string helptext = "Выбранному пользователю будет послан процесс от вашего имени.";
        public VM_ActiveUserSelector()
        {

        }
        public string HelpTextTitle
        {
            get
            {
                return this.helptext;
            }
            set
            {
                this.helptext = value;
                this.OnPropertyChanged(nameof(this.HelpTextTitle));
            }
        }
        public List<Session> Sessions
        {
            get
            {
                return ProgramState.GetActiveSessions();
            }
            set
            {
                this.OnPropertyChanged(nameof(this.Sessions));

            }

        }

        public Session SelectedSession
        {
            get { return this.selectedSession; }
            set
            {
                this.selectedSession = value;
                this.OnPropertyChanged(nameof(this.SelectedSession));
            }
        }

    }
}
