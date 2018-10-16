using UnityEngine;

using Logic;
using LogicTapeT;

namespace LogicTapeT100
{
    public class TapeGenerator
    {
        public static TapeGrid GenerateGradientLinearTape(int width, CellGrid element)
        {
            var tapeCells = string.Empty;

            for (var probability = 0.01f; probability <= 1f; probability += 0.01f)
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
