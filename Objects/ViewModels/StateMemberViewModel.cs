using Incas.Core.ViewModels;
using Incas.Objects.Interfaces;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static IncasEngine.ObjectiveEngine.Models.State;

namespace Incas.Objects.ViewModels
{
    /// <summary>
    /// Класс ViewModel.
    /// Метод Save вызывается после применения изменений на форме и подтверждает сохранение изменений из ViewModel в Model.
    /// </summary>
    public class StateMemberViewModel : BaseViewModel
    {
        /// <summary>
        /// Model
        /// </summary>
        public IClassMemberViewModel Source { get; private set; }
        public MemberState State { get; private set; }
        public StateMemberViewModel(IClassMemberViewModel member, MemberState state)
        {
            this.Source = member;
            this.State = state;
        }
        // if table
        public StateMemberViewModel(IClassMemberViewModel member, MemberState state, ObservableCollection<FieldViewModel> nested)
        {
            this.Source = member;
            this.State = state;
            this.NestedMembers = new();
            nested.CollectionChanged += this.Nested_CollectionChanged;
            this.ApplyNestedCollection(nested, false);
        }
        private void ApplyNestedCollection(ObservableCollection<FieldViewModel> nested, bool preSave = false)
        {
            if (this.State.NestedMembers is null)
            {
                this.State.NestedMembers = new();
            }
            else
            {
                if (preSave)
                {
                    this.State.NestedMembers.Clear();
                    foreach (StateMemberViewModel item in this.NestedMembers)
                    {
                        item.Save();
                        this.State.NestedMembers.TryAdd(item.Source.Id, item.State);
                    }
                    this.NestedMembers.Clear();
                }
            }
            foreach (IClassMemberViewModel item in nested)
            {
                State.MemberState memberState = new();
                if (!this.State.NestedMembers.TryGetValue(item.Id, out memberState))
                {
                    memberState = new()
                    {
                        IsEnabled = true,
                        EditorVisibility = true
                    };
                    if (item.ClassMemberType == IClassMemberViewModel.MemberType.Table)
                    {
                        memberState.InsertEnabled = true;
                        memberState.RemoveEnabled = true;
                    }
                }
                this.NestedMembers.Add(new(item, memberState));
            }
        }
        private void Nested_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.ApplyNestedCollection(sender as ObservableCollection<FieldViewModel>, true);
        }

        public string Name
        {
            get
            {
                return this.Source.Name;
            }
        }
        public IClassMemberViewModel.MemberType ClassMemberType
        {
            get
            {
                return this.Source.ClassMemberType;
            }
        }
        public bool EditorVisibility
        {
            get
            {
                return this.State.EditorVisibility;
            }
            set
            {
                this.State.EditorVisibility = value;
                this.OnPropertyChanged(nameof(this.EditorVisibility));
            }
        }
        public bool CardVisibility
        {
            get
            {
                return this.State.CardVisibility;
            }
            set
            {
                this.State.CardVisibility = value;
                this.OnPropertyChanged(nameof(this.CardVisibility));
            }
        }
        public bool IsEnabled
        {
            get
            {
                return this.State.IsEnabled;
            }
            set
            {
                this.State.IsEnabled = value;
                this.OnPropertyChanged(nameof(this.IsEnabled));
            }
        }
        public bool IsRequired
        {
            get
            {
                return this.State.IsRequired;
            }
            set
            {
                this.State.IsRequired = value;
                this.OnPropertyChanged(nameof(this.IsRequired));
            }
        }
        public bool CanBeRequired
        {
            get
            {
                if (this.Source.ClassMemberType == IClassMemberViewModel.MemberType.Method)
                {
                    return false;
                }
                return true;
            }
        }

        public Visibility ColumnsSettingsVisibility
        {
            get
            {
                if (this.Source.ClassMemberType == IClassMemberViewModel.MemberType.Table)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        private ObservableCollection<StateMemberViewModel> nestedMembers;
        public ObservableCollection<StateMemberViewModel> NestedMembers
        {
            get
            {
                return this.nestedMembers;
            }
            set
            {
                this.nestedMembers = value;
                this.OnPropertyChanged(nameof(this.NestedMembers));
            }
        }
        public bool InsertEnabled
        {
            get
            {
                return this.State.InsertEnabled;
            }
            set
            {
                this.State.InsertEnabled = value;
                this.OnPropertyChanged(nameof(this.InsertEnabled));
            }
        }
        public bool RemoveEnabled
        {
            get
            {
                return this.State.RemoveEnabled;
            }
            set
            {
                this.State.RemoveEnabled = value;
                this.OnPropertyChanged(nameof(this.RemoveEnabled));
            }
        }

        /// <summary>
        /// Сохраняет изменения в Model
        /// </summary>
        public bool Save()
        {           
            if (this.ClassMemberType == IClassMemberViewModel.MemberType.Table)
            {
                if (this.NestedMembers is not null)
                {
                    this.State.NestedMembers.Clear();
                    foreach (StateMemberViewModel item in this.NestedMembers)
                    {
                        item.Save();
                        this.State.NestedMembers.Add(item.Source.Id, item.State);
                    }
                }
            }
            return true;
        }
    }
}
