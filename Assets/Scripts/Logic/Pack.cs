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

    // Receives pack in simple text format.
    // '\n\n' between two puzzles
    // '\n' between lines in puzzle (no '\n' for last line)
    //
    // Example of input text:
    // "10\n10\n\n10\n10"
    // ---
    // 10
    // 10
    //
    // 10
    // 10
    // ---
    public Pack(string rawPackText)
    {
        
    }

    public static explicit operator Pack(string rawPackText)
    {
        return new Pack(rawPackText);
    }
}
