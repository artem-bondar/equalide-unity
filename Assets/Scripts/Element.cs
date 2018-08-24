using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Max() and Min()

// Contains element with next representation:
// 'c' - non-empty cell
// 'e' - empty cell
public class Element
{
    private int height;
    private string body;
    private int width;
    public Element(string body, int width)
    {
        height = body.Length / width;
        cutByWidth();
    }
    bool equals(object other)
    {
        if (other == null || !(other is Element))
            return false;
        else return this.compare(other as Element);
    }
    int hashCode()
    {
        return body.GetHashCode();
    }
    // Checks equality to another element with accuracy to rotations and reflections
    private bool compare(Element other)
    {
        bool result = (this.width == other.width) && (this.height == other.height) &&
            ((this.body.Equals(other.body)) || this.mirrorByHeight().Equals(other.body));
        if (!result)
        {
            for (int i = 0; i < 3; i++) 
            {
                this.rotateClockWise();
                if ((this.width == other.width) && (this.height == other.height) &&
                    ((this.body.Equals(other.body)) || this.mirrorByHeight().Equals(other.body)))
                {
                    result = true;
                    break;
                }
                    
               
            }
        }
        return result;
    }
    // Cut element to it's bounding rectangle of the same height
    private void cutByWidth()
    {
        List<int> startIndexes = new List<int>();
        List<int> endIndexes = new List<int>();

        // Gets all starting and ending indexes of non-empty cells on every row
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (body[i * width + j] != 'e')
                {
                    startIndexes.Add(j);
                    break;
                }
            }
            for (int j = width - 1; j >= 0; j--)
            {
                if (body[i * width + j] != 'e')
                {
                    endIndexes.Add(j);
                    break;
                }
            }
        }

        // Calculate bounds by width
        int start = startIndexes.Min();
        int end = endIndexes.Max();

        // Perform cutting if possible
        if ((start != 0) || (end != width - 1))
        {
            string result = "";

            for (int i = 0; i < height; i++)
                for (int j = start; j <= end; j++)
                    result += body[i * width + j];

            body = result;
            width = end - start + 1;
            height = body.Length / width;
        }
    }
    private void rotateClockWise()
    {
        string result = "";

        for (int i = 0; i < width; i++)
            for (int j = height - 1; j >= 0; j--)
                result += body[j * width + i];

        body = result;
        width = height;
        height = body.Length / width;
    }
    private string mirrorByHeight()
    {
        string result = "";

        for (int i = 0; i < height; i++)
            for (int j = width - 1; j >= 0; j--)
                result += body[i * width + j];

        return result;
    }
    // Checks if element has only one connected component
    public bool checkConnectivity()
    {
        // Stores already traversed cell indexes
        var checkedIndexes = new HashSet<int>();

        // Stores pending cell indexes to traverse on next step
        var pendingIndexes = new HashSet<int>();
        pendingIndexes.Add(body.IndexOf("c"));


        // Stores cell indexes that could be traversed after pending cells
        var findedIndexes = new HashSet<int>();

        bool result = true;

        // Traversing figure starting from selected index (first upper left non-empty cell)
        while (pendingIndexes.Count != 0)
        {
            findedIndexes.Clear();

            foreach (var index in pendingIndexes)
            {
                // Indexes of all neighbour cells
                int up = (index - width >= 0) ? index - width : 0;
                int down = (index + width < body.Length) ? index + width : 0;
                int left = (index % width != 0) ? index - 1 : 0;
                int right = (index % width != width - 1) ? index + 1 : 0;

                var indexesForCheck = new List<int> { up, down, left, right };

                foreach (var i in indexesForCheck)
                    if (((i != 0) && (body[i] == 'c')) && !(checkedIndexes.Contains(i)))
                        findedIndexes.Add(i);
            }
            foreach (var index in pendingIndexes)
                checkedIndexes.Add(index);
            pendingIndexes.Clear();
            foreach (var index in findedIndexes)
                pendingIndexes.Add(index);
        }

        // Checks if element has any non-traversed cells
        for (int i = 0; i < body.Length; i++)
            if ((body[i] == 'c') && !(checkedIndexes.Contains(i)))
            {
                result = false;
                break;
            }

        return result;
    }
}
