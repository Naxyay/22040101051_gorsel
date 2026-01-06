using System;
using System.Windows.Input;

namespace TacticalSentry.Presentation.Utilities
{
    public class RelayCommand : ICommand
    {
        private Action<object> _execute;
        public RelayCommand(Action<object> execute) => _execute = execute;
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => _execute(parameter);
        public event EventHandler CanExecuteChanged;
    }
}