using Incas.Core.Classes;
using Incas.Objects.Components;
using Incas.Objects.Documents.Components;
using Incas.Objects.Models;
using Incas.Objects.Processes.Components;
using Incas.Objects.StaticModels.Components;
using System;
using System.Collections.Generic;
using System.Data;

namespace Incas.Objects.Engine
{
    public static class Helpers
    {
        #region Tables
        /// <summary>
        /// Base table in .objinc, all types of classes must have this table
        /// </summary>
        public const string MainTable = "OBJECTS_MAP";

        /// <summary>
        /// Table in .objinc storing all edits of objects, typical only for documents
        /// </summary>
        public const string EditsTable = "EDITS_MAP";

        /// <summary>
        /// Table in .objinc storing user comments for documents storing in <see cref="MainTable"/>, typical only for documents
        /// </summary>
        public const string CommentsTable = "COMMENTS_MAP";

        /// <summary>
        /// Table in .objinc storing presets definitions
        /// </summary>
        public const string PresetsTable = "PRESETS_MAP";
        #endregion
        #region Fields
        /// <summary>
        /// Service guid for <see cref="MainTable"/>, 
        /// <see cref="EditsTable"/>, 
        /// <see cref="AttachmentsTable"/>, 
        /// <see cref="CommentsTable"/>, all objects of all types of classes must have it
        /// </summary>
        public const string IdField = "SERVICE_ID";

        /// <summary>
        /// Service name for <see cref="MainTable"/>, all objects of all types of classes must have it
        /// </summary>
        public const string NameField = "OBJECT_NAME";

        /// <summary>
        /// Date created for <see cref="MainTable"/>, 
        /// <see cref="EditsTable"/>, 
        /// <see cref="AttachmentsTable"/>, 
        /// <see cref="CommentsTable"/>, typical only for docs
        /// </summary>
        public const string DateCreatedField = "CREATED_DATE";

        /// <summary>
        /// Date created for <see cref="MainTable", only for process/>, 
        /// </summary>
        public const string DateOpenedField = "OPEN_DATE";

        /// <summary>
        /// Date created for <see cref="MainTable", only for process/>, 
        /// </summary>
        public const string DateClosedField = "CLOSE_DATE";

        /// <summary>
        /// Date created for <see cref="MainTable", only for process/>, 
        /// </summary>
        public const string ContributorsField = "CONTRIBUTORS";

        /// <summary>
        /// The user (guid) who created the object, typical for focs and models, but not for generators
        /// </summary>
        public const string AuthorField = "AUTHOR_ID";

        /// <summary>
        /// Custom status metafield (stored in bytes), for <see cref="MainTable"/>, default value is 0
        /// </summary>
        public const string StatusField = "STATUS_ID";

        /// <summary>
        /// Now is useless
        /// </summary>
        public const string MetaField = "META_INFORMATION";

        /// <summary>
        /// Stores the Preset <see cref="Guid" (if preset in use, else <see cref="Guid.Empty")
        /// </summary>
        public const string PresetField = "PRESET_ID";

        /// <summary>
        /// Date when the document is marked by user as <see cref="TerminatedField"/>
        /// </summary>
        public const string DateTerminatedField = "TERMINATED_DATE";

        /// <summary>
        /// Service status of document, default values is 0 (means document is still in working progress), 1 means document is marked by user as completed
        /// </summary>
        public const string TerminatedField = "TERMINATED";

        /// <summary>
        /// A link to source document that the edit or generator belongs to, for <see cref="EditsTable"/>
        /// </summary>
        public const string TargetObjectField = "TARGET_OBJECT_ID";

        /// <summary>
        /// A reference to source class of document that the generator belongs to, for <see cref="MainTable"/>
        /// </summary>
        public const string TargetClassField = "TARGET_CLASS_ID";

        /// <summary>
        /// Type of comment, for <see cref="CommentsTable"/>
        /// </summary>
        public const string TypeField = "TYPE";

        /// <summary>
        /// Data, for <see cref="EditsTable"/> and <see cref="CommentsTable"/>
        /// </summary>
        public const string DataField = "DATA";

        /// <summary>
        /// Password, for <see cref="MainTable"/> (Users service class)/>
        /// </summary>
        public const string PasswordField = "PASSWORD";

        /// <summary>
        /// Group, for <see cref="MainTable"/> (Users service class)/>
        /// </summary>
        public const string GroupField = "GROUP";
        #endregion

        #region Helper Methods
        #region Append
        public static void AppendAuthorServiceField(IHasAuthor obj, Dictionary<string, string> pairs)
        {
            pairs.Add(Helpers.AuthorField, obj.AuthorId.ToString());
        }
        public static void AppendCreationDateServiceField(IHasCreationDate obj, Dictionary<string, string> pairs)
        {
            pairs.Add(Helpers.DateCreatedField, obj.CreationDate.ToString());
        }
        public static void AppendPresetServiceField(IHasPreset obj, Dictionary<string, string> pairs)
        {
            pairs.Add(Helpers.PresetField, obj.Preset.ToString());
        }
        public static void AppendTerminableServiceFields(ITerminable obj, Dictionary<string, string> pairs)
        {
            pairs.Add(Helpers.TerminatedField, obj.Terminated ? "1" : "0");
            pairs.Add(Helpers.DateTerminatedField, obj.TerminatedDate.ToString());
        }
        #endregion
        #region Parse
        public static void ParseAuthorServiceField(IHasAuthor obj, DataRow dr)
        {
            obj.AuthorId = Guid.Parse(dr[Helpers.AuthorField].ToString());
        }
        public static void ParseCreationDateServiceField(IHasCreationDate obj, DataRow dr)
        {
            DateTime cd;
            DateTime.TryParse(dr[Helpers.DateCreatedField].ToString(), out cd);
            obj.CreationDate = cd;
        }
        public static void ParsePresetServiceField(IHasPreset obj, DataRow dr)
        {
            Guid guid = new();
            if (Guid.TryParse(dr[Helpers.PresetField].ToString(), out guid))
            {
                obj.Preset = guid;
            }
        }
        public static void ParseTerminableServiceFields(ITerminable obj, DataRow dr)
        {
            obj.Terminated = dr[Helpers.TerminatedField].ToString() == "1";
            DateTime dt;
            DateTime.TryParse(dr[Helpers.DateTerminatedField].ToString(), out dt);
            obj.TerminatedDate = dt;
        }
        #endregion
        #endregion

        public static bool CheckAuthor(IObject obj)
        {
            IHasAuthor objWithAuthor = obj as IHasAuthor;
            if (objWithAuthor != null)
            {
                return objWithAuthor.AuthorId == ProgramState.CurrentUser.id;
            }
            return true;
        }
        public static bool IsObjectTerminated(IObject obj)
        {
            ITerminable terminable = obj as ITerminable;
            if (terminable != null)
            {
                return terminable.Terminated;
            }
            return false;
        }
        public static IObject CreateObjectByType(ClassData data)
        {
            IObject obj;
            switch (data.ClassType)
            {
                case ClassType.Model:
                default:
                    obj = new Components.Object();
                    break;
                case ClassType.Document:
                    obj = new Document();
                    break;
                case ClassType.Process:
                    obj = new Process();
                    break;
                case ClassType.StaticModel:
                    obj = new StaticObject();
                    break;
                case ClassType.ServiceClassGroup:
                    obj = new ServiceClasses.Groups.Components.Group();
                    break;
                case ClassType.ServiceClassUser:
                    obj = new ServiceClasses.Users.Components.User();
                    break;
            }
            return obj;
        }
    }
}
