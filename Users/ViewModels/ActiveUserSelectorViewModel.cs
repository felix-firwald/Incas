using Incas.Core.Classes;
using Incas.Core.ViewModels;
using Incas.Users.Models;
using System.Collections.Generic;

namespace Incas.Users.ViewModels
{
    public class ActiveUserSelectorViewModel : BaseViewModel
    {
        private Session selectedSession;
        private string helptext = "Выбранному пользователю будет послан процесс от вашего имени.";
        public ActiveUserSelectorViewModel()
        {

        }
        public string HelpTextTitle
        {
            get => this.helptext;
            set
            {
                this.helptext = value;
                this.OnPropertyChanged(nameof(this.HelpTextTitle));
            }
        }
        public List<Session> Sessions
        {
            get => ProgramState.GetActiveSessions();
            set => this.OnPropertyChanged(nameof(this.Sessions));
        }

        public Session SelectedSession
        {
            get => this.selectedSession;
            set
            {
                this.selectedSession = value;
                this.OnPropertyChanged(nameof(this.SelectedSession));
            }
        }

    }
}
