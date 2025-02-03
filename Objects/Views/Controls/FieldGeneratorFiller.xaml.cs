using Incas.Core.Classes;
using Incas.Objects.Interfaces;
using Incas.Objects.Views.Pages;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Exceptions;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using static Incas.Objects.Interfaces.IFillerBase;

namespace Incas.Objects.Views.Controls
{
    public enum GeneratorStatus
    {
        NotContented,
        Contented,
        Warning
    }

    /// <summary>
    /// Логика взаимодействия для GeneratorFiller.xaml
    /// </summary>
    public partial class FieldGeneratorFiller : UserControl, IGeneratorFiller
    {
        public event FillerUpdate OnFillerUpdate;
        public event FillerUpdate OnDatabaseObjectCopyRequested;
        public event StringAction OnInsert;
        public Class TargetClass { get; set; }
        public ClassData TargetClassData { get; set; }
        public Field Field { get; set; }
        
        public FieldGeneratorFiller(Field f) // new
        {
            this.InitializeComponent();
            this.Field = f;
            this.GeneratorName.Content = f.VisibleName;
            this.TargetClass = new(Guid.Parse(f.Value));
            this.TargetClassData = this.TargetClass.GetClassData();
        }
        public string GetData()
        {
            return JsonConvert.SerializeObject(this.GetObjects());
        }
        public object GetDataForScript()
        {
            return this.GetObjects();
        }
        public List<IObject> GetObjects()
        {
            List<IObject> objs = new();
            foreach (ObjectCreator oc in this.ContentPanel.Children)
            {
                objs.Add(oc.PullObject());
            }
            return objs;
        }
        public void SetObjects(List<IObject> objs)
        {
            foreach (IObject obj in objs)
            {
                this.AddObjectCreator(obj);
            }
        }
        public void ApplyObjectsBy(IClass cl, Guid obj)
        {
            //this.SetObjects(Processor.GetRelatedObjects(this.TargetClass, cl, obj));
        }
        public void SetValue(string value)
        {
            this.SetObjects(JsonConvert.DeserializeObject<List<IObject>>(value));
        }
        public void MarkAsNotValidated()
        {

        }
        public void MarkAsValidated()
        {

        }
        
        private void AddObjectCreator(IObject obj = null)
        {
            ObjectCreator creator = new(this.TargetClass, null, obj);
            creator.OnUpdated += this.Creator_OnUpdated;
            creator.OnRemoveRequested += this.Creator_OnRemoveRequested;
            this.ContentPanel.Children.Add(creator);
        }

        private bool Creator_OnRemoveRequested(ObjectCreator creator)
        {
            if (DialogsManager.ShowQuestionDialog("Объект будет безвозвратно удален. Вы уверены, что хотите удалить его?", "Удалить объект?", "Удалить", "Не удалять") == Core.Views.Windows.DialogStatus.Yes)
            {
                Processor.RemoveObject(this.TargetClass, creator.Object.Id);
                this.ContentPanel.Children.Remove(creator);
                return true;
            }
            return false;
        }

        private bool Creator_OnUpdated(ObjectCreator creator)
        {
            this.MarkAsValidated();
            this.OnFillerUpdate?.Invoke(this);
            return true;
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            this.AddObjectCreator();
        }

        private void SaveToFileClick(object sender, RoutedEventArgs e)
        {

        }

        private void GetFromExcelClick(object sender, RoutedEventArgs e)
        {

        }

        private void InsertToOtherClick(object sender, RoutedEventArgs e)
        {
            try
            {
                OnInsert?.Invoke(this.Field.Id, this.GetData());
            }
            catch (NotNullFailed)
            {
                DialogsManager.ShowExclamationDialog("Поле является обязательным, необходимо сначала присвоить ему значение.", "Переназначение прервано");
            }
        }

        private void ObjectCopyRequestClick(object sender, RoutedEventArgs e)
        {
            this.OnDatabaseObjectCopyRequested?.Invoke(this);
        }
    }
}
