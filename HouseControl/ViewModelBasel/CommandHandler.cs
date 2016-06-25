using System;
using System.Windows.Input;

public class CommandHandler : ICommand
{
    private Action<bool> _action;
    public CommandHandler(Action<bool> action)
    {
        _action = action;
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public event EventHandler CanExecuteChanged;

    public void Execute(object parameter)
    {
        var papram = parameter == null || bool.Parse(parameter.ToString());
        _action(papram);
    }
}