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

    public IEnumerator<Puzzle> GetEnumerator()
    {
        return (IEnumerator<Puzzle>)puzzles.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return puzzles.GetEnumerator();
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
        string[] puzzles = rawPackText.Split(new[] { "\n\n " }, 0);

        size = puzzles.Length;
        this.puzzles = new Puzzle[size];

        for (var i = 0; i < size; i++)
        {
            var unicalCells = new HashSet<char>(puzzles[i].ToCharArray());
            unicalCells.RemoveWhere(c => c == 'b' || c == 'e');

            var lines = puzzles[i].Split('\n');

            var height = lines.Length;
            var width = lines[0].Length;

            this.puzzles[i] = new Puzzle(
                System.String.Join("", lines), unicalCells.Count,
                width, height, false, false);
        }
    }
}
