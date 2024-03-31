using Common;
using System.Collections.Generic;
using System.Data;

namespace Incubator_2.Models
{
    public class Sector : Model
    {
        public string slug { get; set; }
        public string name { get; set; }

        public Sector()
        {
            tableName = "Sectors";
        }
        public void AddSector(bool generateSlug = true, bool acumulate = false)
        {
            if (generateSlug)
            {
                slug = ProgramState.GenerateSlug(12);
            }
            Query q = StartCommandToService()
                .Insert(new()
                {
                    {
                        "slug", slug
                    },
                    {
                        "name", name
                    }
                }, true);
            q.ExecuteVoid();
            DatabaseManager.InitializeData(slug);
        }
        public void SaveSector()
        {
            if (string.IsNullOrEmpty(slug))
            {
                AddSector();
            }
            else
            {
                StartCommandToService()
                    .Update("name", name)
                    .WhereEqual("slug", slug)
                    .ExecuteVoid();
            }
        }
        public void RemoveSector()
        {
            StartCommandToService()
                .Delete()
                .WhereEqual("slug", slug)
                .ExecuteVoid();
        }
        public List<Sector> GetSectors()
        {
            DataTable dt = StartCommandToService()
                .Select()
                .Execute();
            List<Sector> result = new List<Sector>();
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
            DataTable dt = StartCommandToService()
                .Select()
                .Execute();
            List<string> result = new();
            foreach (DataRow dr in dt.Rows)
            {
                this.Serialize(dr);
                result.Add(this.slug);
            }
            return result;
        }
        public Sector GetSector()
        {
            DataRow dr = StartCommandToService()
                .Select()
                .WhereEqual("slug", slug)
                .ExecuteOne();
            this.Serialize(dr);
            return this;
        }
    }
}
