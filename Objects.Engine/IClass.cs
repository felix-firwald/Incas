using Incas.Objects.Components;
using Incas.Objects.Models;
using System;

namespace Incas.Objects.Engine
{
    public interface IClass
    {
        public Guid Id { get; set; }
        public ClassType Type { get; set; }
        public string Name { get; set; }
        public ClassData GetClassData();
    }
}
