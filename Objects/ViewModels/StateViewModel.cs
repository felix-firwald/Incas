using Incas.Core.Classes;
using Incas.Core.ViewModels;
using Incas.Objects.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Documents;

namespace Incas.Objects.ViewModels
{
    /// <summary>
    /// Класс ViewModel.
    /// Метод Save вызывается после применения изменений на форме и подтверждает сохранение изменений из ViewModel в Model.
    /// </summary>
    public class StateViewModel : BaseViewModel
    {
        /// <summary>
        /// Model
        /// </summary>
        public State Source { get; set; }

        public delegate void Action(StateViewModel state);
        public event Action OnRemoveRequested;

        public ClassViewModel Owner { get; set; }

        public StateViewModel(State source, ClassViewModel owner)
        {
            this.Source = source;
            this.Owner = owner;
            this.Members = new();
            this.Owner.Fields.CollectionChanged += this.Members_CollectionChanged;
            this.Owner.Methods.CollectionChanged += this.Members_CollectionChanged;
            this.Owner.Tables.CollectionChanged += this.Members_CollectionChanged;
            this.ApplyCollection();
        }

        private void DoRemoveState(object obj)
        {
            if (DialogsManager.ShowQuestionDialog($"Вы действительно хотите удалить состояние [{this.Name}]? После сохранения это действие отменить нельзя: это состояние будет безвозвратно удалено (к существующим объектам применится состояние по-умолчанию).", "Удалить состояние?", "Удалить", "Не удалять") == Core.Views.Windows.DialogStatus.Yes)
            {
                this.OnRemoveRequested?.Invoke(this);
            }
        }

        private void ApplyCollection()
        {           
            foreach (IClassMemberViewModel item in this.Owner.Members)
            {                
                State.MemberState memberState = new();
                if (!this.Source.Settings.TryGetValue(item.Id, out memberState))
                {
                    memberState = new()
                    {
                        IsEnabled = true,
                        CardVisibility = true,
                        EditorVisibility = true
                    };
                }
                if (item.ClassMemberType == IClassMemberViewModel.MemberType.Table)
                {
                    this.Members.Add(new(item, memberState, (item as TableViewModel).Fields));
                }
                else
                {
                    this.Members.Add(new(item, memberState));
                }               
            }
        }

        private void Members_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.Save();
            this.Members.Clear();
            this.ApplyCollection();
        }

        public string Name
        {
            get
            {
                return this.Source.Name;
            }
            set
            {
                this.Source.Name = value;
                this.OnPropertyChanged(nameof(this.Name));
            }
        }
        public ObservableCollection<StateMemberViewModel> Members { get; set; }
        private StateMemberViewModel selectedMember;
        public StateMemberViewModel SelectedMember
        {
            get
            {
                return this.selectedMember;
            }
            set
            {
                this.selectedMember = value;
                this.OnPropertyChanged(nameof(this.SelectedMember));
            }
        }

        public void Save()
        {
            this.Source.Settings = new();
            this.Source.SetId();
            foreach (StateMemberViewModel item in this.Members)
            {
                item.Save();
                this.Source.Settings.Add(item.Source.Id, item.State);
            }
        }
    }
}
