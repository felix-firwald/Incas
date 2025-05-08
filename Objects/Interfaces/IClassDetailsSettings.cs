using Incas.Admin.ViewModels;
using Incas.Objects.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Interfaces
{
    public interface IClassDetailsSettings
    {
        public string ItemName { get; }
        public void SetUpContext(IMembersContainerViewModel vm);
    }
}
