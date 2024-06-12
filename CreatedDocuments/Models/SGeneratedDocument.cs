using Incas.CreatedDocuments.Components;
using IncasEngine;
using System;
using System.Collections.Generic;

namespace Incas.CreatedDocuments.Models
{
    public struct GeneratedElement
    {
        public int template;
        public string filler;
        public List<SGeneratedTag> filledTags;      
    }
    public struct SGeneratedDocument
    {
        public int id;
        public int template;
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

        public GeneratedDocument AsModel()
        {
            GeneratedDocument d = new()
            {
                id = this.id,
                template = this.template,
                templateName = this.templateName,
                generatedTime = this.generatedTime,
                fileName = this.fileName,
                number = this.number,
                fullNumber = this.fullNumber,
                status = this.status,
                content = this.filledTagsString,
                author = this.author
            };
            return d;
        }
    }
}
