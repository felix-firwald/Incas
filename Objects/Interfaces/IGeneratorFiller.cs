using Incas.Objects.Engine;
using Incas.Objects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Interfaces
{
    interface IGeneratorFiller : IFillerBase
    {
        public List<IObject> GetObjects();
        public void SetObjects(List<IObject> objs);
        public void ApplyObjectsBy(Class cl, Guid obj);
    }
}
