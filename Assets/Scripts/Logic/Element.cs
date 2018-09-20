using System.Collections.Generic;
using System.Linq;

// Contains element with next representation:
// 'c' - non-empty cell
// 'e' - empty cell

public class Element
{
    // Holds string representation of 2D-array
    private string body;

    // Dimensions in cells
    private int height;
    private int width;

    // Receives string without any \n
    public Element(string body, int width)
    {
        this.body = body;

        this.width = width;
        height = body.Length / width;

        CutBodyByWidth();
    }

    // Assumed to be already cut by width
    private Element(string body, int width, int height)
    {
        this.body = body;
        this.width = width;
        this.height = height;
    }

    public override bool Equals(object obj)
    {
        var element = obj as Element;

        if (element == null)
        {
            return false;
        }
        else
        {
            return this == element;
        }
    }

    public override int GetHashCode() => body.GetHashCode();

    // Checks equality to another element with accuracy to rotations and reflections
    public static bool operator !=(Element first, Element second) => !(first == second);    
    public static bool operator ==(Element first, Element second)
    {
        if ((first.width != second.width || first.height != second.height) &&
            (first.height != second.width || first.width != second.height))
        {
            return false;
        }

        var equal = first.body == second.body || first.GetBodyMirroredByHeight() == second.body;

        if (!equal)
        {
            Element elementForRotate = first;

            for (var i = 0; i < 3; i++)
            {
                Element rotatedElement = elementForRotate.GetElementRotatedClockWise();

                if (equal =
                    rotatedElement.body == second.body || rotatedElement.GetBodyMirroredByHeight() == second.body)
                {
                    return true;
                }
            }
        }

        return equal;
    }

    // Cut element to it's bounding rectangle of the same height
    private void CutBodyByWidth()
    {
        var startIndexes = new List<int>();
        var endIndexes = new List<int>();

        // Gets all starting and ending indexes of non-empty cells on every row
        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < width; j++)
            {
                if (body[i * width + j] != 'e')
                {
                    startIndexes.Add(j);
                    break;
                }
            }

            for (var j = width - 1; j >= 0; j--)
            {
                if (body[i * width + j] != 'e')
                {
                    endIndexes.Add(j);
                    break;
                }
            }
        }

        // Calculate bounds by width
        var start = startIndexes.Min();
        var end = endIndexes.Max();

        // Perform cutting if possible
        if ((start != 0) || (end != width - 1))
        {
            var cutBody = string.Empty;

            for (var i = 0; i < height; i++)
            {
                for (var j = start; j <= end; j++)
                {
                    cutBody += body[i * width + j];
                }
            }

            body = cutBody;
            width = end - start + 1;
        }
    }

    // Return element rotated by 90° clockwise
    private Element GetElementRotatedClockWise()
    {
        var rotatedBody = string.Empty;

        for (var i = 0; i < width; i++)
        {
            for (var j = height - 1; j >= 0; j--)
            {
                rotatedBody += body[j * width + i];
            }
        }

        return new Element(rotatedBody, height, width);
    }

    // Return element mirrored by vertical axis
    private string GetBodyMirroredByHeight()
    {
        var mirroredBody = string.Empty;

        for (var i = 0; i < height; i++)
        {
            for (var j = width - 1; j >= 0; j--)
            {
                mirroredBody += body[i * width + j];
            }
        }

        return mirroredBody;
    }

    // Checks if element has only one component
    public bool CheckConnectivity()
    {
        // Stores already traversed cell indexes
        var checkedIndexes = new HashSet<int>();

        // Stores pending cell indexes to traverse on next step
        var pendingIndexes = new HashSet<int> { body.IndexOf("c") };

        // Stores cell indexes that could be traversed after pending cells
        var findedIndexes = new HashSet<int>();

        // Traversing figure starting from selected index (first upper left non-empty cell)
        while (pendingIndexes.Count != 0)
        {
            findedIndexes.Clear();

            foreach (var index in pendingIndexes)
            {
                // Indexes of all neighbour cells
                int? up = (index - width >= 0) ? index - width : (int?)null;
                int? down = (index + width < body.Length) ? index + width : (int?)null;
                int? left = (index % width != 0) ? index - 1 : (int?)null;
                int? right = (index % width != width - 1) ? index + 1 : (int?)null;

                var indexesForCheck = new List<int?> { up, down, left, right };

                foreach (var i in indexesForCheck)
                {
                    if (((i != null) && (body[(int)i] == 'c')) && !(checkedIndexes.Contains((int)i)))
                    {
                        findedIndexes.Add((int)i);
                    }
                }
            }

            checkedIndexes.UnionWith(pendingIndexes);
            pendingIndexes.Clear();
            pendingIndexes.UnionWith(findedIndexes);
        }

        // Checks if element has any non-traversed cells
        for (var i = 0; i < body.Length; i++)
        {
            if ((body[i] == 'c') && !(checkedIndexes.Contains(i)))
            {
                return false;
            }
        }

        return true;
    }
}
