using System;
using System.Windows.Input;

namespace Generic.Commands
{
    public class RelayCommandWithParameter : ICommand
    {
        readonly Action<object> _execute;
        readonly Func<bool> _canExecute;

        public RelayCommandWithParameter(Action<object> execute)
            : this(execute, null)
        {
        }

        public RelayCommandWithParameter(Action<object> execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }


        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute.Invoke();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

    }
}
