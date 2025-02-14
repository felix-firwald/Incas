using Incas.Core.Classes;
using Incas.Rendering.Components;
using Incas.Rendering.Components.SimpleRendering;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Incas.Objects.Documents.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для TemplateDebug.xaml
    /// </summary>
    public partial class TemplateDebug : Window
    {
        public TemplateDebug(Template template)
        {
            this.InitializeComponent();
            string path = ProgramState.CurrentWorkspace.GetFullnameOfDocumentFile(template.File);
            this.LoadDocx(path);
        }
        private void LoadDocx(string filePath)
        {
            FlowDocument flowDocument = DocXToFlowDocument.Convert(filePath);

            if (flowDocument != null)
            {
                this.Viewer.Document = flowDocument;
            }
        }
    }
}
