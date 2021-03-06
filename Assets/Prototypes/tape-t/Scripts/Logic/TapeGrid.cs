﻿using System;
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
                    i % 2 == 0 ? element.width : element.height,
                    i % 2 == 0 ? element.height : element.width);

                elements[i * 2 + 1] =
                    new CellGrid(reflections[i * 2 + 1],
                    i % 2 == 0 ? element.width : element.height,
                    i % 2 == 0 ? element.height : element.width);
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

        public bool CheckIfSolved() => FindElement(false).Item1 == -1;

        // Find only one marked element, cut it from grid and
        // return array of indexes for it's cells
        public List<Tuple<int, int>> CutElementIfPossible()
        {
            Tuple<int, int, CellGrid> position = FindElement(true);

            if (position.Item1 != -1)
            {
                List<Tuple<int, int>> coordinates = GetCellsCoordinatesOfElement(
                    position.Item1, position.Item2, position.Item3);

                foreach (var coordinate in coordinates)
                {
                    this[coordinate.Item1, coordinate.Item2] = 'b';
                }

                return coordinates;
            }

            return new List<Tuple<int, int>>();
        }

        public void MoveRowFromBottomToTop()
        {
            string body = Cells.Substring(0, Cells.Length - width);
            string lastRow = Cells.Substring(Cells.Length - width, width);
            Cells = lastRow + body;
        }

        // Find only one marked element,
        // return position of it's upper-left corner and corresponding element
        private Tuple<int, int, CellGrid> FindElement(bool marked)
        {
            foreach (var element in elements)
            {
                for (var i = 0; i < height - element.height + 1; i++)
                {
                    for (var j = 0; j < width - element.width + 1; j++)
                    {
                        if (CheckIfHasElementOnPosition(i, j, element, marked))
                        {
                            return Tuple.Create(i, j, element);
                        }
                    }
                }
            }

            return Tuple.Create(-1, -1, new CellGrid(string.Empty, 0, 0));
        }

        // Check if grid has element and it's upper-left corner is positioned at (x, y) 
        private bool CheckIfHasElementOnPosition(int x, int y, CellGrid element, bool marked)
        {
            for (var i = 0; i < element.height; i++)
            {
                for (var j = 0; j < element.width; j++)
                {
                    if (element[i, j] != 'b' &&
                       ((marked && this[x + i, y + j] != 'm') ||
                       (!marked && this[x + i, y + j] == 'b')))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // Return cells coordinates related to grid of element,
        // upper-left corner of wich is positioned at (x, y)
        private List<Tuple<int, int>> GetCellsCoordinatesOfElement(int x, int y, CellGrid element)
        {
            var coordinates = new List<Tuple<int, int>>();

            for (var i = 0; i < element.height; i++)
            {
                for (var j = 0; j < element.width; j++)
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
