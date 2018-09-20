using System;

[Serializable]
public class ProgressData
{
	// Current level related data
	public readonly int currentPackIndex;
	public readonly int currentPuzzleIndex;
	public readonly string savedPartition;

	// Strings that represent packs's/levels's states,
	// one char for one entity with next alphabet:
	// 'c' - closed
	// 'o' - opened
	// 's' - solved
	public readonly string packsProgress;

	// Contains one puzzles progress string per each pack
	public readonly string[] puzzlesProgress;

	public ProgressData(int currentPackIndex, int currentPuzzleIndex, string savedPartition,
						string packsProgress, string[] puzzlesProgress)
	{
		this.currentPackIndex = currentPackIndex;
		this.currentPuzzleIndex = currentPuzzleIndex;
		this.savedPartition = savedPartition;
		this.packsProgress = packsProgress;
		this.puzzlesProgress = puzzlesProgress;
	}
}
