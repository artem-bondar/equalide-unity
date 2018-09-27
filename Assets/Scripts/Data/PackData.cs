using System;

using Logic;

namespace Data
{
    [Serializable]
    public class PackData
    {
        private readonly string[] puzzles;
        private readonly int[] puzzlesWidths;
        private readonly int[] puzzlesElementsCounts;

        public int size
        {
            get
            {
                return puzzles.Length;
            }
        }

        public PackData(Pack pack)
        {
            puzzles = new string[pack.size];
            puzzlesWidths = new int[pack.size];
            puzzlesElementsCounts = new int[pack.size];

            for (var i = 0; i < pack.size; i++)
            {
                puzzles[i] = pack[i].partition;
                puzzlesWidths[i] = pack[i].width;
                puzzlesElementsCounts[i] = pack[i].elementsCount;
            }
        }

        public string Puzzle(int index) =>
            index >= 0 && index < puzzles.Length ? puzzles[index] : string.Empty;

        public int PuzzleWidth(int index) =>
            index >= 0 && index < puzzlesWidths.Length ? puzzlesWidths[index] : 0;

        public int PuzzleElementCount(int index) =>
            index >= 0 && index < puzzlesElementsCounts.Length ? puzzlesElementsCounts[index] : 0;

        public static explicit operator PackData(string rawPackText) =>
            new PackData(new Pack(rawPackText));
    }
}
