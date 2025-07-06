using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Interfaces
{
    public interface IObjectEditorForm
    {
        public delegate void UpdateRequested();
        public event UpdateRequested OnUpdateRequested;
    }
}
