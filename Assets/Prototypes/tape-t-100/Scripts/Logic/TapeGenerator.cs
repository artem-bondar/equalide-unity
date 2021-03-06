﻿using UnityEngine;

using Logic;
using LogicTapeT;

namespace LogicTapeT100
{
    public class TapeGenerator
    {
        public static TapeGrid GenerateGradientLinearTape(int width, CellGrid element)
        {
            var tapeCells = string.Empty;

            for (var i = 0; i < 300; i++)
            {
                for (var j = 0; j < width; j++)
                {
					tapeCells += Random.value <= 0.75f ? 'e' : 'b';
                }
            }

            for (var probability = 0.75f; probability <= 1f; probability += 0.005f)
            {
                for (var i = 0; i < width; i++)
                {
					tapeCells += Random.value <= probability ? 'e' : 'b';
                }
            }

			return new TapeGrid(tapeCells, width, tapeCells.Length / width, element);
        }
    }
}
