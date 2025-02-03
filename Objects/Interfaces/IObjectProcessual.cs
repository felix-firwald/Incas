using IncasEngine.ObjectiveEngine.Types.Processes.Components;
using System;
using System.Collections.Generic;

namespace Incas.Objects.Interfaces
{
    public interface IObjectProcessual
    {
        public DateTime OpenDate { get; set; }
        public DateTime CloseDate { get; set; }
        public ProcessData Data { get; set; }
        public List<Guid> Contributors { get; set; }
    }
}
