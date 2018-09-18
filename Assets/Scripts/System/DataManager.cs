using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private const string saveDirectoryPath = "Packs Data";
    private const string gameProgressFileName = "save";

    public int currentPack { get; private set; }
    public int currentPuzzle { get; private set; }

    private List<Pack> packs;

    private void Start()
    {
        List<PackData> packsData = LoadPacksData();
        ProgressData progressData =
            (File.Exists($"{Application.persistentDataPath}/{gameProgressFileName}")) ?
            LoadGameProgress() : InitGameProgress(packsData);
        packs = AssemblePacks(packsData, progressData);

        currentPack = progressData.currentPack;
        currentPuzzle = progressData.currentPuzzle;
        packs[currentPack][currentPuzzle].partition = progressData.savedPartition;
    }

    public Puzzle GetCurrentPuzzle()
    {
        return packs[currentPack][currentPuzzle];
    }

    private List<PackData> LoadPacksData()
    {
        var packsData = new List<PackData>();

        var directoryPath = $"{Application.persistentDataPath}/{saveDirectoryPath}";

        if (Directory.Exists(directoryPath))
        {
            // Force sorting because the order of the returned file names
            // is not guaranteed in Directory.GetFiles
            var packPathes = Directory
                .GetFiles(directoryPath)
                .OrderBy(f => f).ToArray();

            if (packPathes.Length > 0)
            {
                BinaryFormatter bf = new BinaryFormatter();

                foreach (var packPath in packPathes)
                {
                    FileStream file = File.Open(packPath, FileMode.Open);

                    PackData packData = (PackData)bf.Deserialize(file);
                    file.Close();
                    
                    packsData.Add(packData);
                }
            }
            else
            {
                Debug.Log("Missing packs data!");
            }
        }
        else
        {
            Debug.Log("Missing packs data directory!");
        }

        return packsData;
    }

    private ProgressData LoadGameProgress()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open($"{Application.persistentDataPath}/{gameProgressFileName}", FileMode.Open);

        ProgressData progressData = (ProgressData)bf.Deserialize(file);
        file.Close();

        return progressData;
    }

    private ProgressData InitGameProgress(List<PackData> packsData)
    {
        currentPack = 0;
        currentPuzzle = 0;

        var packProgress = 'o' + new String('c', packsData.Count - 1);
        var puzzleProgress = new string[packsData.Count];

        puzzleProgress[0] = 'o' + new String('c', packsData[0].puzzles.Length - 1);
        for (int i = 1; i < packsData.Count; i++)
        {
            puzzleProgress[i] += '\n' + new String('c', packsData[i].puzzles.Length);
        }

        var progressData = new ProgressData(currentPack, currentPuzzle, String.Empty, packProgress, puzzleProgress);
        SaveGameProgress(progressData);

        return progressData;
    }

    private List<Pack> AssemblePacks(List<PackData> packsData, ProgressData progressData)
    {
        var packs = new List<Pack>();

        if (!CheckProgressDataIntegrity(packsData, progressData))
        {
            progressData = RepairProgressData(packsData, progressData);
        }

        for (var i = 0; i < packsData.Count; i++)
        {
            var puzzles = new Puzzle[packsData[i].puzzles.Length];

            for (var j = 0; j < packsData[i].puzzles.Length; j++)
            {
                puzzles[j] = new Puzzle(
                    packsData[i].puzzles[j], packsData[i].puzzlesParts[j],
                    packsData[i].puzzlesWidth[j], packsData[i].puzzles[j].Length / packsData[i].puzzlesWidth[j],
                    progressData.puzzleProgress[i][j] == 'o' || progressData.puzzleProgress[i][j] == 's',
                    progressData.puzzleProgress[i][j] == 's');
            }

            packs.Add(new Pack(puzzles,
                progressData.packProgress[i] == 'o' || progressData.packProgress[i] == 's',
                progressData.packProgress[i] == 's'));
        }

        return packs;
    }

    // Checks if progress data is valid corresponding to packs data 
    private bool CheckProgressDataIntegrity(List<PackData> packsData, ProgressData progressData)
    {
        if (packsData.Count != progressData.packProgress.Length)
        {
            return false;
        }

        var validCharacters = new char[] { 's', 'o', 'c' };

        foreach (var c in progressData.packProgress)
        {
            if (!validCharacters.Contains(c))
            {
                return false;
            }
        }

        for (var i = 0; i < packsData.Count; i++)
        {
            if (packsData[i].puzzles.Length != progressData.puzzleProgress[i].Length)
            {
                return false;
            }

            foreach (var c in progressData.puzzleProgress[i])
            {
                if (!validCharacters.Contains(c))
                {
                    return false;
                }
            }
        }

        return true;
    }

    // TODO
    private ProgressData RepairProgressData(List<PackData> packsData, ProgressData progressData)
    {
        return InitGameProgress(packsData);
    }

    private ProgressData GetProgressData()
    {
        var packProgress = String.Empty;
        var puzzleProgress = new string[packs.Count];

        for (var i = 0; i < packs.Count; i++)
        {
            packProgress += packs[i].solved ? 's' : packs[i].opened ? 'o' : 'c';

            foreach (var puzzle in packs[i])
            {
                puzzleProgress[i] += puzzle.solved ? 's' : puzzle.opened ? 'o' : 'c';
            }
        }

        return new ProgressData(
            currentPack, currentPuzzle,
            packs[currentPack][currentPuzzle].partition,
            packProgress, puzzleProgress);
    }

    private void SaveGameProgress(ProgressData progressData)
    {
        var filePath = $"{Application.persistentDataPath}/{gameProgressFileName}";

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(filePath, FileMode.Create);

        bf.Serialize(file, progressData);
        file.Close();
    }
}
