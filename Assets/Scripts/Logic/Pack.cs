public class Pack
{
    // Pack content
    public readonly int size;
    public readonly Puzzle[] puzzles;

    // Status for game progress
    public bool opened { get; private set; }
    public bool solved { get; private set; }

    // Indexer interface to get puzzle using [] operator
    public Puzzle this[int i]
    {
        get
        {
            if (i >= 0 && i < size)
            {
                return puzzles[i];
            }
            else
            {
                return null;
            }
        }
    }

    public Pack(int size, Puzzle[] puzzles, bool opened, bool solved)
    {
        this.size = size;
        this.puzzles = puzzles;
        this.opened = opened;
        this.solved = solved;
    }
}
