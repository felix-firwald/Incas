using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Engine
{
    public struct ObjectReference
    {
        public Guid Class { get; private set; }
        public Guid Object { get; private set; }
        public bool IsCorrect
        {
            get
            {
                return this.Class != Guid.Empty && this.Object != Guid.Empty;
            }
        }

        public ObjectReference(Guid Class, Guid Object)
        {
            this.Class = Class;
            this.Object = Object;
        }

        public override string ToString()
        {
            return $"{this.Class.ToString("N")}z{this.Object.ToString("N")}";
        }
        public static ObjectReference Parse(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                string[] collection = s.Split("z");
                (string classId, string objectId) = (collection[0], collection[1]);
                return new(Guid.ParseExact(classId, "N"), Guid.ParseExact(objectId, "N"));
            }
            return new();
        }
    }
}
