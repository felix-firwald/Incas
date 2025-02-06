using Incas.Core.Classes;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Incas.Rendering.Components
{
    public static class ObjectParser
    {
        public static IObject ParseString(string inputString, string patternString, IClass @class)
        {
            IObject obj = Helpers.CreateObjectByType(@class);
            obj.Fields = new();
            // Экранируем спецсимволы регулярных выражений в строке-шаблоне, кроме тегов
            // экранируем только те символы, которые *не* являются частью тега
            string regexPattern = Regex.Replace(patternString, @"([^\w\s\[\]])", m => "\\" + m.Groups[1].Value);
            ClassData data = @class.GetClassData();
            // Заменяем теги на группы захвата в регулярном выражении
            List<Field> fields = new();
            foreach (Field f in data.Fields)
            {
                if (patternString.Contains("[" + f.Name + "]"))
                {
                    fields.Add(f);
                }
            }
            for (int i = 0; i < fields.Count; i++)
            {
                string tagRegex = "([\\s\\S]*?)";
                regexPattern = regexPattern.Replace($"[{fields[i].Name}]", $"(?<{fields[i].Name}>{tagRegex})?");
            }
            // Используем регулярное выражение для извлечения значений
            Match match = Regex.Match(inputString, regexPattern);

            if (match.Success)
            {
                foreach (Field f in fields)
                {                   
                    //result[fields[i].Name] = match.Groups[i + 1].Value.Trim(); // +1, потому что Group[0] - это весь матч
                    if (match.Groups[f.Name].Success)  // Проверяем, что группа с таким именем была найдена
                    {
                        //result[f.Name] = match.Groups[f.Name].Value.Trim();
                        FieldData item = new()
                        {
                            ClassField = f,
                            Value = match.Groups[f.Name].Value.Trim()
                        };
                        obj.Fields.Add(item);
                    }
                }
            }
            return obj;
        }
    }
}
