using Incas.Core.ViewModels;
using System.Windows;
using static IncasEngine.ObjectiveEngine.Models.State;

namespace Incas.Objects.ViewModels
{
    public class ColumnConfiguration : BaseViewModel
    {
        private bool isReadOnly = false;
        public bool IsReadOnly
        {
            get => this.isReadOnly;
            set
            {
                this.isReadOnly = value;
                this.OnPropertyChanged(nameof(this.IsReadOnly));
            }
        }
        private Visibility visibility = Visibility.Visible;
        public Visibility Visibility
        {
            get => this.visibility;
            set
            {
                this.visibility = value;
                this.OnPropertyChanged(nameof(this.Visibility));
            }
        }
        public void Apply(MemberState state)
        {
            this.Visibility = state.EditorVisibility ? Visibility.Visible : Visibility.Collapsed;
            this.IsReadOnly = !state.IsEnabled;
        }
    }
}
