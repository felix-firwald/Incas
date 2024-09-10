using Incas.CreatedDocuments.Components;
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
        public string templateName;
        public DateTime generatedTime;
        public string fileName;
        public string number;
        public string fullNumber;
        public DocumentStatus status { get; set; }
        public List<SGeneratedTag> filledTags;
        public string filledTagsString;
        public string author;
        public List<SGeneratedTag> GetFilledTags()
        {
            this.filledTags ??= Newtonsoft.Json.JsonConvert.DeserializeObject<List<SGeneratedTag>>(Cryptographer.DecryptString(this.filledTagsString));
            return this.filledTags;
        }
        public void SaveFilledTags(List<SGeneratedTag> tags)
        {
            this.filledTagsString = Cryptographer.EncryptString(Newtonsoft.Json.JsonConvert.SerializeObject(tags));
        }
    }
}
