using Incas.Common;
using Incas.Core.Classes;
using System.Collections.Generic;
using System.Data;

namespace Incas.Core.Models
{
    public class Sector : Model
    {
        public string slug { get; set; }
        public string name { get; set; }

        public Sector()
        {
            this.tableName = "Sectors";
        }
        public void AddSector(bool generateSlug = true, bool acumulate = false)
        {
            if (generateSlug)
            {
                this.slug = ProgramState.GenerateSlug(12);
            }
            Query q = this.StartCommandToService()
                .Insert(new()
                {
                    {
                        "slug", this.slug
                    },
                    {
                        "name", this.name
                    }
                }, true);
            q.ExecuteVoid();
            DatabaseManager.InitializeData(this.slug);
        }
        public void SaveSector()
        {
            if (string.IsNullOrEmpty(this.slug))
            {
                this.AddSector();
            }
            else
            {
                this.StartCommandToService()
                    .Update("name", this.name)
                    .WhereEqual("slug", this.slug)
                    .ExecuteVoid();
            }
        }
        public void RemoveSector()
        {
            this.StartCommandToService()
                .Delete()
                .WhereEqual("slug", this.slug)
                .ExecuteVoid();
        }
        public List<Sector> GetSectors()
        {
            DataTable dt = this.StartCommandToService()
                .Select()
                .Execute();
            List<Sector> result = [];
            foreach (DataRow dr in dt.Rows)
            {
                Sector sector = new();
                sector.Serialize(dr);
                result.Add(sector);
            }
            return result;
        }
        public List<string> GetSectorSlugs()
        {
            DataTable dt = this.StartCommandToService()
                .Select()
                .Execute();
            List<string> result = [];
            foreach (DataRow dr in dt.Rows)
            {
                this.Serialize(dr);
                result.Add(this.slug);
            }
            return result;
        }
        public Sector GetSector()
        {
            DataRow dr = this.StartCommandToService()
                .Select()
                .WhereEqual("slug", this.slug)
                .ExecuteOne();
            this.Serialize(dr);
            return this;
        }
    }
}
