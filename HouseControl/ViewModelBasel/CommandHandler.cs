using System;
using System.Windows.Input;

public class CommandHandler : ICommand
{
    private Action<bool> _action;
    private bool _executable;

    public CommandHandler(Action<bool> action)
    {
        Executable = true;
        _action = action;
    }

    public bool Executable
    {
        get { return _executable; }
        set
        {
            _executable = value;
            CanExecuteChanged?.Invoke(this,new EventArgs());
        }
    }

    public bool CanExecute(object parameter)
    {
        return Executable;
    }

    public event EventHandler CanExecuteChanged;

    public void Execute(object parameter)
    {
        var papram = parameter == null || bool.Parse(parameter.ToString());
        _action(papram);
    }
}