using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incubator_2.CustomControls
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
            get
            {
                return Math.Clamp(this.currentValue, this.MinValue, this.MaxValue);
            }
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

        public void ApplyMinAndMax(string input)
        {
            string[] parts = input.Split(';');
            switch (parts.Length)
            {
                case 2:
                    int.TryParse(parts[0], out this.MinValue);
                    int.TryParse(parts[1], out this.MaxValue);
                    break;
                case 3:
                    int.TryParse(parts[0], out this.MinValue);
                    int currentDigit;
                    if (int.TryParse(parts[1], out currentDigit))
                    {
                        this.Value = currentDigit;
                    }
                    int.TryParse(parts[2], out this.MaxValue);
                    break;
            }
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
