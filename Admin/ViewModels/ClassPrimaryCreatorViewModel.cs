using Incas.Core.Classes;
using Incas.Core.ViewModels;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.Workspace;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Incas.Admin.ViewModels
{
    /// <summary>
    /// Класс ViewModel.
    /// Метод Save вызывается после применения изменений на форме и подтверждает сохранение изменений из ViewModel в Model.
    /// </summary>
    public class ClassPrimaryCreatorViewModel : BaseViewModel
    {
        /// <summary>
        /// Model
        /// </summary>
        public Class Source { get; set; }
        public delegate void Finished();
        public event Finished OnFinished;

        public ClassPrimaryCreatorViewModel()
        {
            this.Source = new();
            this.GoBack = new Command(this.DoGoBack);
            this.GoNext = new Command(this.DoGoNext);
        }

        private void DoGoNext(object obj)
        {
            if (this.CurrentStep < 4)
            {
                this.CurrentStep++;
            }
            else
            {
                if (this.Validate())
                {
                    this.OnFinished?.Invoke();
                }              
            }
        }

        private void DoGoBack(object obj)
        {
            if (this.CurrentStep > 1)
            {
                this.CurrentStep--;
            }
        }
        #region Commands
        public ICommand GoBack { get; private set; }
        public ICommand GoNext { get; private set; }

        #endregion
        public string ClassName
        {
            get
            {
                return this.Source.Name;
            }
            set
            {
                this.Source.Name = value;
                this.OnPropertyChanged(nameof(this.ClassName));
                this.OnPropertyChanged(nameof(this.ListName));
            }
        }
        private string listName = "";
        public string ListName
        {
            get
            {
                if (string.IsNullOrEmpty(this.listName))
                {
                    return $"Список объектов: {this.ClassName}";
                }
                return this.listName;
            }
            set
            {
                this.listName = value;
                this.OnPropertyChanged(nameof(this.ListName));
            }
        }
        public string ClassDescription
        {
            get
            {
                return this.Source.Description;
            }
            set
            {
                this.Source.Description = value;
                this.OnPropertyChanged(nameof(this.ClassDescription));
            }
        }
        public string InternalName
        {
            get
            {
                return this.Source.InternalName;
            }
            set
            {
                this.Source.InternalName = value.Replace(' ', '_');
                this.OnPropertyChanged(nameof(this.InternalName));
            }
        }
        public List<WorkspaceComponent> Components
        {
            get
            {
                return ProgramState.CurrentWorkspace.CurrentGroup.GetAvailableComponents();
            }
        }
        public WorkspaceComponent SelectedComponent
        {
            get
            {
                return this.Source.Component;
            }
            set
            {
                this.Source.Component = value;
                this.OnPropertyChanged(nameof(this.SelectedComponent));
            }
        }
        private int currentStep = 1;
        public int CurrentStep
        {
            get
            {
                return this.currentStep;
            }
            set
            {
                this.currentStep = value;
                this.OnPropertyChanged(nameof(this.CurrentStep));
                this.OnPropertyChanged(nameof(this.StepHeader));
                this.OnPropertyChanged(nameof(this.StepSubheader));
                this.OnPropertyChanged(nameof(this.CurrentStepTabIndex));
                this.OnPropertyChanged(nameof(this.BackButtonEnabled));
                this.OnPropertyChanged(nameof(this.NextButtonContent));
            }
        }
        public int CurrentStepTabIndex
        {
            get
            {
                return this.CurrentStep - 1;
            }
        }
        public string StepHeader
        {
            get
            {
                switch (this.CurrentStep)
                {
                    default:
                    case 1:
                        return "Наименование и описание";
                    case 2:
                        return "Компонент";
                    case 3:
                        return "Наследование типа";
                    case 4:
                        return "Реализовать обобщения";
                }
            }
        }
        public string StepSubheader
        {
            get
            {
                return $"Шаг {this.CurrentStep} из 4";
            }
        }
        public bool BackButtonEnabled
        {
            get
            {
                if (this.CurrentStep < 2)
                {
                    return false;
                }
                return true;
            }
        }
        public string NextButtonContent
        {
            get
            {
                switch (this.CurrentStep)
                {
                    default:
                        return "Далее";
                    case 4:
                        return "В редактор";
                }
            }
        }

        /// <summary>
        /// Сохраняет изменения в Model
        /// </summary>
        public bool Validate()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(this.ClassName))
                {
                    DialogsManager.ShowExclamationDialog("Классу не присвоено имя.", "Действие прервано");
                    return false;
                }
                using Class cl = new();
                foreach (string c in cl.GetAllClassesNames())
                {
                    if (c == this.ClassName)
                    {
                        DialogsManager.ShowExclamationDialog("Класс с таким именем уже существует.", "Действие прервано");
                        return false;
                    }
                }
                if (this.SelectedComponent is null)
                {
                    DialogsManager.ShowExclamationDialog("Не выбран компонент, в котором класс будет размещаться.", "Действие прервано");
                    return false;
                }
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
                return false;
            }
            return true;
        }
    }
}
