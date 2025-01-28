using System;

namespace Incas.Objects.Processes.Components
{
    public struct ProcessTask
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
        public bool Done { get; set; }
    }
}
