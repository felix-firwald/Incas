using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using Incas.Objects.AutoUI;
using Incas.Templates.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Incas.Templates.Components
{
    public class TemplatePort
    {
        private const string dataName = "data.json";
        public TemplateData Data;
        public class TemplateData
        {
            public Template SourceTemplate { get; set; }
            public List<Objects.Models.Field> Tags { get; set; }
        }

        public TemplatePort()
        {
            this.Data = new();
        }

        public void FillData(Template temp, bool applyParent)
        {
            this.Data.SourceTemplate = temp;
            this.Data.Tags = applyParent
                ? this.Data.SourceTemplate.GetFields(false)
                : this.Data.SourceTemplate.GetFields(true);
            foreach (Objects.Models.Field tag in this.Data.Tags)
            {
                tag.Id = Guid.Empty;
            }
            this.Data.SourceTemplate.id = Guid.Empty;
            this.Data.SourceTemplate.parent = "";
        }
        public void ToFile(string folder, string sourceFullname, string sourceFilename)
        {
            string zipFolder = $"{folder}\\{this.Data.SourceTemplate.name}";
            Directory.CreateDirectory(zipFolder);
            File.WriteAllText($"{zipFolder}\\{dataName}", JsonConvert.SerializeObject(this.Data));
            File.Copy(sourceFullname, $"{zipFolder}\\{sourceFilename}");
            ZipFile.CreateFromDirectory(zipFolder, zipFolder + ".tinc");
            Directory.Delete(zipFolder, true);
        }
        public void ParseData(string file)
        {
            using ZipArchive zip = ZipFile.Open(file, ZipArchiveMode.Read);
            ZipArchiveEntry entry = zip.GetEntry(dataName);
            if (entry != null)
            {
                using Stream entryStream = entry.Open();
                using StreamReader reader = new(entryStream);
                this.Data = JsonConvert.DeserializeObject<TemplateData>(reader.ReadToEnd());
            }
        }
        public void GetSourceFile(string file, string sourceName)
        {
            string newpath = "";
            using ZipArchive zip = ZipFile.Open(file, ZipArchiveMode.Read);
            ZipArchiveEntry entry = zip.GetEntry(sourceName);
            if (entry != null)
            {
                if (File.Exists(ProgramState.GetFullnameOfDocumentFile(entry.Name))) // если файл с таким именем есть в папке Sources
                {
                    if (DialogsManager.ShowQuestionDialog($"Исходный файл шаблона с именем \"{entry.Name}\" уже существует в рабочем пространстве.\n" +
                        $"Использовать его или переименовать предлагаемый файл из импортированного шаблона?", "Использовать старый шаблон?", "Использовать старый", "Переименовать предлагаемый") == DialogStatus.No)
                    {
                        RenameSourceFile rs = new(entry.Name);
                        if (rs.ShowDialog("Новое имя", Icon.Subscript))
                        {
                            newpath = rs.Name;
                            this.Data.SourceTemplate.path = newpath;
                            entry.ExtractToFile(ProgramState.GetFullnameOfDocumentFile(newpath));
                        }                      
                    }
                }
                else // если файла с таким именем нет в папке Sources
                {
                    entry.ExtractToFile(ProgramState.GetFullnameOfDocumentFile(entry.Name));
                }
            }
        }
    }
}
