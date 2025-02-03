using System.Data;

namespace Incas.Objects.Interfaces
{
    public interface ITableFiller : IFillerBase
    {
        public DataTable GetValue();
    }
}
