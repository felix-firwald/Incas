using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using Incas.Scripting.Interfaces;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace Incas.Scripting.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для ScriptHelperCreateObject.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class ScriptHelperCreateObject : StaticAutoUIBase, IScriptHelper
    {
        protected override string FinishButtonText { get => "Вставить код"; }
        #region Data
        [Description("Класс объекта")]
        public Selector TargetClass { get; set; }

        [Description("Название переменной")]
        public string VariableName { get; set; }
        #endregion

        public ScriptHelperCreateObject()
        {
            Dictionary<object, string> elements = new();
            using (Class cl = new())
            {
                foreach (ClassItem ci in cl.GetAllClassItems())
                {
                    elements[ci] = ci.Name;
                }
            }
            this.TargetClass = new(elements);
            this.VariableName = "new_object";
        }

        #region Functionality
        public override void Load()
        {

        }

        public override void Validate()
        {

        }

        public override void Save()
        {

        }

        public string GetScript()
        {
            IClass cl = new Class((ClassItem)this.TargetClass.SelectedObject);
            string variable = this.VariableName.Replace(' ', '_');
            string result = $"{variable} = incas.create_object('{cl.InternalName}')";
            foreach (Field f in cl.GetClassData().Fields)
            {
                switch (f.Type)
                {
                    case IncasEngine.ObjectiveEngine.Classes.FieldType.String:
                    case IncasEngine.ObjectiveEngine.Classes.FieldType.Text:
                    case IncasEngine.ObjectiveEngine.Classes.FieldType.LocalEnumeration:
                    case IncasEngine.ObjectiveEngine.Classes.FieldType.GlobalEnumeration:
                        result += $"\n{variable}.{f.Name} = \"\"";
                        break;
                    case IncasEngine.ObjectiveEngine.Classes.FieldType.Boolean:
                        result += $"\n{variable}.{f.Name} = False";
                        break;
                    case IncasEngine.ObjectiveEngine.Classes.FieldType.Integer:
                    case IncasEngine.ObjectiveEngine.Classes.FieldType.Float:
                        result += $"\n{variable}.{f.Name} = 0";
                        break;
                    default:
                        result += $"\n{variable}.{f.Name} = \"\"";
                        break;
                }                
            }
            result += $"\nincas.write_objects({variable})";
            return result;
        }
        #endregion
    }
}
