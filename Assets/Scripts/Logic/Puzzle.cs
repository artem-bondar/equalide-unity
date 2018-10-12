using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Logic
{
    // Contains puzzle with next representation:
    // '0-9' - colored cell
    // 'e' - empty cell, can be colored
    // 'b' - blank cell, can't be colored
    public class Puzzle : CellGrid
    {
        public readonly int elementsCount;

        public override string cells
        {
            get { return Cells; }
            set
            {
                if (CheckIfValidPartition(value))
                {
                    Cells = value;
                }
            }
        }

        public Puzzle(string cells, int elementsCount,
                      int width, int height) : base(cells, width, height)
        {
            this.elementsCount = elementsCount;
        }

        // Receives puzzle with solution in simple text format:
        // '\n' between lines (no '\n' for last line)
        // '0-9' - colored cell
        // 'b' - blank cell, can't be colored
        //
        // Example of input text:
        // "b10\n10b"
        // ---
        // b10
        // 10b
        // ---
        public static explicit operator Puzzle(string rawPuzzleText)
        {
            var unicalCells = new HashSet<char>(rawPuzzleText.ToCharArray());
            unicalCells.RemoveWhere(c => c == 'b' || c == 'e' || c == '\n');

            string[] lines = rawPuzzleText.Split('\n');

            Puzzle puzzle = new Puzzle(
                string.Join("", lines), unicalCells.Count,
                lines[0].Length, lines.Length);

            puzzle.Refresh();

            return puzzle;
        }

        // Indexer interface to get/set cell using [,] operator
        public override char this[int i, int j]
        {
            get
            {
                return CheckIfValidIndexes(i, j) ? Cells[i * width + j] : 'b';
            }
            set
            {
                if (CheckIfValidIndexes(i, j) &&
                    CheckIfValidCell(value) && Cells[i * width + j] != 'b')
                {
                    var newCells = Cells.ToCharArray();
                    newCells[i * width + j] = value;
                    Cells = new string(newCells);
                }
            }
        }

        // Return puzzle partition to initial unsolved state
        public void Refresh() => Cells = Regex.Replace(Cells, "[^be]", "e");

        // Checks if current puzzle partition is a valid solution
        public bool CheckForSolution()
        {
            // Checks if puzzle contains any unpainted primitive
            if (Cells.IndexOf('e') != -1)
            {
                return false;
            }

            List<Element> elements = SeparateInElements();

            if (elements.Count != elementsCount)
            {
                return false;
            }

            if (!elements[0].CheckConnectivity())
            {
                return false;
            }

            // Checks if all elements are equal
            for (var i = 1; i < elements.Count; i++)
            {
                if (elements[0] != elements[i])
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckIfValidCell(char cell) =>
            (!char.IsDigit(cell) && (cell == 'b' || cell == 'e')) ||
            (char.IsDigit(cell) && cell - '0' < this.elementsCount);

        // Checks if partition for loading has the same shape
        // and contains only valid cells
        private bool CheckIfValidPartition(string partition)
        {
            if (Cells.Length != partition.Length)
            {
                return false;
            }

            for (var i = 0; i < partition.Length; i++)
            {
                if ((Cells[i] == 'b' && partition[i] != 'b') ||
                    (Cells[i] != 'b' && partition[i] == 'b') ||
                    !CheckIfValidCell(partition[i]))
                {
                    return false;
                }
            }

            return true;
        }

        // Separates current partition in elements of same color
        private List<Element> SeparateInElements()
        {
            var result = new List<Element>();

            var unicalCells = new HashSet<char>(Cells.ToCharArray());
            unicalCells.RemoveWhere(c => c == 'b' || c == 'e');

            foreach (var cell in unicalCells)
            {
                int firstOccurance = Cells.IndexOf(cell);
                int lastOccurance = Cells.LastIndexOf(cell);
                var elementHeight = lastOccurance / width - firstOccurance / width + 1;

                // Get partition that represent element cut by height bounds
                string substr = Cells.Substring(
                    firstOccurance - firstOccurance % width,
                    elementHeight * width);

                result.Add(new Element(
                    Regex.Replace(substr, string.Format("[^{0}]", cell), "e")
                         .Replace(cell, 'c'),
                    elementHeight));
            }

            return result;
        }
    }
}
