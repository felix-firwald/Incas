using Incas.Core.ViewModels;
using Incas.Users.Models;
using Incubator_2.Common;
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
        public string Username
        {
            get
            {
                return this._session.user;
            }
        }
        public string StartTime
        {
            get
            {
                return this._session.timeStarted.ToString("G");
            }
        }
        public string EndTime
        {
            get
            {
                if (this.IsActive)
                {
                    return "В сети";
                }
                else
                {
                    return this._session.timeFinished.ToString("G");
                }
            }
        }
        public bool IsActive
        {
            get
            {
                return this._session.active;
            }
        }
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
