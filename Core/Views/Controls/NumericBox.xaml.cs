using IncasEngine.ObjectiveEngine.FieldComponents;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incas.Core.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для NumericBox.xaml
    /// </summary>
    public partial class NumericBox : UserControl, INotifyPropertyChanged
    {
        public int MinValue = 0;
        public int MaxValue = int.MaxValue;
        private int currentValue;
        public int Value
        {
            get => Math.Clamp(this.currentValue, this.MinValue, this.MaxValue);
            set
            {
                this.currentValue = value;
                this.UpdateButtonsState();
                this.OnPropertyChanged("Value");
                this.OnNumberChanged?.Invoke(this.currentValue);
            }
        }
        public delegate void ValueChanged(object sender);
        public event ValueChanged OnNumberChanged;

        public NumericBox()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        public void ApplyMinAndMax(NumberFieldData input)
        {
            this.MinValue = input.MinValue;
            this.Value = input.DefaultValue;
            this.MaxValue = input.MaxValue;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateButtonsState()
        {
            this.MinusButton.IsEnabled = this.Value > this.MinValue;
            this.PlusButton.IsEnabled = this.Value < this.MaxValue;
        }

        private void DecrementClick(object sender, MouseButtonEventArgs e)
        {
            this.Value--;
        }
        private void IncrementClick(object sender, MouseButtonEventArgs e)
        {
            this.Value++;
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            int checkDigit;
            if (!int.TryParse(((TextBox)sender).Text, out checkDigit))
            {
                this.Value = this.currentValue;
                this.Input.CaretIndex = this.Input.Text.Length;
            }
        }
    }
}
