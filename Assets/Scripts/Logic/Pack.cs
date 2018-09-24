using System.Collections;
using System.Collections.Generic;

public class Pack : IEnumerable<Puzzle>
{
    // Pack content
    public readonly int size;
    public readonly Puzzle[] puzzles;

    public Pack(Puzzle[] puzzles)
    {
        this.size = puzzles.Length;
        this.puzzles = puzzles;
    }

    // Receives pack with solutions in simple text format:
    // '\n\n' between two puzzles
    // '\n' between lines in puzzle (no '\n' for last line)
    // '0-9' - colored cell
    // 'b' - blank cell, can't be colored
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

    // Indexer interface to get puzzle using [] operator
    public Puzzle this[int i]
    {
        get
        {
            return i >= 0 && i < size ? puzzles[i] : null;
        }
    }

    public IEnumerator<Puzzle> GetEnumerator() =>
        ((IEnumerable<Puzzle>)puzzles).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => puzzles.GetEnumerator();
}
