using System.Collections;
using System.Collections.Generic;

public class Pack : IEnumerable<Puzzle>
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

    public IEnumerator<Puzzle> GetEnumerator() =>
        ((IEnumerable<Puzzle>)puzzles).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => puzzles.GetEnumerator();

    public Pack(Puzzle[] puzzles, bool opened, bool solved)
    {
        this.size = puzzles.Length;
        this.puzzles = puzzles;
        this.opened = opened;
        this.solved = solved;
    }

    // Receives pack with solutions in simple text format.
    // '\n\n' between two puzzles
    // '\n' between lines in puzzle (no '\n' for last line)
    //
    // Example of input text:
    // "b10\n10b\n\nb10\n10b"
    // ---
    // b10
    // 10b
    //
    // b10
    // 10b
    // ---
    public Pack(string rawPackText)
    {
        string[] puzzles = rawPackText.Split(new[] { "\n\n" }, 0);

        size = puzzles.Length;
        this.puzzles = new Puzzle[size];

        for (var i = 0; i < size; i++)
        {
            this.puzzles[i] = new Puzzle(puzzles[i]);
        }
    }
}
