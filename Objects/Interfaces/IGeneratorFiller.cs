using IncasEngine.ObjectiveEngine.Interfaces;
using System;
using System.Collections.Generic;

namespace Incas.Objects.Interfaces
{
    interface IGeneratorFiller : IFillerBase
    {
        public List<IObject> GetObjects();
        public void SetObjects(List<IObject> objs);
        public void ApplyObjectsBy(IClass cl, Guid obj);
    }
}
