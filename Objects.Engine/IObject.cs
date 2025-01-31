using Incas.Objects.Components;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Incas.Objects.Engine
{
    public interface IObject
    {
        /// <summary>
        /// Class of object
        /// </summary>
        public IClass Class { get; set; }

        /// <summary>
        /// Unique identifier of object stored in <see cref="Helpers.IdField"/> column
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// A name of object stored in <see cref="Helpers.NameField"/> column
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// User-defined fields
        /// <para>Each field is stored in one column of the database</para>
        /// </summary>
        public List<FieldData> Fields { get; set; }

        public async Task<string> GetFieldValue(Guid id)
        {
            string result = "";
            await Task.Run(() =>
            {
                foreach (FieldData field in this.Fields)
                {
                    if (field.ClassField.Id == id)
                    {
                        result = field.Value;
                        break;
                    }
                }
            });
            return result;
        }

        /// <summary>
        /// Engine calls this method if <see cref="Id"/> is <see cref="Guid.Empty"/>
        /// <para>Attention:</para>
        /// <para>Do not set <see cref="Id"/> in this method, <see cref="Processor"/> itself will do this</para>
        /// </summary>
        public void Initialize();

        /// <summary>
        /// Incas calls this method when user wants to copy the object
        /// </summary>
        /// <returns></returns>
        public IObject Copy();

        /// <summary>
        /// Engine calls this method to get the all service fields of object for inserting or updating
        /// <para>1. Please, do not unwrap there fields stored in <see cref="Fields"/>, <see cref="Processor"/> itself will do this</para>
        /// <para>2. Please, do not place to this dictionary <see cref="Id"/> and <see cref="Name"/>, <see cref="Processor"/> itself will do this</para>
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> AddServiceFields(Dictionary<string, string> pairs);

        /// <summary>
        /// Engine calls this method when it gets object from the database
        /// <para>1. Please, do not parse fields stored in <see cref="Fields"/>, <see cref="Processor"/> itself will do this</para>
        /// <para>2. Please, do not parse <see cref="Id"/> and <see cref="Name"/>, <see cref="Processor"/> itself will do this</para>
        /// </summary>
        /// <returns></returns>
        public void ParseServiceFields(DataRow dr);
    }
}
