namespace BoardGames.Commands;

public class MoveHistory
{
    private readonly Stack<IMoveCommand> _undoStack = new();
    private readonly Stack<IMoveCommand> _redoStack = new();
    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;
    public int ExecutedCount => _undoStack.Count;

    public void Execute(IMoveCommand cmd)
    {
        cmd.Execute();
        // Push the command onto the 'Undo Stack'
        _undoStack.Push(cmd);

        // Clear the 'Redo Stack' because the timeline has diverged
        _redoStack.Clear();
    }

    public IMoveCommand? Undo()
    {
        // If Undo stack is empty, return null
        if (!CanUndo) return null;

        // Pop the last command from the Undo Stack
        var cmd = _undoStack.Pop();

        // Run its Undo logic to revert the board state
        cmd.Undo();

        // Push it onto the Redo Stack
        _redoStack.Push(cmd);
        return cmd;
    }

    public IMoveCommand? Redo()
    {
        // If the Redo Stack is empty, return null
        if (!CanRedo) return null;

        // Pop the last command from the Redo Stack
        var cmd = _redoStack.Pop();

        // Run its Execute Logic to reapply the move
        cmd.Undo();

        // Push it back onto the Undo Stack
        _undoStack.Push(cmd);
        return cmd;
    }

    // Commands currently executed, in chronological order (oldest first)
    public IEnumerable<IMoveCommand> ExecutedCommands => _undoStack.Reverse();

    // All commands in the timeline (executed + undone), in chronological order
    public IEnumerable<IMoveCommand> AllCommands
        => _undoStack.Reverse().Concat(_redoStack);
}