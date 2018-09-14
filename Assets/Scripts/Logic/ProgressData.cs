using System;

[Serializable]
public class ProgressData
{
	// Current level related data
	public readonly int currentPack;
	public readonly int currentPuzzle;
	public readonly string savedPartition;

	// Strings that represent packs's/levels's statuses,
	// one char for one entity with next alphabet:
	// 'c' - closed
	// 'o' - opened
	// 's' - solved
	public readonly string packProgress;

	// Contains one progress string per each pack
	public readonly string[] puzzleProgress;

	public ProgressData(int currentPack, int currentPuzzle, string savedPartition,
						string packProgress, string[] puzzleProgress)
	{
		this.currentPack = currentPack;
		this.currentPuzzle = currentPuzzle;
		this.savedPartition = savedPartition;
		this.packProgress = packProgress;
		this.puzzleProgress = puzzleProgress;
	}
}
