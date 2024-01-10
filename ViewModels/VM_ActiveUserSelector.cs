using Common;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.ViewModels
{
    public class VM_ActiveUserSelector : VM_Base
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
                return helptext;
            }
            set
            {
                helptext = value;
                OnPropertyChanged(nameof(HelpTextTitle));
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
                OnPropertyChanged(nameof(Sessions));
                
            }
        
        }

        public Session SelectedSession 
        {
            get { return selectedSession; } 
            set
            {
                selectedSession = value;
                OnPropertyChanged(nameof(SelectedSession));
            }
        }

    }
}
