using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Views.Controls
{
    public interface IFiller
    {
        /// <summary>
        /// Delegate for all actions 
        /// </summary>
        /// <param name="filler"></param>
        public delegate void FillerUpdate(IFiller filler);

        /// <summary>
        /// For the inserting
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="text"></param>
        public delegate void StringAction(Guid tag, string text);

        /// <summary>
        /// Calling every time the controls contained within it are updated
        /// </summary>
        public event FillerUpdate OnFillerUpdate;

        /// <summary>
        /// Calling when a filler requests a copy for itself from the custom database (objects map)
        /// </summary>
        public event FillerUpdate OnDatabaseObjectCopyRequested;

        /// <summary>
        /// Calling when a filler requests a copying its value to other objects
        /// </summary>
        public event StringAction OnInsert;

        /// <summary>
        /// Source field from class
        /// </summary>
        public Objects.Models.Field Field { get; set; }

        /// <summary>
        /// Get an internal value
        /// </summary>
        /// <returns></returns>
        public string GetData();

        /// <summary>
        /// Set a value
        /// </summary>
        /// <returns></returns>
        public void SetValue(string value);

        /// <summary>
        /// Marks filler in red color 
        /// </summary>
        public void MarkAsNotValidated();

        /// <summary>
        /// Marks filler in null
        /// </summary>
        public void MarkAsValidated();
    }
}
