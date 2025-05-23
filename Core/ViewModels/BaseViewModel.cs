﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Incas.Core.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged, ICommand
    {
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {

        }

        public Visibility FromBool(bool b)
        {
            if (b)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
