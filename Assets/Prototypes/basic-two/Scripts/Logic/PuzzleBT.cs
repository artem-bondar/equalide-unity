using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// Contains puzzle with next representation:
// '0-9' - colored cell
// 'e' - empty cell, can be colored
// 'b' - blank cell, can't be colored

public class PuzzleBT
{

    // Holds string representation of 2D-array
    private string partition;
    public string Partition
    {
        get { return partition; }
        set
        { 
            if (CheckIfValidPartition(value))
                partition = value;
        } 
    }

    public char this[int i, int j]
    {
        get
        {
            if (CheckIfValidIndexes(i, j))
                return partition[i * width + j];
            else
                return 'b';
        }
        set
        {   
            if (CheckIfValidIndexes(i, j))
            {
                var copy = partition.ToCharArray();
                copy[i * width + j] = value;
                partition = new string(copy);
            }
        }
    }

    // Amount of elements in puzzle
    public int parts { get; private set; }

    public int width { get; private set; }
    public int height { get; private set; }

    public bool opened { get; private set; }
    public bool solved { get; private set; }
     
    public PuzzleBT(string partition, int parts,
                  int width, int height,
                  bool opened, bool solved)
    {
        this.partition = partition;
        this.parts = parts;
        this.width = width;
        this.height = height;
        this.opened = opened;
        this.solved = solved;
    }

    // Clears puzzle partition to initial state
    public void Clear()
    {
        partition = Regex.Replace(partition, "[^be]", "0");
    }

    // Checks if puzzle partition is a solution and mark puzzle as solved if true 
    public bool CheckForSolution()
    {
        // Checks if puzzle contains any unpainted primitive
        if (partition.IndexOf('e') != -1)
            return false;

        List<ElementBT> elements = SeparateInElements();

        if (elements.Count != parts)
            return false;

        if (!elements[0].CheckConnectivity())
            return false;

        // Checks if elements are equal
        for (int i = 1; i < elements.Count; i++)
            if (elements[0] != elements[i])
                return false;

        return solved = true;
    }

    private List<ElementBT> SeparateInElements() 
    {
        var result = new List<ElementBT>();
        
        var unicalCells = new HashSet<char>(partition.ToCharArray());
        unicalCells.RemoveWhere( elem => !IsPainted(elem));

        foreach(char cell in unicalCells) {
            int firstOccurance = partition.IndexOf(cell);
            int lastOccurance = partition.LastIndexOf(cell);
            int length = lastOccurance - firstOccurance + 1;

            // Get partition that represent element cut by height
            string substr = partition.Substring(
                    firstOccurance - firstOccurance % width,
                    length + width - length % width);

            result.Add(new ElementBT(
                Regex.Replace(substr, System.String.Format("[^{0}]", cell), "e")
                .Replace(cell, 'c'),
                width));
        }

        return result;
    }

    private bool IsPainted(char c)
    {
        return c != 'b' && c != 'e';
    }

    private bool CheckIfValidPartition(string partition)
    {
        if (this.partition.Length != partition.Length)
            return false;
        
        for (int i = 0; i < partition.Length; i++)
            if ((this.partition[i] == 'b' && partition[i] != 'b') ||
                (this.partition[i] != 'b' && partition[i] == 'b') ||
                (!System.Char.IsDigit(partition[i]) && partition[i] != 'b' && partition[i] != 'e') ||
                partition[i] - '0' >= this.parts)
                return false;

        return true;
    }

    private bool CheckIfValidIndexes(int i, int j)
    {
        return i * width + j > 0 && i * width + j < partition.Length;
    }
}
