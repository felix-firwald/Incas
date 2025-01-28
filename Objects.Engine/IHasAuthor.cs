using System;

namespace Incas.Objects.Engine
{
    public interface IHasAuthor
    {
        public Guid AuthorId { get; set; }
    }
}
