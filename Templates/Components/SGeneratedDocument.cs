using Incas.Core.Classes;
using System;
using System.Collections.Generic;

namespace Incas.CreatedDocuments.Models
{
    public struct GeneratedElement
    {
        public Guid template;
        public string filler;
        public List<SGeneratedTag> filledTags;      
    }
    public struct SGeneratedDocument
    {
        public Guid id;
        public Guid template;
        public DateTime generatedTime;
        public string fileName;
        public List<SGeneratedTag> filledTags;
    }
}
