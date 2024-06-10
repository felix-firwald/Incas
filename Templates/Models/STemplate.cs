using Incas.Templates.Components;

namespace Incas.Templates.Models
{
    public struct STemplate
    {
        public int id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string suggestedPath { get; set; }
        public string parent { get; set; }
        public TemplateType type { get; set; }
        public string settings { get; set; }

        public Template AsModel()
        {
            Template template = new()
            {
                id = this.id,
                name = this.name,
                path = this.path,
                suggestedPath = this.suggestedPath,
                parent = this.parent,
                type = this.type,
                settings = this.settings
            };
            return template;
        }
    }
}
