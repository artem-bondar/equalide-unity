using System;
using System.Collections.Generic;

namespace Logic
{
    // Contains rectangle grid that represent a round glued tape.
    public class TapeGrid : CellGrid
    {
		private Element element;

        public override string cells
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

        public TapeGrid() { }

        // Indexer interface to get/set cell using [,] operator
        public override char this[int i, int j]
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

        public bool CheckIfSolved()
        {
			return false;
        }

		// Find marked element, cut it from grid and
		// return array of indexes for it's cells
		public List<Tuple<int, int>> CutElement()
		{
			return new List<Tuple<int, int>>();
		}

        public void MoveRowFromBottomToTop()
        {
			string body = Cells.Substring(0, Cells.Length - width);
			string lastRow = Cells.Substring(Cells.Length - width - 1, width);
			Cells = lastRow + body;
		}
    }
}
