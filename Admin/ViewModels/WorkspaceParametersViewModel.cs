using Incas.Core.Classes;
using Incas.Core.Models;
using Incas.Core.ViewModels;
using Incas.Users.Models;

namespace Incas.Admin.ViewModels
{
    public class WorkspaceParametersViewModel : BaseViewModel
    {
        private Parameter _workspaceName = new Parameter().GetParameter(ParameterType.INCUBATOR, "ws_name");
        private Parameter _workspaceOpened = new Parameter().GetParameter(ParameterType.INCUBATOR, "ws_opened");
        private Parameter _workspaceLocked = new Parameter().GetParameter(ParameterType.INCUBATOR, "ws_locked");
        private bool _terminateSessions = false;
        public WorkspaceParametersViewModel()
        {

        }

        public string WorkspaceName
        {
            get => this._workspaceName.value;
            set
            {
                this._workspaceName.value = value;
                this.OnPropertyChanged(nameof(this.WorkspaceName));
            }
        }
        public bool WorkspaceOpened
        {
            get => this._workspaceOpened.GetValueAsBool();
            set
            {
                this._workspaceOpened.WriteBoolValue(value);
                this.OnPropertyChanged(nameof(this.WorkspaceOpened));
            }
        }
        public bool WorkspaceLocked
        {
            get => this._workspaceLocked.GetValueAsBool();
            set
            {
                this._workspaceLocked.WriteBoolValue(value);
                this.OnPropertyChanged(nameof(this.WorkspaceLocked));
            }
        }
        public bool TerminateSessions
        {
            get => this._terminateSessions;
            set
            {
                this._terminateSessions = value;
                this.OnPropertyChanged(nameof(this.TerminateSessions));
            }
        }
        public void SaveParameters()
        {
            this._workspaceName.UpdateValue();
            this._workspaceOpened.UpdateValue();
            this._workspaceLocked.UpdateValue();
            if (this._workspaceLocked.GetValueAsBool() && this.TerminateSessions)
            {
                using (Session session = new Session())
                {
                    session.GetOpenedSessions().ForEach(session =>
                    {
                        ServerProcessor.SendTerminateProcess(session.slug);
                    });
                }
            }
        }
    }
}
