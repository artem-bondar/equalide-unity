using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Logic;

namespace LogicTapeT
{
    // Contains rectangle grid that represent a round glued tape
    // with next encoding:
    // 'm' - marked cell
    // 'e' - empty cell, can be colored
    // 'b' - blank cell, can't be colored
    public class TapeGrid : CellGrid
    {
        private Element[] elements;

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

        public TapeGrid(string tape, int width, int height, Element element)
            : base(tape, width, height)
        {
            
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
                if (CheckIfValidIndexes(i, j))
                {
                    var newCells = Cells.ToCharArray();
                    newCells[i * width + j] = value;
                    Cells = new string(newCells);
                }
            }
        }

        // Return puzzle partition to initial unsolved state
        public void Refresh() => Cells = Regex.Replace(Cells, "[^be]", "e");

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
