using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uno.Disposables;

namespace UnexpectedApis.ViewModels;

[Microsoft.UI.Xaml.Data.Bindable]
public abstract class ViewModelBase : ObservableRecipient
{
    private readonly Dictionary<string, ICommand> _commands = new();
    private readonly Dictionary<string, object?> _propertyValueStore = new();

    public CompositeDisposable Disposables { get; } = new();

    protected ICommand GetOrCreateCommand(Action action, [CallerMemberName] string commandName = "")
    {
        if (!_commands.TryGetValue(commandName, out var command))
        {
            command = new RelayCommand(action);
            _commands.Add(commandName, command);
        }
        return command;
    }

    protected ICommand GetOrCreateCommand(Action action, Func<bool> canExecute, [CallerMemberName] string commandName = "")
    {
        if (!_commands.TryGetValue(commandName, out var command))
        {
            command = new RelayCommand(action, canExecute);
            _commands.Add(commandName, command);
        }
        return command;
    }

    protected ICommand GetOrCreateCommand<T>(Action<T?> action, [CallerMemberName] string commandName = "")
    {
        if (!_commands.TryGetValue(commandName, out var command))
        {
            command = new RelayCommand<T>(action);
            _commands.Add(commandName, command);
        }
        return command;
    }

    protected ICommand GetOrCreateCommand<T>(Action<T?> action, Predicate<T?> canExecute, [CallerMemberName] string commandName = "")
    {
        if (!_commands.TryGetValue(commandName, out var command))
        {
            command = new RelayCommand<T>(action, canExecute);
            _commands.Add(commandName, command);
        }
        return command;
    }

    protected ICommand GetOrCreateAsyncCommand(Func<Task> action, [CallerMemberName] string commandName = "")
    {
        if (!_commands.TryGetValue(commandName, out var command))
        {
            command = new AsyncRelayCommand(action);
            _commands.Add(commandName, command);
        }
        return command;
    }

    protected ICommand GetOrCreateAsyncCommand(Func<Task> action, Func<bool> canExecute, [CallerMemberName] string commandName = "")
    {
        if (!_commands.TryGetValue(commandName, out var command))
        {
            command = new AsyncRelayCommand(action, canExecute);
            _commands.Add(commandName, command);
        }
        return command;
    }

    protected ICommand GetOrCreateAsyncCommand<T>(Func<T?, Task> action, [CallerMemberName] string commandName = "")
    {
        if (!_commands.TryGetValue(commandName, out var command))
        {
            command = new AsyncRelayCommand<T>(action);
            _commands.Add(commandName, command);
        }
        return command;
    }

    protected ICommand GetOrCreateAsyncCommand<T>(Func<T?, Task> action, Predicate<T?> canExecute, [CallerMemberName] string commandName = "")
    {
        if (!_commands.TryGetValue(commandName, out var command))
        {
            command = new AsyncRelayCommand<T>(action, canExecute);
            _commands.Add(commandName, command);
        }
        return command;
    }

    protected T? GetProperty<T>([CallerMemberName] string? propertyName = null)
    {
        if (propertyName is null)
        {
            throw new ArgumentNullException("Property name must not be null");
        }

        return _propertyValueStore.TryGetValue(propertyName, out var value) ? (T?)value : default;
    }

    protected void SetProperty<T>(T value, [CallerMemberName] string? propertyName = null)
    {
        if (propertyName is null)
        {
            throw new ArgumentNullException("Property name must not be null");
        }

        if (!(_propertyValueStore.TryGetValue(propertyName, out var oldValue) && oldValue?.Equals(value) == true))
        {
            _propertyValueStore[propertyName] = value;
            OnPropertyChanged(propertyName);
        }
    }
}
