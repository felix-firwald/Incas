using ICSharpCode.AvalonEdit.Document;
using Incas.Core.Classes;
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
            this.Source.SetId();
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
        public MethodViewModel(Method method)
        {
            this.Source = method;
            this.Source.SetId();
            this.textDocument = new()
            {
                Text = this.Source.Code ?? "\n\n\n\n\n"
            };
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
                        Name = this.VisibleName,
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
        public bool EditingEnabled
        {
            get
            {
                return this.Source.TargetGeneralizator == Guid.Empty;
            }
        }
        private int fontSize = 12;
        public int FontSize
        {
            get
            {
                return this.fontSize;
            }
            set
            {
                this.fontSize = value;
                this.OnPropertyChanged(nameof(this.FontSize));
            }
        }
        private void DoRemoveMethod(object obj)
        {
            if (DialogsManager.ShowQuestionDialog($"Вы действительно хотите удалить метод [{this.Name}]? После сохранения это действие отменить нельзя: этот метод будет безвозвратно удален.", "Удалить метод?", "Удалить", "Не удалять") == Core.Views.Windows.DialogStatus.Yes)
            {
                this.OnRemoveRequested?.Invoke(this);
            }
        }

        private void DoOpenMethod(object obj)
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                this.Name = "новый_метод";
            }
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
                if (value != this.Source.Name)
                {
                    this.Source.Name = ClassDataBase.HandleName(value);
                    this.OnPropertyChanged(nameof(this.Name));
                }
            }
        }
        public string VisibleName
        {
            get
            {
                return this.Source.VisibleName;
            }
            set
            {
                if (value != this.Source.VisibleName)
                {
                    this.Source.VisibleName = ClassDataBase.HandleVisibleName(value);
                    this.OnPropertyChanged(nameof(this.VisibleName));
                }
            }
        }
        public string Description
        {
            get
            {
                return this.Source.Description;
            }
            set
            {
                this.Source.Description = value;
                this.OnPropertyChanged(nameof(this.Description));
            }
        }
        public string Icon
        {
            get
            {
                return this.Source.Icon;
            }
            set
            {
                this.Source.Icon = value;
                this.OnPropertyChanged(nameof(this.Icon));
            }
        }
        public IncasEngine.Core.Color Color
        {
            get
            {
                return this.Source.Color;
            }
        }
        public byte ColorR
        {
            get
            {
                return this.Source.Color.R;
            }
            set
            {
                IncasEngine.Core.Color color = IncasEngine.Core.Color.FromRGB(value, this.Source.Color.G, this.Source.Color.B);
                this.OnPropertyChanged(nameof(this.ColorR));             
                this.Source.Color = color;
                this.OnPropertyChanged(nameof(this.Color));
            }
        }
        public byte ColorG
        {
            get
            {
                return this.Source.Color.G;
            }
            set
            {
                IncasEngine.Core.Color color = IncasEngine.Core.Color.FromRGB(this.Source.Color.R, value, this.Source.Color.B);
                this.OnPropertyChanged(nameof(this.ColorG));
                this.Source.Color = color;
                this.OnPropertyChanged(nameof(this.Color));
            }
        }
        public byte ColorB
        {
            get
            {
                return this.Source.Color.B;
            }
            set
            {
                IncasEngine.Core.Color color = IncasEngine.Core.Color.FromRGB(this.Source.Color.R, this.Source.Color.G, value);
                this.OnPropertyChanged(nameof(this.ColorB));
                this.Source.Color = color;
                this.OnPropertyChanged(nameof(this.Color));
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