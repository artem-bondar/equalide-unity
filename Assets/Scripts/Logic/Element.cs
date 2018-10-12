using System.Linq;
using System.Collections.Generic;

namespace Logic
{
    // Contains immutable element with next representation:
    // 'c' - non-empty cell
    // 'e' - empty cell
    public class Element : CellGrid
    {
        // Remove base class set accessor
        public override string cells
        {
            get { return Cells; }
            set { }
        }

        // Receives string without '\n'
        public Element(string cells, int height) : base(CutByWidth(cells, height), height) { }

        // Assumed to be already cut by width
        private Element(string cells, int width, int height) : base(cells, width, height) { }

        // Remove base class set accessor
        public override char this[int i, int j]
        {
            get
            {
                return CheckIfValidIndexes(i, j) ? Cells[i * width + j] : 'c';
            }
            set { }
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
        private static string CutByWidth(string cells, int height)
        {
            var width = cells.Length / height;
            var startIndexes = new List<int>();
            var endIndexes = new List<int>();

            // Gets all starting and ending indexes of non-empty cells on every row
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    if (cells[i * width + j] != 'e')
                    {
                        startIndexes.Add(j);
                        break;
                    }
                }

                for (var j = width - 1; j >= 0; j--)
                {
                    if (cells[i * width + j] != 'e')
                    {
                        endIndexes.Add(j);
                        break;
                    }
                }
            }

            if (startIndexes.Count != 0 || endIndexes.Count == 0)
            {
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
                            cutCells += cells[i * width + j];
                        }
                    }

                    return cutCells;
                }
            }

            return cells;
        }
    }
}
