using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Core;

namespace UnexpectedApisDemo.Shared.ViewModels
{
    [Windows.UI.Xaml.Data.Bindable]
    public class ViewModelBase : INotifyPropertyChanged
    {
        private readonly Dictionary<string, Command> _commands = new Dictionary<string, Command>();

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModelBase()
        {
        }

        public CompositeDisposable Disposables { get; } = new CompositeDisposable();

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected Command GetOrCreateCommand<T>(Action<T> action, [CallerMemberName] string commandName = null)
        {
            if (!_commands.TryGetValue(commandName, out var command))
            {
                _commands[commandName] = command = new Command(x => action((T)x));
            }
            return command;
        }

        protected Command GetOrCreateCommand(Action action, [CallerMemberName] string commandName = null)
        {
            if (!_commands.TryGetValue(commandName, out var command))
            {
                _commands[commandName] = command = new Command(_ => action());
            }
            return command;
        }

        [Windows.UI.Xaml.Data.Bindable]
        public class Command : ICommand
        {
            private readonly Action<object> _action;
            private readonly Func<object, Task> _actionTask;
            private readonly Func<object, bool> _canExecute;
            private bool _manualCanExecute = true;

            public bool ManualCanExecute
            {
                get => _manualCanExecute;
                set
                {
                    _manualCanExecute = value;
                    CanExecuteChanged?.Invoke(this, null);
                }
            }

            private object _isExecuting = null;
            private static object _isExecutingNull = new object(); // this represent a null parameter when something is executing

            public Command(Action<object> action, Func<object, bool> canExecute = null)
            {
                _action = action ?? throw new ArgumentNullException(nameof(action));
                _canExecute = canExecute;
            }

            public Command(Func<object, Task> actionTask, Func<object, bool> canExecute = null)
            {
                _actionTask = actionTask ?? throw new ArgumentNullException(nameof(actionTask));
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter)
            {
                var canExecuteParameter = parameter ?? _isExecutingNull;

                return (_isExecuting != canExecuteParameter)
                    && (_canExecute?.Invoke(parameter) ?? true)
                    && _manualCanExecute;
            }

            public async void Execute(object parameter)
            {
                var isExecutingParameter = parameter ?? _isExecutingNull;

                if (_isExecuting == isExecutingParameter)
                {
                    // This parameter is executing
                    return;
                }

                try
                {
                    _isExecuting = isExecutingParameter;
                    CanExecuteChanged?.Invoke(this, null);
                    if (_action != null)
                    {
                        _action(parameter);
                    }
                    else
                    {
                        await _actionTask(parameter);
                    }
                }
                finally
                {
                    _isExecuting = null;
                    CanExecuteChanged?.Invoke(this, null);
                }
            }

            public event EventHandler CanExecuteChanged;
        }
    }
}
