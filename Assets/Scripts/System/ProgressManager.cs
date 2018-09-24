using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

enum ProgressState
{
    Closed,
    Opened,
    Solved
}

public class ProgressManager : MonoBehaviour
{
    // Current level related data
    public int currentPackIndex { get; private set; }
    public int currentPuzzleIndex { get; private set; }

    private readonly ProgressState[] packsStates;
    private readonly ProgressState[][] puzzlesStates;

    private const string gameProgressFileName = "save";

    private PackList packList;

    private DataManager dataManager;

    public string packsProgress
    {
        get
        {
            var packsProgress = string.Empty;

            foreach (var packState in packsStates)
            {
                packsProgress += packState == ProgressState.Closed ? 'c' :
                    packState == ProgressState.Opened ? 'o' : 's';
            }

            return packsProgress;
        }
    }

    public string[] puzzlesProgress
    {
        get
        {
            var puzzlesProgress = new string[puzzlesStates.Length];

            for (var i = 0; i < puzzlesStates.Length; i++)
            {
                foreach (var puzzleState in puzzlesStates[i])
                {
                    puzzlesProgress[i] += puzzleState == ProgressState.Closed ? 'c' :
                        puzzleState == ProgressState.Opened ? 'o' : 's';
                }
            }

            return puzzlesProgress;
        }
    }

    // public ProgressData gameProgress
    // {
    //     get
    //     {
    //         var puzzlesProgress = new string[packs.Count];

    //         for (var i = 0; i < packs.Count; i++)
    //         {
    //             puzzlesProgress[i] = packs[i].puzzlesProgress;
    //         }

    //         return new ProgressData(
    //             currentPackIndex, currentPuzzleIndex,
    //             packs[currentPackIndex][currentPuzzleIndex].partition,
    //             packsProgress, puzzlesProgress);
    //     }
    // }

    private void Start()
    {
        dataManager = GameObject.FindObjectOfType<DataManager>();

        ProgressData progressData =
            (File.Exists($"{Application.persistentDataPath}/{gameProgressFileName}")) ?
            LoadGameProgress() : dataManager.GetInitialProgressData();

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

    // TODO
    private ProgressData RepairProgressData(List<PackData> packsData, ProgressData progressData)
    {
        return InitGameProgress(packsData);
    }
}
