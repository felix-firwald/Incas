using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Incas.Objects.Interfaces
{
    public interface IClassMemberViewModel
    {
        public enum MemberType
        {
            Field,
            Method,
            Table
        }
        public Guid Id { get; }
        public string Name { get; set; }
        public MemberType ClassMemberType { get; }
        public bool BelongsThisClass { get; }
        public ICommand AssignToContainer { get; }
        public bool EditingEnabled { get; }
    }
}
