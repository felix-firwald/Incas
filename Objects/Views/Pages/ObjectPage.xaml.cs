using Incas.Core.Classes;
using Incas.Core.Extensions;
using Incas.Core.ViewModels;
using Incas.DialogSimpleForm.Components;
using Incas.Objects.AutoUI;
using Incas.Objects.Interfaces;
using Incas.Objects.Views.Controls;
using IncasEngine.Core.Registry;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Common.FunctionalityUtils.CustomForms;
using IncasEngine.ObjectiveEngine.Exceptions;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Groups.Components;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Incas.Objects.Components.FormDrawingManager;
using static IncasEngine.ObjectiveEngine.Models.State;
using static System.Windows.Forms.AxHost;

namespace Incas.Objects.Views.Pages
{
    public class ObjectPageViewModel : BaseViewModel
    {
        private IObject source;
        public IObject Source
        {
            get
            {
                return this.source;
            }
            set
            {
                this.source = value;
                this.OnPropertyChanged(nameof(this.Source));
            }
        }
        public void SetUpdated()
        {
            this.OnPropertyChanged(nameof(this.Source));
        }
    }
    /// <summary>
    /// Логика взаимодействия для ObjectPage.xaml
    /// </summary>
    public partial class ObjectPage : UserControl
    {
        public readonly IClass Class;
        public readonly IClassData ClassData;
        public ObjectCreator Creator { get; set; }
        public ObjectPageViewModel vm { get; set; }
        public GroupClassPermissionSettings PermissionSettings { get; set; }
        private Dictionary<Button, Method> buttons;
        public ObjectPage(IClass source, IObject obj)
        {
            this.InitializeComponent();
            this.Class = source;
            this.ClassData = source.GetClassData();
            this.vm = new();
            this.DataContext = this.vm;
            this.vm.Source = obj;
            this.buttons = new();
            this.PlaceButtons();
            this.PermissionSettings = ProgramState.CurrentWorkspace.CurrentGroup.GetClassPermissions(source.Id);
            this.AddObjectCreator(obj);
            if (source.Type == IncasEngine.ObjectiveEngine.Classes.ClassType.Document)
            {
                this.RenderButton.Visibility = Visibility.Visible;
            }
            else
            {
                this.RenderButton.Visibility = Visibility.Collapsed;
            }
            DialogsManager.ShowWaitCursor(false);
            this.Class.OnUpdated += this.EngineEvents_OnUpdateClassRequested;
        }
        public ObjectPage(IClass source)
        {
            this.InitializeComponent();
            this.Class = source;
            this.ClassData = source.GetClassData();
            this.vm = new();
            this.DataContext = this.vm;
            this.buttons = new();
            this.PlaceButtons();
            this.PermissionSettings = ProgramState.CurrentWorkspace.CurrentGroup.GetClassPermissions(source.Id);
            this.AddObjectCreator();
            
            DialogsManager.ShowWaitCursor(false);
        }

        private void EngineEvents_OnUpdateClassRequested()
        {
            ClassUpdatedMessage cum = new();
            this.MainGrid.Children.Clear();
            this.MainGrid.Children.Add(cum);
            Grid.SetColumnSpan(cum, 3);
            Grid.SetRowSpan(cum, 3);
        }

        private ObjectCreator AddObjectCreator(IObject obj = null)
        {
            this.Creator = new(this.Class, obj);
            this.Creator.PermissionSettings = this.PermissionSettings;
            this.Creator.HideNCA();
            this.ContentPanel.Children.Add(this.Creator);
            this.vm.Source = this.Creator.Object;
            return this.Creator;
        }

        private async void CreateObjectsClick(object sender, RoutedEventArgs e)
        {
            await Processor.WriteObjects(this.Class, this.Creator.PullObject());
            this.vm.SetUpdated();
        }

        private void PlaceButtons()
        {
            if (this.Creator is null)
            {
                return;
            }
            this.AllButtonsPanel.Items.Clear();
            IncasEngine.ObjectiveEngine.Models.State stateTarget = null;
            foreach (IncasEngine.ObjectiveEngine.Models.State state in this.ClassData.States)
            {
                if (state.Id == this.Creator.Object.State)
                {
                    stateTarget = state;
                    break;
                }
            }
            if (stateTarget is null)
            {
                this.Creator.PullObjectForScript();
                StateUndefinedMessage sum = new(this.Creator.Object);
                this.MainGrid.Children.Clear();
                this.MainGrid.Children.Add(sum);
                Grid.SetRowSpan(sum, 3);
                Grid.SetColumnSpan(sum, 3);
                return;
            }
            foreach (Method m in this.ClassData.Methods)
            {
                MemberState memberState = stateTarget.Settings[m.Id];
                if (memberState.EditorVisibility)
                {
                    Button btn = this.MakeButton(m);
                    btn.IsEnabled = memberState.IsEnabled;
                    this.AllButtonsPanel.Items.Add(btn);
                }                              
            }            
        }
        private Button MakeButton(Method targetMethod)
        {
            Grid grid = new();
            grid.ColumnDefinitions.Add(new() { Width = new(30) });
            grid.ColumnDefinitions.Add(new());
            Label l = new()
            {
                Content = targetMethod.VisibleName,
                Foreground = new SolidColorBrush(System.Windows.Media.Colors.White),
                FontFamily = ResourceStyleManager.FindFontFamily(ResourceStyleManager.FontRubik),
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(l, 1);
            Path p = new();
            if (targetMethod.Icon != null)
            {
                p.Data = Geometry.Parse(targetMethod.Icon);
            }
            p.Fill = targetMethod.Color.AsBrush();
            p.VerticalAlignment = VerticalAlignment.Center;
            p.Stretch = Stretch.Uniform;
            p.Height = 15;
            p.Width = 15;
            grid.Children.Add(p);
            grid.Children.Add(l);
            Button btn = new()
            {
                Content = grid,
                HorizontalAlignment = HorizontalAlignment.Left,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Margin = new(0),
                ToolTip = targetMethod.Description,
                Style = ResourceStyleManager.FindStyle(ResourceStyleManager.ButtonRectangle)
            };
            btn.Click += this.Btn_Click;
            this.buttons.Add(btn, targetMethod);
            return btn;
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            this.ExternalOptions.IsOpen = false;
            this.Creator.ApplyMethod(this.buttons[sender as Button]);
        }

        private async void RenderObjectsClick(object sender, RoutedEventArgs e)
        {
            List<IObject> objects = [];
            IncasEngine.ObjectiveEngine.Types.Documents.Document doc = (this.ContentPanel.Children[0] as ObjectCreator).PullObject() as IncasEngine.ObjectiveEngine.Types.Documents.Document;

            Template templateFile = new();
            DocumentClassData docData = this.ClassData as DocumentClassData;
            if (docData.Documents?.Count == 1)
            {
                templateFile = docData.Documents[0];
            }
            else if (docData.Documents?.Count > 1)
            {
                TemplateSelection ts = new(docData);
                if (ts.ShowDialog("Выбор шаблона", Incas.Core.Classes.Icon.Magic) == true)
                {
                    templateFile = ts.GetSelectedPath();
                }
            }
            else
            {
                DialogsManager.ShowExclamationDialog("Не найдены привязанные шаблоны к этому классу.", "Рендеринг невозможен");
                return;
            }
            string path = RegistryData.GetClassTemplatePrefferedPath(this.Class.Id, templateFile);
            if (DialogsManager.ShowFolderBrowserDialog(ref path) == true)
            {
                try
                {
                    RegistryData.SetClassTemplatePrefferedPath(this.Class.Id, templateFile, path);
                    ProgramStatusBar.SetText("Выполняется рендеринг объектов...");
                    foreach (ObjectCreator c in this.ContentPanel.Children)
                    {
                        objects.Add(c.PullObject());
                        await c.GenerateDocument(templateFile, path, false);
                    }
                    ProgramStatusBar.Hide();
                    bool result = await Processor.WriteObjects(this.Class, objects);
                    ProgramState.OpenFolder(path);
                }
                catch (FieldDataFailed fdf)
                {
                    DialogsManager.ShowExclamationDialog(fdf.Message, "Рендеринг прерван");
                }
                catch (NotNullFailed nnex)
                {
                    DialogsManager.ShowExclamationDialog(nnex.Message, "Рендеринг прерван");
                }
                catch (AuthorFailed af)
                {
                    DialogsManager.ShowAccessErrorDialog(af.Message);
                }
                catch (Exception ex)
                {
                    DialogsManager.ShowErrorDialog(ex);
                }
            }
        }

        private void ExternalOptions_Opened(object sender, RoutedEventArgs e)
        {
            this.PlaceButtons();
        }
    }
}
