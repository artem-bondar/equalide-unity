[System.Serializable]
public class ProgressData
{
	// Current level related data
	public int currentPack;
	public int currentLevel;
	public string savedPartition;
	
	// Overall progress related data
	[System.NonSerialized]
	private int packAmount = 9;

	// Strings that represent packs's/levels's statuses,
	// one char for one entity with next alphabet:
	// 'c' - closed
	// 'o' - opened
	// 's' - solved

	public string packProgress;
	public string[] puzzleProgress;

	public ProgressData(int currentPack, int currentLevel, string savedPartition,
						string packProgress, string[] puzzleProgress)
	{
		this.currentPack = currentPack;
		this.currentLevel = currentLevel;
		this.savedPartition = savedPartition;
		this.packProgress = packProgress;
		this.puzzleProgress = puzzleProgress;
	}
}
