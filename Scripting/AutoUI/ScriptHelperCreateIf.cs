using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using Incas.Objects.Interfaces;
using Incas.Objects.ViewModels;
using Incas.Scripting.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace Incas.Scripting.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для ScriptHelperCreateIf.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class ScriptHelperCreateIf : StaticAutoUIBase, IScriptHelper
    {
        protected override string FinishButtonText { get => "Вставить код"; }
        #region Data
        private IMembersContainerViewModel container { get; set; }

        [Description("Целевое поле для проверки")]
        public Selector TargetField { get; set; }

        [Description("Добавить инструкцию else")]
        public bool AddElse { get; set; }
        #endregion

        public ScriptHelperCreateIf(IMembersContainerViewModel vm)
        {
            this.container = vm;
            Dictionary<object, string> fields = new();
            foreach (FieldViewModel field in vm.Fields)
            {
                fields[field] = field.VisibleName;
            }
            this.TargetField = new(fields);
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
            string result = "";
            FieldViewModel field = (FieldViewModel)this.TargetField.SelectedObject;
            switch (field.Type)
            {
                case IncasEngine.ObjectiveEngine.Classes.FieldType.String:
                case IncasEngine.ObjectiveEngine.Classes.FieldType.Text:
                    if (this.AddElse)
                    {
                        result = $"if this.{field.Name} == \"\":\n\tpass\nelse:\n\tpass";
                    }
                    else
                    {
                        result = $"if this.{field.Name} == \"\":\n\tpass";
                    }
                    break;
                case IncasEngine.ObjectiveEngine.Classes.FieldType.Boolean:
                    if (this.AddElse)
                    {
                        result = $"if this.{field.Name} == True:\n\tpass\nelse:\n\tpass";
                    }
                    else
                    {
                        result = $"if this.{field.Name} == True:\n\tpass";
                    }
                    break;
                case IncasEngine.ObjectiveEngine.Classes.FieldType.LocalEnumeration:
                    bool first = true;
                    foreach (string variant in field.Source.GetLocalEnumeration())
                    {
                        if (first)
                        {
                            result = $"if this.{field.Name} == \"{variant}\":\n\tpass";
                            first = false;
                        }
                        else
                        {
                            result += $"\nelif this.{field.Name} == \"{variant}\":\n\tpass";
                        }
                    }
                    if (this.AddElse)
                    {
                        result += "\nelse:\n\tpass";
                    }
                    break;
                case IncasEngine.ObjectiveEngine.Classes.FieldType.GlobalEnumeration:
                    bool first2 = true;
                    List<string> variants = ProgramState.GetEnumeration(field.Source.GetGlobalEnumeration().TargetId);
                    foreach (string variant in variants)
                    {
                        if (first2)
                        {
                            result = $"if this.{field.Name} == \"{variant}\":\n\tpass";
                            first2 = false;
                        }
                        else
                        {
                            result += $"\nelif this.{field.Name} == \"{variant}\":\n\tpass";
                        }
                    }
                    if (this.AddElse)
                    {
                        result += "\nelse:\n\tpass";
                    }
                    break;
                case IncasEngine.ObjectiveEngine.Classes.FieldType.Integer:
                case IncasEngine.ObjectiveEngine.Classes.FieldType.Float:
                    result = $"if this.{field.Name} == {field.Source.GetNumberFieldData()?.MinValue}:\n\tpass";
                    if (this.AddElse)
                    {
                        result += "\nelse:\n\tpass";
                    }
                    break;
                case IncasEngine.ObjectiveEngine.Classes.FieldType.Object:
                    result = $"if this.{field.Name} is not None: # если объект установлен\n\tpass";
                    if (this.AddElse)
                    {
                        result += "\nelse # если объект НЕ установлен:\n\tpass";
                    }
                    break;
            }
            return result;
        }
        #endregion
    }
}
