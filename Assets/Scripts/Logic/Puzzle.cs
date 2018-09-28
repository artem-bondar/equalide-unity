using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Logic
{
    // Contains puzzle with next representation:
    // '0-9' - colored cell
    // 'e' - empty cell, can be colored
    // 'b' - blank cell, can't be colored
    public class Puzzle : IEnumerable<char>
    {
        // Holds string representation of 2D-array
        private string Partition;
        public string partition
        {
            get { return Partition; }
            set
            {
                if (CheckIfValidPartition(value))
                {
                    Partition = value;
                }
            }
        }

        public readonly int elementsCount;

        // Dimensions in cells
        public readonly int width;
        public readonly int height;

        public Puzzle(string partition, int elementsCount,
                      int width, int height)
        {
            this.Partition = partition;
            this.elementsCount = elementsCount;
            this.width = width;
            this.height = height;
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
        public Puzzle(string rawPuzzleText)
        {
            var unicalCells = new HashSet<char>(rawPuzzleText.ToCharArray());
            unicalCells.RemoveWhere(c => c == 'b' || c == 'e' || c == '\n');

            string[] lines = rawPuzzleText.Split('\n');

            this.Partition = string.Join("", lines);
            Refresh();

            this.elementsCount = unicalCells.Count;
            this.width = lines[0].Length;
            this.height = lines.Length;
        }

        // Indexer interface to get/set char using [,] operator
        public char this[int i, int j]
        {
            get
            {
                return CheckIfValidIndexes(i, j) ? Partition[i * width + j] : 'b';
            }
            set
            {
                if (CheckIfValidIndexes(i, j))
                {
                    char[] charArray = Partition.ToCharArray();
                    charArray[i * width + j] = value;
                    Partition = new string(charArray);
                }
            }
        }

        public IEnumerator<char> GetEnumerator() => partition.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => partition.GetEnumerator();

        // Return puzzle partition to initial unsolved state
        public void Refresh() => Partition = Regex.Replace(Partition, "[^be]", "e");

        // Checks if current puzzle partition is a valid solution
        public bool CheckForSolution()
        {
            // Checks if puzzle contains any unpainted primitive
            if (Partition.IndexOf('e') != -1)
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

        private bool CheckIfValidIndexes(int i, int j) =>
            i >= 0 && i < height && j >= 0 && j < width;

        // Checks if partition for loading has the same shape
        // and contains only valid cells
        private bool CheckIfValidPartition(string partition)
        {
            if (Partition.Length != partition.Length)
            {
                return false;
            }

            for (var i = 0; i < partition.Length; i++)
            {
                if ((Partition[i] == 'b' && partition[i] != 'b') ||
                    (Partition[i] != 'b' && partition[i] == 'b') ||
                    (!char.IsDigit(partition[i]) && partition[i] != 'b' && partition[i] != 'e') ||
                    (char.IsDigit(partition[i]) && partition[i] - '0' >= this.elementsCount))
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

            var unicalCells = new HashSet<char>(Partition.ToCharArray());
            unicalCells.RemoveWhere(c => c == 'b' || c == 'e');

            foreach (var cell in unicalCells)
            {
                int firstOccurance = Partition.IndexOf(cell);
                int lastOccurance = Partition.LastIndexOf(cell);

                // Get partition that represent element cut by height bounds
                string substr = Partition.Substring(
                    firstOccurance - firstOccurance % width,
                    (lastOccurance / width - firstOccurance / width + 1) * width);

                result.Add(new Element(
                    Regex.Replace(substr, string.Format("[^{0}]", cell), "e")
                    .Replace(cell, 'c'),
                    width));
            }

            return result;
        }
    }
}
