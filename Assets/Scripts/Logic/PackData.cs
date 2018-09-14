using System;

[Serializable]
public class PackData
{
	public readonly string[] puzzles;
	public readonly int[] puzzlesParts;
	public readonly int[] puzzlesWidth;

	public PackData(Pack pack)
	{
		puzzles = new string[pack.size];
		puzzlesParts = new int[pack.size];
		puzzlesWidth = new int[pack.size];

		for (var i = 0; i < pack.size; i++)
		{
			puzzles[i] = pack[i].partition;
			puzzlesParts[i] = pack[i].parts;
			puzzlesWidth[i] = pack[i].width;
		}
	}

	public static explicit operator PackData(string rawPackText)
    {
        return new PackData(new Pack(rawPackText));
    }
}
