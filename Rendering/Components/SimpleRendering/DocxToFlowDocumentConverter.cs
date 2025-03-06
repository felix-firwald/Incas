using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Xceed.Words.NET;
using Paragraph = System.Windows.Documents.Paragraph;
using Table = System.Windows.Documents.Table;
using TableCell = System.Windows.Documents.TableCell;
using TableRow = System.Windows.Documents.TableRow;
using Run = System.Windows.Documents.Run;
using Incas.Core.Classes;
using Xceed.Document.NET;
using Image = Xceed.Document.NET.Image;
using System.Collections.Generic;

namespace Incas.Rendering.Components.SimpleRendering
{
    public static class DocXToFlowDocument
    {
        private static FlowDocument flowDoc;
        public static FlowDocument Convert(string docxFilePath)
        {
            try
            {
                using DocX document = DocX.Load(docxFilePath);
                flowDoc = new()
                {
                    PagePadding = new Thickness(20)
                };
                flowDoc.Background = Brushes.White;
                foreach (Xceed.Document.NET.Paragraph docxParagraph in document.Paragraphs)
                {
                    flowDoc.Blocks.Add(ConvertParagraph(docxParagraph));
                }

                return flowDoc;
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog($"Error converting DocX to FlowDocument: {ex.Message}", "Error");
                return null;
            }
        }

        private static Paragraph ConvertParagraph(Xceed.Document.NET.Paragraph docxParagraph)
        {
            Paragraph wpfParagraph = new()
            {
                Margin = new Thickness(0) // remove default margin
            };
            foreach (Xceed.Document.NET.FormattedText run in docxParagraph.MagicText)
            {
                Run wpfRun = new()
                {
                    Text = run.text
                };
                if (run.formatting?.Size is not null and not null)
                {
                    wpfRun.FontSize = (double)run.formatting.Size;
                }
                if (run.formatting?.FontFamily is not null and not null)
                {
                    wpfRun.FontFamily = new FontFamily(run.formatting.FontFamily.Name);
                }
                if (run.formatting?.Bold is not null && (bool)run.formatting?.Bold)
                {
                    wpfRun.FontWeight = FontWeights.Bold;
                }

                if (run.formatting?.Italic is not null && (bool)run.formatting?.Italic)
                {
                    wpfRun.FontStyle = FontStyles.Italic;
                }

                if (run.formatting?.UnderlineStyle is not null and not UnderlineStyle.none)
                {
                    wpfRun.TextDecorations = TextDecorations.Underline; // Map to WPF underlines
                }

                if (run.formatting?.FontColor is not null && run.formatting.FontColor != System.Drawing.Color.Empty)
                {
                    System.Drawing.Color color = (System.Drawing.Color)run.formatting.FontColor;
                    wpfRun.Foreground = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
                }
                else
                {
                    wpfRun.Foreground = Brushes.Black;
                }
                wpfParagraph.Inlines.Add(wpfRun);
            }
            return wpfParagraph;
        }

        private static Table ConvertTable(Xceed.Document.NET.Table docxTable)
        {
            Table wpfTable = new()
            {
                // Copy table properties (border, etc.)
                CellSpacing = 0, // Remove default cell spacing
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black // Set a default border
            };

            // Copy columns
            foreach (double columnWidth in docxTable.ColumnWidths)
            {
                wpfTable.Columns.Add(new TableColumn() { Width = new GridLength(columnWidth) });
            }

            // Copy rows
            foreach (Row docxRow in docxTable.Rows)
            {
                TableRow wpfRow = new();
                wpfTable.RowGroups.Add(new TableRowGroup());
                wpfTable.RowGroups[0].Rows.Add(wpfRow);

                foreach (Cell docxCell in docxRow.Cells)
                {
                    TableCell wpfCell = new();

                    foreach (Xceed.Document.NET.Paragraph element in docxCell.Paragraphs) // A cell can contain multiple paragraphs
                    {
                        wpfCell.Blocks.Add(ConvertParagraph(element));
                    }

                    wpfCell.BorderThickness = new Thickness(1);
                    wpfCell.BorderBrush = Brushes.Black;
                    wpfCell.Padding = new Thickness(5);

                    wpfRow.Cells.Add(wpfCell);
                }
            }

            return wpfTable;
        }
        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private static BlockUIContainer ConvertPicture(Xceed.Document.NET.Picture docxPicture)
        {
            try
            {
                //// Get the image data from the DocX picture.
                //byte[] imageData = ReadFully(docxPicture.Stream);

                //// Create a BitmapImage from the byte array.
                //BitmapImage bitmapImage = new();
                //using (MemoryStream ms = new(imageData))
                //{
                //    ms.Position = 0;
                //    bitmapImage.BeginInit();
                //    bitmapImage.CacheOption = BitmapCacheOption.OnLoad; // Important for avoiding resource leaks
                //    bitmapImage.StreamSource = ms;
                //    bitmapImage.EndInit();
                //}
                //bitmapImage.Freeze(); // Freeze the image to make it thread-safe

                //// Create an Image control and set its source.
                //System.Drawing.Image image = bitmapImage;
                ////image.
                ////{
                ////    Source = bitmapImage,
                ////    Width = docxPicture.Width,
                ////    Height = docxPicture.Height
                ////};

                //// Wrap the Image in a BlockUIContainer.
                //BlockUIContainer container = new(image)
                //{
                //    Margin = new Thickness(5)  // Add a margin around the image
                //};

                return new();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting picture: {ex.Message}");
                // Return a placeholder or null, or handle the error as needed.
                return null; // Or return an error placeholder.
            }
        }
    }
}
