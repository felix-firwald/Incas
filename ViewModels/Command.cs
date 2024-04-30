using System;
using System.Windows.Input;

namespace Incubator_2.ViewModels
{
    public class Command : ICommand
    {
        private Action<object> executeMethod;
#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067
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
            this.executeMethod(parameter);
        }
    }
}