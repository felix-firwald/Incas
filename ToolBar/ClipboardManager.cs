using Common;
using Incubator_2.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.ToolBar
{
    public struct ClipboardElement
    {
        public string name;
        public string text;
        public string category;
    }
    public static class ClipboardManager
    {
        private static string target { get { return $"{ProgramState.Root}\\clipboard.jinc"; } }
        private static List<ClipboardElement> elements = new();
        public static List<ClipboardElement> GetElements()
        {
            if (File.Exists(target))
            {
                elements = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ClipboardElement>>(File.ReadAllText(target));
                if (elements == null )
                {
                    elements = new();
                }
                return elements;
            }
            else
            {
                return new List<ClipboardElement>();
            }
        }
        public static void AddElement(ClipboardElement element)
        {
            GetElements();
            elements.Add(element);
            Save();
        }
        public static void RemoveElement(ClipboardElement element)
        {
            GetElements();
            elements.Remove(element);
            Save();
        }
        public static List<ClipboardElement> GetElementsByCategory(string category)
        {
            GetElements();
            List<ClipboardElement> result = new();
            foreach (ClipboardElement element in elements)
            {
                if (element.category == category)
                {
                    result.Add(element);
                }
            }
            return result;
        }
        private static void Save()
        {
            File.WriteAllText(target, Newtonsoft.Json.JsonConvert.SerializeObject(elements));
        }
    }
}
