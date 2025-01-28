using Incas.Objects.Models;
using Incas.Objects.ServiceClasses.Components;
using System;

namespace Incas.Objects.ServiceClasses.Models
{
    public class ServiceClass
    {
        public Guid Id { get; set; }
        public ServiceClassType ClassType { get; set; }
        public ClassData Data { get; set; }
    }
}
