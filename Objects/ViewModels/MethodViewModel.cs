using AvaloniaEdit.Document;
using Incas.Core.ViewModels;
using Incas.Objects.Interfaces;
using Incas.Objects.Views.Pages;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.Windows.Input;

namespace Incas.Objects.ViewModels
{
    public class MethodViewModel : BaseViewModel, IClassMemberViewModel
    {
        public delegate void OpenMethod(IClassDetailsSettings settings);
        public event OpenMethod OnOpenMethodRequested;
        public delegate void Action(MethodViewModel field);
        public event Action OnRemoveRequested;
        public Method Source { get; set; }
        public ClassViewModel Owner;
        public MethodViewModel(Method method, ClassViewModel cvm)
        {
            this.Source = method;
            this.textDocument = new()
            {
                Text = this.Source.Code ?? "\n\n\n\n\n"
            };
            this.Owner = cvm;
            this.textDocument.TextChanged += this.TextDocument_TextChanged;
            this.OpenMethodSettings = new Command(this.DoOpenMethod);
            this.RemoveMethod = new Command(this.DoRemoveMethod);
            this.AssignToContainer = new Command(this.DoAssignToContainer);
        }

        private void DoAssignToContainer(object obj)
        {
            if (this.Owner.SelectedViewControl is not null)
            {
                this.Source.SetId();
                ViewControlViewModel vm = new(
                    new()
                    {
                        Name = this.Name,
                        Type = IncasEngine.ObjectiveEngine.Common.FunctionalityUtils.CustomForms.ControlType.Button,
                        RunMethod = this.Source.Id
                    }
                );
                this.Owner.RemoveFieldControl(this.Source.Id);
                this.Owner.SelectedViewControl.AddChild(vm);
            }
        }

        public bool BelongsThisClass
        {
            get
            {
                return this.Source.Owner is null;
            }
        }

        private void DoRemoveMethod(object obj)
        {
            this.OnRemoveRequested?.Invoke(this);
        }

        private void DoOpenMethod(object obj)
        {
            MethodEditor editor = new(this);
            this.OnOpenMethodRequested?.Invoke(editor);
        }

        public ICommand OpenMethodSettings { get; set; }
        public ICommand AssignToContainer { get; set; }
        public ICommand RemoveMethod { get; set; }

        private void TextDocument_TextChanged(object sender, System.EventArgs e)
        {
            this.Source.Code = this.textDocument.Text;
        }

        public string Name
        {
            get
            {
                return this.Source.Name;
            }
            set
            {
                this.Source.Name = value.Replace(' ', '_').Replace('.', '_');
                this.OnPropertyChanged(nameof(this.Name));
            }
        }
        public string Code
        {
            get
            {
                return this.Source.Code;
            }
            set
            {
                this.Source.Code = value;
                this.OnPropertyChanged(nameof(this.Code));
            }
        }
        private TextDocument textDocument;
        public TextDocument CodeModule
        {
            get => this.textDocument;
            set
            {
                this.textDocument = value;
                this.Source.Code = value.Text;
                this.OnPropertyChanged(nameof(this.CodeModule));
            }
        }

        public Guid Id
        {
            get
            {
                return this.Source.Id;
            }
        }

        public IClassMemberViewModel.MemberType ClassMemberType => IClassMemberViewModel.MemberType.Method;
    }
}