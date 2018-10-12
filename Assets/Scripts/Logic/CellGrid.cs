using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Logic
{
    // Contains square grid.
    // Each cell is represented as simple char.
    public class CellGrid : IEnumerable<char>
    {
        // Holds string representation of 2D-array
        protected string Cells;
        public string cells
        {
            get { return Cells; }
            set
            {
                if (value.Length == cells.Length)
                {
                    Cells = value;
                }
            }
        }

        // Dimensions in cells
        public readonly int width;
        public readonly int height;

        public CellGrid(string cells, int width, int height)
        {
            this.Cells = cells;
            this.width = width;
            this.height = height;
        }

        protected CellGrid() {}
        
        protected CellGrid(string cells)
        {
            this.Cells = cells;
        }

        protected CellGrid(string cells, int height)
        {
            this.Cells = cells;
            this.height = height;
        }

        // Indexer interface to get/set cell using [,] operator
        virtual public char this[int i, int j]
        {
            get
            {
                return CheckIfValidIndexes(i, j) ? Cells[i * width + j] : 'b';
            }
            set
            {
                if (CheckIfValidIndexes(i, j))
                {
                    char[] charArray = Cells.ToCharArray();
                    charArray[i * width + j] = value;
                    Cells = new string(charArray);
                }
            }
        }

        public IEnumerator<char> GetEnumerator() => Cells.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Cells.GetEnumerator();

        private bool CheckIfValidIndexes(int i, int j) =>
            i >= 0 && i < height && j >= 0 && j < width;
    }
}
