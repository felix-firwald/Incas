using IncasEngine.ObjectiveEngine.Models;
using System;

namespace Incas.Objects.Interfaces
{
    public interface IFillerBase
    {
        /// <summary>
        /// Delegate for all actions 
        /// </summary>
        /// <param name="filler"></param>
        public delegate void FillerUpdate(IFillerBase filler);
        public delegate void AutoRunMethod(Guid method);

        /// <summary>
        /// For the inserting
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="text"></param>
        public delegate void StringAction(Guid tag, string text);

        /// <summary>
        /// Calling every time the controls contained within it are updated
        /// </summary>
        public event AutoRunMethod OnFillerUpdate;

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
        public Field Field { get; set; }

        /// <summary>
        /// Get an internal value
        /// </summary>
        /// <returns></returns>
        public string GetData();

        /// <summary>
        /// Get an internal value
        /// </summary>
        /// <returns></returns>
        public object GetDataForScript();

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

        /// <summary>
        /// Applies state of filler by State of object
        /// </summary>
        public void ApplyState(State state);
    }
}
