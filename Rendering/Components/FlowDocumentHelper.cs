using DocumentFormat.OpenXml.Packaging;
using Incas.Core.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Incas.Rendering.Components
{
    public static class FlowDocumentHelper
    {
        public static FlowDocument LoadFlowDocumentFromDocx(string filePath)
        {
            FlowDocument flowDocument = new();
            try
            {
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(filePath, false))
                {
                    if (wordDocument.MainDocumentPart != null)
                    {
                        // Создаем XDocument из содержимого документа
                        System.Xml.Linq.XDocument document = wordDocument.MainDocumentPart.GetXDocument();

                        // Преобразуем XDocument в FlowDocument с использованием XamlReader
                        if (document != null)
                        {
                            string xamlText = ConvertWordprocessingDocumentToXaml(document);
                            if (!string.IsNullOrEmpty(xamlText))
                            {
                                try
                                {
                                    StringReader stringReader = new(xamlText);
                                    System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(stringReader);
                                    FlowDocument loadedFlowDocument = (FlowDocument)XamlReader.Load(xmlReader);

                                    // Копируем содержимое в основной FlowDocument, чтобы сохранить стили
                                    flowDocument.Blocks.Clear(); // Очищаем существующее содержимое
                                    foreach (Block block in loadedFlowDocument.Blocks)
                                    {
                                        flowDocument.Blocks.Add(block);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    DialogsManager.ShowErrorDialog(ex);
                                }
                            }
                        }
                    }
                    else
                    {
                        DialogsManager.ShowErrorDialog("Основная часть файла шаблона отсутствует.");
                    }
                }
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog($"Ошибка при загрузке .docx: {ex.Message}");
            }

            return flowDocument;
        }

        private static string ConvertWordprocessingDocumentToXaml(System.Xml.Linq.XDocument document)
        {
            string xaml = "<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" " +
                          "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">";

            foreach (System.Xml.Linq.XElement element in document.Root.Elements())
            {
                if (element.Name.LocalName == "p") // Paragraph
                {
                    xaml += "<Paragraph>";
                    foreach (System.Xml.Linq.XElement run in element.Elements())
                    {
                        if (run.Name.LocalName == "r") // Run
                        {
                            xaml += "<Run>";
                            foreach (System.Xml.Linq.XElement text in run.Elements())
                            {
                                if (text.Name.LocalName == "t") // Text
                                {
                                    xaml += text.Value;
                                }
                            }
                            xaml += "</Run>";
                        }
                    }
                    xaml += "</Paragraph>";
                }
            }
            xaml += "</FlowDocument>";
            return xaml;
        }

        // Расширение для получения XDocument из MainDocumentPart (если его нет в OpenXml SDK)
        public static System.Xml.Linq.XDocument GetXDocument(this MainDocumentPart part)
        {
            using (Stream stream = part.GetStream())
            {
                if (stream != null)
                {
                    try
                    {
                        return System.Xml.Linq.XDocument.Load(stream);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при загрузке XDocument: {ex.Message}");
                        return null;
                    }
                }
                return null;
            }
        }
    }
}
