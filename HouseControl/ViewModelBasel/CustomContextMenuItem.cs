using System.Windows.Input;
using Facade;

public class CustomContextMenuItem : IContexMenuItem
{
    public CustomContextMenuItem(string text, ICommand todo)
    {
        Text = text;
        Todo = todo;
    }

    public string Text { get; }
    public ICommand Todo { get; }


}