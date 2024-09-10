using Incas.Core.Classes;
using Incas.Core.Models;
using Incas.Core.ViewModels;
using Incas.Users.Models;
using Mono.Unix;
using System.Data;
using System.Collections.Generic;

namespace Incas.Admin.ViewModels
{
    public class WorkspaceParametersViewModel : BaseViewModel
    {
        private Parameter _workspaceName = new Parameter().GetParameter(ParameterType.WORKSPACE, "ws_name");
        private Parameter _workspaceOpened = new Parameter().GetParameter(ParameterType.WORKSPACE, "ws_opened");
        private Parameter _workspaceLocked = new Parameter().GetParameter(ParameterType.WORKSPACE, "ws_locked");
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
        public DataTable Constants
        {
            get
            {
                using (Parameter p = new())
                {
                    return p.GetConstants();
                }
            }
        }
        public DataTable Enumerations
        {
            get
            {
                using (Parameter p = new())
                {
                    return p.GetEnumerators();
                }
            }
        }
        private DataRow selectedConstant;
        public DataRow SelectedConstant
        {
            get
            {
                return this.selectedConstant;
            }
            set
            {
                this.selectedConstant = value;
                this.OnPropertyChanged(nameof(this.SelectedConstant));
            }
        }
        private string selectedEnumeration;
        public string SelectedEnumeration
        {
            get
            {
                return this.selectedEnumeration;
            }
            set
            {
                this.selectedEnumeration = value;
                this.OnPropertyChanged(nameof(this.SelectedEnumeration));
                this.OnPropertyChanged(nameof(this.SelectedEnumValues));
            }
        }

        public string SelectedEnumValues
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.SelectedEnumeration))
                {
                    return "";
                }
                List<string> list = ProgramState.GetEnumeration(this.SelectedEnumeration);
                string result = "";
                int counter = 1;
                foreach (string value in list)
                {
                    result += $"({counter}) {value}\n";
                    counter++;
                }
                //DialogsManager.ShowInfoDialog(result);
                return result;
            }
        }
        public void UpdateConstants()
        {
            this.OnPropertyChanged(nameof(this.Constants));
        }
        public void UpdateEnumerations()
        {
            this.OnPropertyChanged(nameof(this.Enumerations));
        }
    }
}
