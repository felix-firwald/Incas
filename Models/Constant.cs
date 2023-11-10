using Common;
using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace Models
{
    public class Constant : Model
    {
        public string name { get; set; }
        public string content { get; set; }
        public string category { get; set; }
        public bool hidden { get; set; }
        public Constant() 
        {
            tableName = "Constants";
            definition = "name STRING UNIQUE NOT NULL,\n" +
                "content STRING NOT NULL,\n" +
                "category STRING,\n" +
                "hidden BOOLEAN NOT NULL DEFAULT (False)";
        }
        public List<Constant> GetAllConstants()
        {
            List<Constant> list = new List<Constant>();
            DataTable dt = StartCommand()
                                .Select()
                                .WhereNotEqual("hidden", "True")
                                .Execute();
            foreach (DataRow dr in dt.Rows)
            {
                Constant mc = new Constant();
                mc.Serialize(dr);
                list.Add(mc);
            }
            return list;
        }
        public void GetConstantByName()
        {
            ;
            DataRow dr = GetOne(
                    StartCommand()
                        .Select()
                        .WhereEqual("name", name)
                        .Execute()
            );
            if (dr != null)
            {
                this.Serialize(dr);
            }
            else
            {
                MessageBox.Show($"Константа с именем \"{name}\" не найдена!");
            }
        }
    }
}
