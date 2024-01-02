using Incubator_2.Common;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.ViewModels.VMAdmin
{
    class VM_SessionElement : VM_Base
    {
        Session _session;
        public VM_SessionElement(Session s)
        {
            _session = s;
        }
        public string Username
        {
            get
            {
                return _session.user;
            }
        }
        public string StartTime
        {
            get
            {
                return _session.timeStarted.ToString("G");
            }
        }
        public string EndTime
        {
            get
            {
                if (IsActive)
                {
                    return "В сети";
                }
                else
                {
                    return _session.timeFinished.ToString("G");
                }
                
            }
        }
        public bool IsActive
        {
            get
            {
                return _session.active;
            }
        }
        public void Terminate()
        {
            ServerProcessor.SendTerminateProcess(_session.slug);
        }
        public void Restart()
        {
            ServerProcessor.SendRestartProcess(_session.slug);
        }
    }
}
