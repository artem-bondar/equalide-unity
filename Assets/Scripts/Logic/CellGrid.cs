using System.Collections;
using System.Collections.Generic;

namespace Logic
{
    // Contains rectangle grid.
    // Each cell is represented as a simple char.
    public class CellGrid : IEnumerable<char>
    {
        // Dimensions in cells
        public readonly int width;
        public readonly int height;

        // String representation of 2D-array
        protected string Cells;
        public virtual string cells
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

        public CellGrid(string cells, int width, int height)
        {
            this.Cells = cells;
            this.width = width;
            this.height = height;
        }

        protected CellGrid() { }

        protected CellGrid(string cells, int height)
        {
            this.Cells = cells;
            this.width = cells.Length / height;
            this.height = height;
        }

        // Indexer interface to get/set cell using [,] operator
        public virtual char this[int i, int j]
        {
            get
            {
                return CheckIfValidIndexes(i, j) ? Cells[i * width + j] : 'b';
            }
            set
            {
                if (CheckIfValidIndexes(i, j))
                {
                    var newCells = Cells.ToCharArray();
                    newCells[i * width + j] = value;
                    Cells = new string(newCells);
                }
            }
        }

        public override string ToString()
        {
            var result = string.Empty;

            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    result += this[i, j];
                }

                result += '\n';
            }

            return result;
        }

        public IEnumerator<char> GetEnumerator() => Cells.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Cells.GetEnumerator();

        public override bool Equals(object obj)
        {
            var element = obj as CellGrid;

            return element == null ? false : this == element;
        }

        public override int GetHashCode() => Cells.GetHashCode();

        // Checks equality to another grid with accuracy to rotations and reflections
        public static bool operator !=(CellGrid first, CellGrid second) => !(first == second);
        public static bool operator ==(CellGrid first, CellGrid second)
        {
            if ((first.width != second.width || first.height != second.height) &&
                (first.height != second.width || first.width != second.height))
            {
                return false;
            }

            foreach (var cells in first.GetCellsReflections())
            {
                if (cells == second.cells)
                {
                    return true;
                }
            }

            return false;
        }

        // Return cells rotated by 90° clockwise
        public static string RotateCellsClockWise(string cells, int width, int height)
        {
            var rotatedCells = string.Empty;

            for (var i = 0; i < width; i++)
            {
                for (var j = height - 1; j >= 0; j--)
                {
                    rotatedCells += cells[j * width + i];
                }
            }

            return rotatedCells;
        }

        // Return cells mirrored by vertical axis
        public static string MirrorCellsByHeight(string cells, int width, int height)
        {
            var mirroredCells = string.Empty;

            for (var i = 0; i < height; i++)
            {
                for (var j = width - 1; j >= 0; j--)
                {
                    mirroredCells += cells[i * width + j];
                }
            }

            return mirroredCells;
        }

        // Return all 8 cells reflections: 4 rotations + 4 mirrored rotations
        public static string[] ReflectCells(string cells, int width, int height)
        {
            var reflections = new string[8];
            reflections[0] = cells;
            reflections[1] = MirrorCellsByHeight(cells, width, height);

            for (var i = 1; i <= 3; i++)
            {
                reflections[i * 2] =
                    RotateCellsClockWise(reflections[(i - 1) * 2],
                    i % 2 == 1 ? width : height,
                    i % 2 == 1 ? height : width);

                reflections[i * 2 + 1] =
                    MirrorCellsByHeight(reflections[i * 2],
                    i % 2 == 1 ? height : width,
                    i % 2 == 1 ? width : height);
            }

            return reflections;
        }

        public string GetCellsRotatedClockWise() => RotateCellsClockWise(Cells, width, height);
        public string GetCellsMirroredByHeight() => MirrorCellsByHeight(Cells, width, height);
        public string[] GetCellsReflections() => ReflectCells(Cells, width, height);

        protected bool CheckIfValidIndexes(int i, int j) =>
            i >= 0 && i < height && j >= 0 && j < width;
    }
}
