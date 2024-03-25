using System;
using System.Windows.Input;

namespace Incubator_2.ViewModels
{
    public class Command : ICommand
    {
        Action<object> executeMethod;
        public event EventHandler CanExecuteChanged;

        public Command(Action<object> method)
        {
            this.executeMethod = method;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            executeMethod(parameter);
        }
    }
}