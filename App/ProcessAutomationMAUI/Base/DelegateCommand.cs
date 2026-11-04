using System.Windows.Input;

namespace ProcessAutomationMAUI.Base
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> executeAction;
        private readonly Func<object, bool> canExecuteAction;

        public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecuteAction)
        {
            this.executeAction = executeAction;
            this.canExecuteAction = canExecuteAction;
        }

        public DelegateCommand(Action<object> executeAction)
        {
            this.executeAction = executeAction;
        }

        public void Execute(object parameter) => this.executeAction(parameter);

        public bool CanExecute(object parameter) => this.canExecuteAction?.Invoke(parameter) ?? true;

        public event EventHandler CanExecuteChanged;

        public void InvokeCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
