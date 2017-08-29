using System;
using System.Windows.Input;
using Facade;

public class PasteCommandHandler : CommandHandler
{
    private readonly ICopyService _copyService;
    private readonly ITreeNode _self;

    public PasteCommandHandler(Action<bool> action,ICopyService copyService,ITreeNode self) : base(action)
    {
        _copyService = copyService;
        _self = self;
        _copyService.CopyObjChanged += () => { base.Executable = this.Executable; };
    }

    public override bool Executable
    {
        get { return _copyService.AllowPasteOn(_self); }
    }
}
public class CommandHandler : ICommand
{
    private Action<bool> _action;
    private bool _executable;

    public CommandHandler(Action<bool> action)
    {
        Executable = true;
        _action = action;
    }

    public virtual bool Executable
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