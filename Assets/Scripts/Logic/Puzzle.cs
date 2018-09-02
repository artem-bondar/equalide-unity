using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

// Contains puzzle with next representation:
// '0-9' - colored cell
// 'e' - empty cell, can be colored
// 'b' - blank cell, can't be colored

public class Puzzle  {

    // Holds string representation of 2D-array
    public string partition
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
            char[] copy = partition.ToCharArray();
            copy[i * width + j] = value;
            partition = new string(copy);
        }
    }

    // Amount of elements in puzzle
    public int parts { get; private set; }

    public int width { get; private set; }
    public int height { get; private set; }

    public bool opened { get; private set; }
    public bool solved { get; private set; }

    public Puzzle(string partition, int parts,
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
        partition = Regex.Replace(partition, "[^eb]", "e");;
    }

    public bool CheckForSolution()
    {
        // Checks if puzzle contains any unpainted primitive
        if (partition.IndexOf('e') != -1)
            return false;

        List<Element> elements = SeparateInElements();

        if (elements.Count != parts)
            return false;

        if (!elements[0].CheckConnectivity())
            return false;

        // Checks if elements are equal
        for (int i = 1; i < elements.Count; i++)
            if (!elements[0].Compare(elements[i]))
                return false;

        return solved = true;
    }

    private List<Element> SeparateInElements() 
    {
        var unicalCells = new HashSet<char>();
        for (int i = 0; i < partition.Length; i++)
        {
            if (partition[i] != 'b' && partition[i] != 'e')
                unicalCells.Add(partition[i]);
        }

        var result = new List<Element>();

        foreach(char cell in unicalCells) {
            var firstOccurance = partition.IndexOf(cell);
            var lastOccurance = partition.LastIndexOf(cell);
            var substr = partition.Substring(firstOccurance - firstOccurance % width,
                        (lastOccurance - firstOccurance+1) + width - (lastOccurance - firstOccurance+1) % width);
            substr = Regex.Replace(substr, "[^" + cell.ToString() + "]", "e");
            substr = substr.Replace(cell, 'c');
            result.Add(new Element(substr, width));
            
        }
        return result;
    }

    private bool CheckIfValidPartition(string partition)
    {
        if (this.partition.Length != partition.Length)
            return false;
        
        for (int i = 0; i < partition.Length; i++)
            if ((this.partition[i] == 'b' && partition[i] != 'b') ||
                (this.partition[i] != 'b' && partition[i] == 'b') ||
                !System.Char.IsDigit(partition[i]) ||
                partition[i] - '0' >= this.parts)
                return false;

        return true;
    }

    private bool CheckIfValidIndexes(int i, int j)
    {
        return i * width + j > 0 && i * width + j < partition.Length;
    }
}
