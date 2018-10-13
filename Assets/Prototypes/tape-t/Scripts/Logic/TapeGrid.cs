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
        private readonly CellGrid[] elements = new CellGrid[8];

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

        public TapeGrid(string tape, int width, int height, CellGrid element)
            : base(tape, width, height)
        {
            var reflections = element.GetCellsReflections();

            for (var i = 0; i <= 3; i++)
            {
                elements[i * 2] =
                    new CellGrid(reflections[i * 2],
                    i % 2 == 0 ? width : height,
                    i % 2 == 0 ? height : width);

                elements[i * 2 + 1] =
                    new CellGrid(reflections[i * 2 + 1],
                    i % 2 == 0 ? width : height,
                    i % 2 == 0 ? height : width);
            }
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

        // Find only one marked element, cut it from grid and
        // return array of indexes for it's cells
        public List<Tuple<int, int>> CutElement()
        {
            foreach (var element in elements)
            {
                for (var i = 0; i < width - element.width; i++)
                {
                    for (var j = 0; j < height - element.height; j++)
                    {
                        if (CheckIfHasElementOnPosition(i, j, element))
                        {
                            return GetCellsCoordinatesOfElement(i, j, element);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }

            return new List<Tuple<int, int>>();
        }

        public void MoveRowFromBottomToTop()
        {
            string body = Cells.Substring(0, Cells.Length - width);
            string lastRow = Cells.Substring(Cells.Length - width - 1, width);
            Cells = lastRow + body;
        }

        // Check if element is in grid and top-left corner of it is positioned at (x, y) 
        private bool CheckIfHasElementOnPosition(int x, int y, CellGrid element)
        {
            for (var i = 0; i < element.width; i++)
            {
                for (var j = 0; j < element.height; j++)
                {
                    if (element[i, j] != 'b' && this[x + i, y + j] != 'm')
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // Return cells coordinates related to grid of element,
        // top-left corner of wich is positioned at (x, y)
        private List<Tuple<int, int>> GetCellsCoordinatesOfElement(int x, int y, CellGrid element)
        {
            var coordinates = new List<Tuple<int, int>>();

            for (var i = 0; i < element.width; i++)
            {
                for (var j = 0; j < element.height; j++)
                {
                    if (element[i, j] != 'b' && this[x + i, y + j] == 'm')
                    {
                        coordinates.Add(Tuple.Create(x + i, y + j));
                    }
                }
            }

            return coordinates;
        }
    }
}
