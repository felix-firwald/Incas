using Incas.Objects.Components;
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
        public List<Components.Object> GetObjects();
        public void SetObjects(List<Components.Object> objs);
        public void ApplyObjectsBy(Class cl, Guid obj);
    }
}
