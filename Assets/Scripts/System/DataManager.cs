﻿using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private const string saveDirectoryPath = "Packs Data";
    private const string gameProgressFileName = "save";

    private List<Pack> packs;

    public int currentPackIndex { get; private set; }
    public int currentPuzzleIndex { get; private set; }

    public Pack currentPack
    {
        get
        {
            return packs[currentPackIndex];
        }
    }
    public Puzzle currentPuzzle
    {
        get
        {
            return packs[currentPackIndex][currentPuzzleIndex];
        }
    }

    public void SaveGameProgress(ProgressData progressData)
    {
        var filePath = $"{Application.persistentDataPath}/{gameProgressFileName}";

        FileStream file = File.Open(filePath, FileMode.Create);
        new BinaryFormatter().Serialize(file, progressData);
        file.Close();
    }

    private void Start()
    {
        List<PackData> packsData = LoadPacksData();
        ProgressData progressData =
            (File.Exists($"{Application.persistentDataPath}/{gameProgressFileName}")) ?
            LoadGameProgress() : InitGameProgress(packsData);
        packs = AssemblePacks(packsData, progressData);

        currentPackIndex = progressData.currentPackIndex;
        currentPuzzleIndex = progressData.currentPuzzleIndex;
        packs[currentPackIndex][currentPuzzleIndex].partition = progressData.savedPartition;
    }

    private List<PackData> LoadPacksData()
    {
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
                var packsData = new List<PackData>();
                BinaryFormatter bf = new BinaryFormatter();

                foreach (var packPath in packPathes)
                {
                    FileStream file = File.Open(packPath, FileMode.Open);

                    PackData packData = (PackData)bf.Deserialize(file);
                    file.Close();

                    packsData.Add(packData);
                }

                return packsData;
            }
            else
            {
                Debug.LogError("Missing packs data!");
            }
        }
        else
        {
            Debug.LogError("Missing packs data directory!");
        }

        return new List<PackData>();
    }

    private ProgressData LoadGameProgress()
    {
        FileStream file = File.Open(
            $"{Application.persistentDataPath}/{gameProgressFileName}", FileMode.Open);
        ProgressData progressData = (ProgressData)new BinaryFormatter().Deserialize(file);
        file.Close();

        return progressData;
    }

    // Initializes new game progress state and saves it to file
    private ProgressData InitGameProgress(List<PackData> packsData)
    {
        currentPackIndex = 0;
        currentPuzzleIndex = 0;

        var packsProgress = 'o' + new String('c', packsData.Count - 1);
        var puzzlesProgress = new string[packsData.Count];

        puzzlesProgress[0] = 'o' + new String('c', packsData[0].puzzles.Length - 1);
        for (var i = 1; i < packsData.Count; i++)
        {
            puzzlesProgress[i] += '\n' + new String('c', packsData[i].puzzles.Length);
        }

        var progressData = new ProgressData(currentPackIndex, currentPuzzleIndex,
            String.Empty, packsProgress, puzzlesProgress);
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
                    progressData.puzzlesProgress[i][j] == 'o' || progressData.puzzlesProgress[i][j] == 's',
                    progressData.puzzlesProgress[i][j] == 's');
            }

            packs.Add(new Pack(puzzles,
                progressData.packsProgress[i] == 'o' || progressData.packsProgress[i] == 's',
                progressData.packsProgress[i] == 's'));
        }

        return packs;
    }

    // Checks if progress data is valid corresponding to packs data 
    private bool CheckProgressDataIntegrity(List<PackData> packsData, ProgressData progressData)
    {
        if (packsData.Count != progressData.packsProgress.Length)
        {
            return false;
        }

        var validCharacters = new char[] { 's', 'o', 'c' };

        foreach (var c in progressData.packsProgress)
        {
            if (!validCharacters.Contains(c))
            {
                return false;
            }
        }

        for (var i = 0; i < packsData.Count; i++)
        {
            if (packsData[i].puzzles.Length != progressData.puzzlesProgress[i].Length)
            {
                return false;
            }

            foreach (var c in progressData.puzzlesProgress[i])
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
        var packsProgress = String.Empty;
        var puzzlesProgress = new string[packs.Count];

        for (var i = 0; i < packs.Count; i++)
        {
            packsProgress += packs[i].solved ? 's' : packs[i].opened ? 'o' : 'c';

            foreach (var puzzle in packs[i])
            {
                puzzlesProgress[i] += puzzle.solved ? 's' : puzzle.opened ? 'o' : 'c';
            }
        }

        return new ProgressData(
            currentPackIndex, currentPuzzleIndex,
            packs[currentPackIndex][currentPuzzleIndex].partition,
            packsProgress, puzzlesProgress);
    }
}