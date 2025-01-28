using System;

namespace Incas.Objects.Processes.Components
{
    public struct ProcessReview
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
        public ReviewResult Result { get; set; }
        public string Comment { get; set; }
    }
}
