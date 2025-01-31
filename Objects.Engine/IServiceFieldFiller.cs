using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Engine
{
    public interface IServiceFieldFiller
    {
        public IViewModel ViewModel { get; set; }
        public IObject TargetObject { get; set; }

        /// <summary>
        /// Engine calls this method when setting the ObjectCreator for editing operation by user
        /// <para>Please set View-Model here!</para>
        /// </summary>
        /// <param name="obj"></param>
        public IServiceFieldFiller SetUp(IObject obj);

        /// <summary>
        /// Engine calls this method when it needs to get the actual data to save the object
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public IObject GetResult();
    }
}
