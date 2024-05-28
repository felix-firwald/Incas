using Incas.Core.Classes;
using Incas.Core.ViewModels;
using Incas.Users.Models;
using Incubator_2.Windows.AdminWindows;

namespace Incas.Users.ViewModels
{
    internal class SessionElementViewModel : BaseViewModel
    {
        private SSession _session;
        public SessionElementViewModel(ref SSession s)
        {
            this._session = s;
        }
        public string Username => this._session.user;
        public string StartTime => this._session.timeStarted.ToString("G");
        public string EndTime => this.IsActive ? "В сети" : this._session.timeFinished.ToString("G");
        public bool IsActive => this._session.active;
        public void Terminate()
        {
            ServerProcessor.SendTerminateProcess(this._session.slug);
        }
        public void Restart()
        {
            ServerProcessor.SendRestartProcess(this._session.slug);
        }
        public void ShowExplicit()
        {
            MakeExplicit d = new();
            if (d.ShowDialog() == true)
            {
                ServerProcessor.SendExplicitProcess(d.message, this._session.slug);
            }
        }
    }
}
