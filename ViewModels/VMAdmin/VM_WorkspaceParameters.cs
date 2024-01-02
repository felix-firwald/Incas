using Incubator_2.Common;
using Incubator_2.Models;
using Models;


namespace Incubator_2.ViewModels.VMAdmin
{
    public class VM_WorkspaceParameters : VM_Base
    {
        private Parameter _workspaceName = new Parameter().GetParameter(ParameterType.INCUBATOR, "ws_name");
        private Parameter _workspaceOpened = new Parameter().GetParameter(ParameterType.INCUBATOR, "ws_opened");
        private Parameter _workspaceLocked = new Parameter().GetParameter(ParameterType.INCUBATOR, "ws_locked");
        private bool _terminateSessions = false;
        public VM_WorkspaceParameters()
        {
            
        }

        public string WorkspaceName
        {
            get
            {
                return _workspaceName.value;
            }
            set
            {
                _workspaceName.value = value;
                OnPropertyChanged(nameof(WorkspaceName));
            }
        }
        public bool WorkspaceOpened
        {
            get
            {
                return _workspaceOpened.GetValueAsBool();
            }
            set
            {
                _workspaceOpened.WriteBoolValue(value);
                OnPropertyChanged(nameof(WorkspaceOpened));
            }
        }
        public bool WorkspaceLocked
        {
            get
            {
                return _workspaceLocked.GetValueAsBool();
            }
            set
            {
                _workspaceLocked.WriteBoolValue(value);
                OnPropertyChanged(nameof(WorkspaceLocked));
            }
        }
        public bool TerminateSessions
        {
            get
            {
                return _terminateSessions;
            }
            set
            {
                _terminateSessions = value;
                OnPropertyChanged(nameof(TerminateSessions));
            }
        }
        public void SaveParameters()
        {
            _workspaceName.UpdateValue();
            _workspaceOpened.UpdateValue();
            _workspaceLocked.UpdateValue();
            if (_workspaceLocked.GetValueAsBool() && TerminateSessions)
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
