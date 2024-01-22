using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.Models
{
    public struct SGeneratedTag
    {
        public int tag { get; set; }
        public string value { get; set; }
    }
    
    //public class GeneratedTag : Model
    //{
    //    public string document { get; set; }
    //    public int tag { get; set; }
    //    public string value { get; set; }
    //    public GeneratedTag()
    //    {
    //        tableName = "GeneratedTags";
    //    }
        
    //    public SGeneratedTag AsStruct()
    //    {
    //        SGeneratedTag result = new();
    //        result.id = id;
    //        result.document = document;
    //        result.tag = tag;
    //        result.value = value;
    //        return result;
    //    }

         

    //    public GeneratedTag AddGeneratedTags(List<SGeneratedTag> filledTags)
    //    {
    //        Query q = StartCommand();
    //        foreach(SGeneratedTag t in filledTags)
    //        {
    //            q.Insert(new()
    //            {
    //                {nameof(document), document },
    //                {nameof(tag), t.tag.ToString() },
    //                {nameof(value), t.value}
    //            });
    //            q.Accumulate();
    //        }
    //        //q.ExecuteVoid();
    //        return this;
    //    }
    //    public void DeleteGeneratedTags()
    //    {
    //        StartCommand()
    //            .Delete()
    //            .WhereEqual(nameof(document), document)
    //            .ExecuteVoid();
    //    }
    //    public List<SGeneratedTag> GetByDocument(string doc)
    //    {
    //        DataTable dt = StartCommand()
    //            .Select()
    //            .WhereEqual(nameof(document), doc)
    //            .Execute();
    //        List<SGeneratedTag> tags = new();
    //        foreach (DataRow dr in dt.Rows)
    //        {
    //            this.Serialize(dr);
    //            tags.Add(this.AsStruct());
    //        }
    //        return tags;
    //    }

    //}
}
