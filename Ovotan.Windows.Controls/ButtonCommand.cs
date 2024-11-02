using System.Windows.Input;

namespace Ovotan.Windows.Controls.Controls
{
    /// <summary>
    /// Класс описывает базовую команду.
    /// </summary>
    /// <typeparam name="T"></typeparam>
	public class ButtonCommand<T1> : ICommand where T1 : class
    {
        /// <summary>
        /// Выполнняемое дествие при вызове команды.
        /// </summary>
        Action<T1> _executeAction;
        Func<object, bool> _canExecuteFunction;
        public event EventHandler CanExecuteChanged;

        public ButtonCommand(Action<T1> executeAction)
        {
            _executeAction = executeAction;
        }
        public ButtonCommand(Action<T1> executeAction, Func<object, bool> canExecuteFunction)
        {
            _executeAction = executeAction;
            _canExecuteFunction = canExecuteFunction;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecuteFunction == null ? true : _canExecuteFunction(parameter);
        }

        public void Execute(object parameter)
        {
            _executeAction(parameter as T1);
        }
    }
}
