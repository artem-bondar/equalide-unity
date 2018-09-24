using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    private const string gameProgressFileName = "save";

    private PackList packList;
	
    public string packsProgress
    {
        get
        {
            var packsProgress = string.Empty;

            foreach (var pack in packs)
            {
                packsProgress += pack.solved ? 's' : pack.opened ? 'o' : 'c';
            }

            return packsProgress;
        }
    }

    public int currentPackIndex { get; private set; }
    public int currentPuzzleIndex { get; private set; }

    public ProgressData gameProgress
    {
        get
        {
            var puzzlesProgress = new string[packs.Count];

            for (var i = 0; i < packs.Count; i++)
            {
                puzzlesProgress[i] = packs[i].puzzlesProgress;
            }

            return new ProgressData(
                currentPackIndex, currentPuzzleIndex,
                packs[currentPackIndex][currentPuzzleIndex].partition,
                packsProgress, puzzlesProgress);
        }
    }

    private void Start()
    {
        ProgressData progressData =
            (File.Exists($"{Application.persistentDataPath}/{gameProgressFileName}")) ?
            LoadGameProgress() : InitGameProgress(packsData);

        currentPackIndex = progressData.currentPackIndex;
        currentPuzzleIndex = progressData.currentPuzzleIndex;
        packs[currentPackIndex][currentPuzzleIndex].partition = progressData.savedPartition;

        packList = GameObject.FindObjectOfType<PackList>();
        packList.Create(packsProgress);
    }

    private void OnApplicationQuit() => SaveGameProgress(gameProgress);

    public void SetCurrentLevel(int currentPackIndex, int currentPuzzleIndex)
    {
        this.currentPackIndex = currentPackIndex;
        this.currentPuzzleIndex = currentPuzzleIndex;
    }

    public void SaveGameProgress(ProgressData progressData)
    {
        var filePath = $"{Application.persistentDataPath}/{gameProgressFileName}";

        FileStream file = File.Open(filePath, FileMode.Create);
        new BinaryFormatter().Serialize(file, progressData);
        file.Close();
    }

    public bool IsOnLastLevel() => currentPackIndex == packs.Count - 1 &&
                                   currentPuzzleIndex == packs[currentPackIndex].size - 1;

    public void OpenNextLevel()
    {
        if (currentPuzzleIndex != packs[currentPackIndex].size - 1)
        {
            packs[currentPackIndex][currentPuzzleIndex + 1].opened = true;
        }
        else if (currentPackIndex != packs.Count - 1)
        {
            packs[currentPackIndex + 1].opened = true;
            packList.UpdatePackIcon(currentPackIndex + 1);

            packs[currentPackIndex + 1][0].opened = true;
        }
    }

    public void SelectNextLevel()
    {
        if (currentPuzzleIndex != packs[currentPackIndex].size - 1)
        {
            currentPuzzleIndex++;
        }
        else if (currentPackIndex != packs.Count - 1)
        {
            packs[currentPackIndex].solved = true;
            packList.UpdatePackIcon(currentPackIndex, true);

            currentPackIndex++;
            currentPuzzleIndex = 0;
        }
    }

    private ProgressData LoadGameProgress()
    {
        FileStream file = File.Open(
            $"{Application.persistentDataPath}/{gameProgressFileName}", FileMode.Open);
        var progressData = (ProgressData)new BinaryFormatter().Deserialize(file);
        file.Close();

        return progressData;
    }

    // Initializes new game progress state and saves it to file
    private ProgressData InitGameProgress(List<PackData> packsData)
    {
        currentPackIndex = 0;
        currentPuzzleIndex = 0;

        var packsProgress = 'o' + new string('c', packsData.Count - 1);
        var puzzlesProgress = new string[packsData.Count];

        puzzlesProgress[0] = 'o' + new string('c', packsData[0].puzzles.Length - 1);
        for (var i = 1; i < packsData.Count; i++)
        {
            puzzlesProgress[i] += '\n' + new string('c', packsData[i].puzzles.Length);
        }

        var progressData = new ProgressData(currentPackIndex, currentPuzzleIndex,
            string.Empty, packsProgress, puzzlesProgress);
        SaveGameProgress(progressData);

        return progressData;
    }
}
