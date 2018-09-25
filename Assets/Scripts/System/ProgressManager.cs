﻿using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public enum ProgressState
{
    Closed = 'c',
    Opened = 'o',
    Solved = 's'
}

public class ProgressManager : MonoBehaviour
{
    private const string gameProgressFileName = "save";

    private PackList packList;

    private DataManager dataManager;

    // Current level related data
    public int currentPackIndex { get; private set; }
    public int currentPuzzleIndex { get; private set; }
    public Puzzle currentPuzzle
    {
        get
        {
            return dataManager.Pack(currentPackIndex)[currentPuzzleIndex];
        }
    }

    private ProgressState[] packsStates;
    private ProgressState[][] puzzlesStates;

    private string packsProgress
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

    private string[] puzzlesProgress
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

    private void Awake()
    {
        packList = GameObject.FindObjectOfType<PackList>();
        dataManager = GameObject.FindObjectOfType<DataManager>();
    }

    private void OnApplicationQuit() => SaveGame();

    public void LoadGame()
    {
        ProgressData progressData =
            (File.Exists($"{Application.persistentDataPath}/{gameProgressFileName}")) ?
            LoadGameProgressData() : dataManager.GetInitialProgressData();

        LoadGameProgress(progressData);
        currentPackIndex = progressData.currentPackIndex;
        currentPuzzleIndex = progressData.currentPuzzleIndex;
        dataManager.Pack(currentPackIndex)
            [currentPuzzleIndex].partition = progressData.savedPartition;

        packList.Create(packsProgress);
    }

    public void SaveGame() => SaveGameProgress(GetProgressData());

    public ProgressState PackState(int packIndex) => packsStates[packIndex];
    public ProgressState[] PackProgress(int packIndex) => puzzlesStates[packIndex];
    public ProgressState PuzzleState(int packIndex, int puzzleIndex) => puzzlesStates[packIndex][puzzleIndex];

    public void SetCurrentLevel(int packIndex, int puzzleIndex)
    {
        this.currentPackIndex = packIndex;
        this.currentPuzzleIndex = puzzleIndex;
    }

    public bool IsOnLastLevel() =>
        currentPackIndex == packsStates.Length - 1 &&
        currentPuzzleIndex == puzzlesStates[currentPackIndex].Length - 1;

    public void OpenNextLevel()
    {
        if (currentPuzzleIndex != puzzlesStates[currentPackIndex].Length - 1)
        {
            puzzlesStates[currentPackIndex][currentPuzzleIndex + 1] = ProgressState.Opened;
        }
        else if (currentPackIndex != packsStates.Length - 1)
        {
            packsStates[currentPackIndex + 1] = ProgressState.Opened;
            packList.UpdatePackIcon(currentPackIndex + 1);

            puzzlesStates[currentPackIndex + 1][0] = ProgressState.Opened;
        }
    }

    public void SelectNextLevel()
    {
        if (currentPuzzleIndex != puzzlesStates[currentPackIndex].Length - 1)
        {
            currentPuzzleIndex++;
        }
        else if (currentPackIndex != packsStates.Length - 1)
        {
            packsStates[currentPackIndex] = ProgressState.Solved;
            packList.UpdatePackIcon(currentPackIndex, true);

            currentPackIndex++;
            currentPuzzleIndex = 0;
        }
    }

    private ProgressData LoadGameProgressData()
    {
        FileStream file = File.Open(
            $"{Application.persistentDataPath}/{gameProgressFileName}", FileMode.Open);
        var progressData = (ProgressData)new BinaryFormatter().Deserialize(file);
        file.Close();

        return progressData;
    }

    private void LoadGameProgress(ProgressData progressData)
    {
        packsStates = new ProgressState[progressData.packsProgress.Length];
        puzzlesStates = new ProgressState[progressData.packsProgress.Length][];

        for (var i = 0; i < progressData.packsProgress.Length; i++)
        {
            packsStates[i] = (ProgressState)progressData.packsProgress[i];
            puzzlesStates[i] = new ProgressState[progressData.puzzlesProgress[i].Length];

            for (var j = 0; j < progressData.puzzlesProgress[i].Length; j++)
            {
                puzzlesStates[i][j] = (ProgressState)progressData.puzzlesProgress[i][j];
            }
        }
    }

    // TODO
    private ProgressData RepairProgressData(
        ProgressData brokenProgressData, ProgressData initialProgressData)
    {
        return dataManager.GetInitialProgressData();
    }

    private ProgressData GetProgressData() => new ProgressData(
            currentPackIndex, currentPuzzleIndex,
            dataManager.Pack(currentPackIndex)[currentPuzzleIndex].partition,
            packsProgress, puzzlesProgress);

    private void SaveGameProgress(ProgressData progressData)
    {
        var filePath = $"{Application.persistentDataPath}/{gameProgressFileName}";
        FileStream file = File.Open(filePath, FileMode.Create);

        new BinaryFormatter().Serialize(file, progressData);
        file.Close();
    }
}
