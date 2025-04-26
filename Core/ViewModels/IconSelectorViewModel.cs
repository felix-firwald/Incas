using Incas.Core.Classes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Incas.Core.ViewModels
{
    public class IconSelectorViewModel : BaseViewModel
    {
        public IconSelectorViewModel()
        {
            
        }
        public Dictionary<Icon, string> Icons
        {
            get
            {
                return IconsManager.Icons;
            }
        }
        private KeyValuePair<Icon, string> selected;
        public KeyValuePair<Icon, string> SelectedIcon
        {
            get
            {
                return this.selected;
            }
            set
            {
                this.selected = value;
                this.OnPropertyChanged(nameof(this.SelectedIcon));
            }
        }
        private string customSelectedIcon;
        public string CustomSelectedIcon
        {
            get
            {
                return this.customSelectedIcon;
            }
            set
            {
                this.customSelectedIcon = value;
                this.OnPropertyChanged(nameof(this.CustomSelectedIcon));
            }
        }
        public enum SelectorMode
        {
            PredefinedIcons,
            CustomIcon
        }
        private SelectorMode _mode;
        public SelectorMode Mode
        {
            get
            {
                return this._mode;
            }
            set
            {
                this._mode = value;
                this.OnPropertyChanged(nameof(this.Mode));
                this.OnPropertyChanged(nameof(this.PredefinedVisibility));
                this.OnPropertyChanged(nameof(this.CustomVisibility));
            }
        }
        public Visibility PredefinedVisibility
        {
            get
            {
                switch (this.Mode)
                {
                    case SelectorMode.PredefinedIcons: return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        public Visibility CustomVisibility
        {
            get
            {
                switch (this.Mode)
                {
                    case SelectorMode.CustomIcon: return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
    }
}
