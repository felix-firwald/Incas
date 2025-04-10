using Incas.Core.ViewModels;
using Incas.Objects.Interfaces;
using IncasEngine.ObjectiveEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        /// <summary>
        /// Сохраняет изменения в Model
        /// </summary>
        public bool Save()
        {
            return false;
        }
    }
}
