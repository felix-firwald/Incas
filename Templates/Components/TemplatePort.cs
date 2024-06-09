using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using Incas.Templates.Models;
using Newtonsoft.Json;
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
            public List<Tag> Tags { get; set; }
        }

        public TemplatePort()
        {
            this.Data = new();
        }

        public void FillData(Template temp, bool applyParent)
        {
            this.Data.SourceTemplate = temp;
            using (Tag tag = new())
            {
                this.Data.Tags = applyParent
                    ? tag.GetAllTagsByTemplate(this.Data.SourceTemplate.id, this.Data.SourceTemplate.parent)
                    : tag.GetAllTagsByTemplate(this.Data.SourceTemplate.id);
            }
            foreach (Tag tag in this.Data.Tags)
            {
                tag.id = 0;
            }
            this.Data.SourceTemplate.id = 0;
            this.Data.SourceTemplate.parent = "";
            //switch (this.Data.SourceTemplate.type)
            //{
            //    case TemplateType.Word:
            //        this.Source = ProgramState.GetFullnameOfWordFile(this.Data.SourceTemplate.path);
            //        break;
            //    case TemplateType.Excel:
            //        this.Source = ProgramState.GetFullnameOfExcelFile(this.Data.SourceTemplate.path);
            //        break;
            //}
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
                // Read the content of the entry without extracting the entire file
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
                if (entry.Name.EndsWith(".docx"))
                {
                    if (File.Exists(ProgramState.GetFullnameOfWordFile(entry.Name))) // если файл с таким именем есть в папке Word
                    {
                        if (DialogsManager.ShowQuestionDialog($"Исходный файл шаблона с именем \"{entry.Name}\" уже существует в рабочем пространстве.\n" +
                            $"Использовать его или переименовать предлагаемый файл из импортированного шаблона?", "Использовать старый шаблон?", "Использовать старый", "Переименовать предлагаемый") == DialogStatus.No)
                        {
                            newpath = DialogsManager.ShowInputBox("Имя исходного файла", "Придумайте другое имя").Replace("\\", "").Replace(".docx", "") + ".docx"; // use new
                            this.Data.SourceTemplate.path = newpath;
                            entry.ExtractToFile(ProgramState.GetFullnameOfWordFile(newpath));
                        }
                    }
                    else // если файла с таким именем нет в папке Word
                    {
                        entry.ExtractToFile(ProgramState.GetFullnameOfWordFile(entry.Name));
                    }
                }
                else // если это Excel
                {
                    if (File.Exists(ProgramState.GetFullnameOfExcelFile(entry.Name))) // если файл с таким именем есть в папке Excel
                    {
                        if (DialogsManager.ShowQuestionDialog($"Исходный файл шаблона с именем \"{entry.Name}\" уже существует в рабочем пространстве.\n" +
                            $"Использовать его или переименовать предлагаемый файл из импортированного шаблона?", "Использовать старый шаблон?", "Использовать старый", "Переименовать предлагаемый") == DialogStatus.No)
                        {
                            newpath = DialogsManager.ShowInputBox("Имя исходного файла", "Придумайте другое имя").Replace("\\", "").Replace(".docx", "") + ".docx"; // use new
                            this.Data.SourceTemplate.path = newpath;
                            entry.ExtractToFile(ProgramState.GetFullnameOfExcelFile(newpath));
                        }
                    }
                    else // если файла с таким именем нет в папке Excel
                    {
                        entry.ExtractToFile(ProgramState.GetFullnameOfExcelFile(entry.Name));
                    }
                }
            }
        }
    }
}
