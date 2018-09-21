using System;

[Serializable]
public class PackData
{
	public readonly string[] puzzles;
	public readonly int[] puzzlesWidths;
	public readonly int[] puzzlesElementsCounts;

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

	public static explicit operator PackData(string rawPackText) =>
        new PackData(new Pack(rawPackText));
}
