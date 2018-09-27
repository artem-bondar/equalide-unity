using System.Linq;
using System.Collections.Generic;

namespace Logic
{
    // Contains element with next representation:
    // 'c' - non-empty cell
    // 'e' - empty cell
    public class Element
    {
        // Holds string representation of 2D-array
        private string shape;

        // Dimensions in cells
        private int width;
        private int height;

        // Receives string without '\n'
        public Element(string shape, int width)
        {
            this.shape = shape;

            this.width = width;
            height = shape.Length / width;

            CutShapeByWidth();
        }

        // Assumed to be already cut by width
        private Element(string shape, int width, int height)
        {
            this.shape = shape;
            this.width = width;
            this.height = height;
        }

        public override bool Equals(object obj)
        {
            var element = obj as Element;

            return element == null ? false : this == element;
        }

        public override int GetHashCode() => shape.GetHashCode();

        // Checks equality to another element with accuracy to rotations and reflections
        public static bool operator !=(Element first, Element second) => !(first == second);
        public static bool operator ==(Element first, Element second)
        {
            if ((first.width != second.width || first.height != second.height) &&
                (first.height != second.width || first.width != second.height))
            {
                return false;
            }

            if (first.shape != second.shape && first.GetBodyMirroredByHeight() != second.shape)
            {
                Element elementForRotate = first;

                for (var i = 0; i < 3; i++)
                {
                    Element rotatedElement = elementForRotate.GetElementRotatedClockWise();

                    if (rotatedElement.shape == second.shape ||
                        rotatedElement.GetBodyMirroredByHeight() == second.shape)
                    {
                        return true;
                    }

                    elementForRotate = rotatedElement;
                }

                return false;
            }

            return true;
        }

        // Checks if element has only one component
        public bool CheckConnectivity()
        {
            // Stores already traversed cell indexes
            var checkedIndexes = new HashSet<int>();

            // Stores pending cell indexes to traverse on next step
            var pendingIndexes = new HashSet<int> { shape.IndexOf("c") };

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
                    int? down = (index + width < shape.Length) ? index + width : (int?)null;
                    int? left = (index % width != 0) ? index - 1 : (int?)null;
                    int? right = (index % width != width - 1) ? index + 1 : (int?)null;

                    var indexesForCheck = new List<int?> { up, down, left, right };

                    foreach (var i in indexesForCheck)
                    {
                        if (((i != null) &&
                            (shape[(int)i] == 'c')) && !(checkedIndexes.Contains((int)i)))
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
            for (var i = 0; i < shape.Length; i++)
            {
                if ((shape[i] == 'c') && !(checkedIndexes.Contains(i)))
                {
                    return false;
                }
            }

            return true;
        }

        // Cut element to it's bounding rectangle of the same height
        private void CutShapeByWidth()
        {
            var startIndexes = new List<int>();
            var endIndexes = new List<int>();

            // Gets all starting and ending indexes of non-empty cells on every row
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    if (shape[i * width + j] != 'e')
                    {
                        startIndexes.Add(j);
                        break;
                    }
                }

                for (var j = width - 1; j >= 0; j--)
                {
                    if (shape[i * width + j] != 'e')
                    {
                        endIndexes.Add(j);
                        break;
                    }
                }
            }

            if (startIndexes.Count == 0 && endIndexes.Count == 0)
            {
                return;
            }

            // Calculate bounds by width
            int start = startIndexes.Min();
            int end = endIndexes.Max();

            // Perform cutting if possible
            if ((start != 0) || (end != width - 1))
            {
                var cutShape = string.Empty;

                for (var i = 0; i < height; i++)
                {
                    for (var j = start; j <= end; j++)
                    {
                        cutShape += shape[i * width + j];
                    }
                }

                shape = cutShape;
                width = end - start + 1;
            }
        }

        // Return element rotated by 90° clockwise
        private Element GetElementRotatedClockWise()
        {
            var rotatedShape = string.Empty;

            for (var i = 0; i < width; i++)
            {
                for (var j = height - 1; j >= 0; j--)
                {
                    rotatedShape += shape[j * width + i];
                }
            }

            return new Element(rotatedShape, height, width);
        }

        // Return element mirrored by vertical axis
        private string GetBodyMirroredByHeight()
        {
            var mirroredShape = string.Empty;

            for (var i = 0; i < height; i++)
            {
                for (var j = width - 1; j >= 0; j--)
                {
                    mirroredShape += shape[i * width + j];
                }
            }

            return mirroredShape;
        }
    }
}
