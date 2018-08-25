using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

// Contains puzzle with next representation:
// '0-9' - colored cell
// 'e' - empty cell, can be colored
// 'b' - blank cell, can't be colored

public class Puzzle  {


    string cleaned;

    string solution;
    public int parts;

    public int width;
    public int height;

    string partition;

    public bool opened;
    public bool solved;

    public Puzzle(string source)
    {
        var elements = new HashSet<char>();
        for (int i = 0; i < source.Length; i++)
        {
            if (source[i] != 'b' && source[i] != '\n')
                elements.Add(source[i]);
        }

        parts = elements.Count;

        var lines = Regex.Split(source, "\r\n|\r|\n");

        height = lines.Length;
        width = lines[0].Length;

        solution = Regex.Replace(source, "\r\n|\r|\n", "");
        cleaned = Regex.Replace(solution, "[0-9]", "e");
        partition = cleaned;

        opened = false;
        solved = false;
    }

    public char get(int i, int j)
    {
        return solution[i * width + j];
    }

    public void set(int i, int j, char c)
    {
        var temp = partition.ToCharArray();
        temp[i * width + j] = c;
        partition = new string(temp);
    }

    public void loadPartition(string partition)
    {
        this.partition = partition;
    }

    public void refresh()
    {
        partition = cleaned;
    }

    public bool checkIfSolved()
    {
        // Checks if puzzle contains any unpainted primitive
        if (partition.IndexOf('e') != -1)
            return false;

        var elements = separateInElements();

        if (elements.Count != parts)
            return false;

        if (!elements[0].checkConnectivity())
            return false;

        // Checks if elements are equal
        for (int i = 1; i < elements.Count; i++)
            if (elements[0] != elements[i])
                return false;

        return true;
    }

    public bool checkIfValid()
    {
        // Checks if puzzle contains any unpainted primitive
        if (partition.IndexOf('e') != -1)
            return false;

        var elements = separateInElements();

        if (elements.Count == 1)
            return false;

        if (!elements[0].checkConnectivity())
            return false;

        // Checks if elements are equal
        for (int i = 1; i < elements.Count; i++)
            if (elements[0] != elements[i])
                return false;

        return true;
    }

    private List<Element> separateInElements() 
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
            substr = substr.Replace(cell, 'c');
            substr = Regex.Replace(substr, "[0-9]", "e");

            result.Add(new Element(substr, width));
            
        }
        return result;
    }
}
