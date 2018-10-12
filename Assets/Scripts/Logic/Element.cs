using System.Linq;
using System.Collections.Generic;

namespace Logic
{
    // Contains element with next representation:
    // 'c' - non-empty cell
    // 'e' - empty cell
    public class Element : CellGrid
    {
        // Hide base class set accessor
        new public string cells
        {
            get { return Cells; }
            private set { Cells = value; }
        }

        // Dimension in cells
        new public int width { get; private set; }

        // Receives string without '\n'
        public Element(string cells, int width) : base(cells, cells.Length / width)
        {
            this.width = width;
            CutByWidth();
        }

        // Assumed to be already cut by width
        private Element(string cells, int width, int height) : base(cells, height)
        {
            this.width = width;
        }

        public override bool Equals(object obj)
        {
            var element = obj as Element;

            return element == null ? false : this == element;
        }

        public override int GetHashCode() => Cells.GetHashCode();

        // Checks equality to another element with accuracy to rotations and reflections
        public static bool operator !=(Element first, Element second) => !(first == second);
        public static bool operator ==(Element first, Element second)
        {
            if ((first.width != second.width || first.height != second.height) &&
                (first.height != second.width || first.width != second.height))
            {
                return false;
            }

            if (first.Cells != second.Cells && first.GetCellsMirroredByHeight() != second.Cells)
            {
                Element elementForRotate = first;

                for (var i = 0; i < 3; i++)
                {
                    Element rotatedElement = elementForRotate.GetElementRotatedClockWise();

                    if (rotatedElement.Cells == second.Cells ||
                        rotatedElement.GetCellsMirroredByHeight() == second.Cells)
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
            var pendingIndexes = new HashSet<int> { Cells.IndexOf("c") };

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
                    int? down = (index + width < Cells.Length) ? index + width : (int?)null;
                    int? left = (index % width != 0) ? index - 1 : (int?)null;
                    int? right = (index % width != width - 1) ? index + 1 : (int?)null;

                    var indexesForCheck = new List<int?> { up, down, left, right };

                    foreach (var i in indexesForCheck)
                    {
                        if (((i != null) &&
                            (Cells[(int)i] == 'c')) && !(checkedIndexes.Contains((int)i)))
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
            for (var i = 0; i < Cells.Length; i++)
            {
                if ((Cells[i] == 'c') && !(checkedIndexes.Contains(i)))
                {
                    return false;
                }
            }

            return true;
        }

        // Cut element to it's bounding rectangle of the same height
        private void CutByWidth()
        {
            var startIndexes = new List<int>();
            var endIndexes = new List<int>();

            // Gets all starting and ending indexes of non-empty cells on every row
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    if (Cells[i * width + j] != 'e')
                    {
                        startIndexes.Add(j);
                        break;
                    }
                }

                for (var j = width - 1; j >= 0; j--)
                {
                    if (Cells[i * width + j] != 'e')
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
                var cutCells = string.Empty;

                for (var i = 0; i < height; i++)
                {
                    for (var j = start; j <= end; j++)
                    {
                        cutCells += Cells[i * width + j];
                    }
                }

                Cells = cutCells;
                width = end - start + 1;
            }
        }

        // Return element rotated by 90° clockwise
        private Element GetElementRotatedClockWise()
        {
            var rotatedCells = string.Empty;

            for (var i = 0; i < width; i++)
            {
                for (var j = height - 1; j >= 0; j--)
                {
                    rotatedCells += Cells[j * width + i];
                }
            }

            return new Element(rotatedCells, height, width);
        }

        // Return element mirrored by vertical axis
        private string GetCellsMirroredByHeight()
        {
            var mirroredCells = string.Empty;

            for (var i = 0; i < height; i++)
            {
                for (var j = width - 1; j >= 0; j--)
                {
                    mirroredCells += Cells[i * width + j];
                }
            }

            return mirroredCells;
        }
    }
}
