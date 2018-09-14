using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    private readonly string packsSaveDirectoryPath = $"Packs Data";
    private readonly string gameProgressFileName = "save";

    public int currentPack { get; private set; }
    public int currentLevel { get; private set; }
    public string savedPartition { get; private set; }

    private List<Pack> packs;

    private void Start()
    {
        List<PackData> packData = LoadPacksData();
        ProgressData progressData = LoadGameProgress();
        packs = AssemblePacks(packData, progressData);
    }

    private List<PackData> LoadPacksData()
    {
        var packsData = new List<PackData>();

        var directoryPath = $"{Application.persistentDataPath}/{packsSaveDirectoryPath}";

        if (Directory.Exists(directoryPath))
        {
            var packPathes = Directory.GetFiles(directoryPath);
            Debug.Log(packPathes);

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
        if (File.Exists($"{Application.persistentDataPath}/{gameProgressFileName}"))
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream file = File.Open($"{Application.persistentDataPath}/{gameProgressFileName}", FileMode.Open);
            ProgressData progressData = (ProgressData)bf.Deserialize(file);
            file.Close();

            return progressData;
        }
        else
        {
            return InitGameProgress();
        }
    }

    private ProgressData InitGameProgress()
    {
        var filePath = $"{Application.persistentDataPath}/{gameProgressFileName}";

        currentPack = 0;
        currentLevel = 0;
        savedPartition = "";

        var packProgress = 'o' + new String('c', packs.Count - 1);
        var puzzleProgress = new string[packs.Count];

        puzzleProgress[0] = 'o' + new String('c', packs[0].size - 1);
        for (int i = 1; i < packs.Count; i++)
        {
            puzzleProgress[i] += '\n' + new String('c', packs[i].size);
        }

        return new ProgressData(currentPack, currentLevel, savedPartition, packProgress, puzzleProgress);
    }

    private List<Pack> AssemblePacks(List<PackData> packData, ProgressData progressData)
    {

    }

    private void SaveGameProgress()
    {
        var filePath = $"{Application.persistentDataPath}/{gameProgressFileName}";

        var packProgress = "";
        var puzzleProgress = new string[packs.Count];

        for (var i = 0; i < packs.Count; i++)
        {
            packProgress += packs[i].solved ? 's' : packs[i].opened ? 'o' : 'c';

            foreach (var puzzle in packs[i])
            {
                puzzleProgress[i] += puzzle.solved ? 's' : puzzle.opened ? 'o' : 'c';
            }
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(filePath, FileMode.Create);
        bf.Serialize(file, new ProgressData(
            currentPack, currentLevel, savedPartition, packProgress, puzzleProgress));
        file.Close();
    }
}
