using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public readonly string gameProgressFileName = "save";

	public int currentPack { get; private set; }
	public int currentLevel { get; private set; }
	public string savedPartition { get; private set; }

    private readonly Pack[] loadedPacks;

    private void Start() {
        LoadPacksData();
        LoadGameProgress();   
    }

    private void LoadPacksData()
    {
        
    }

    private void LoadGameProgress()
    {

    }

    private void InitGameProgress()
    {
        
    }

    private void SaveGameProgress()
    {
        var filePath = $"{Application.persistentDataPath}/{gameProgressFileName}";

        var packProgress = "";
        var puzzleProgress = new string[loadedPacks.Length];

        for (var i = 0; i < loadedPacks.Length; i++)
        {
            packProgress += loadedPacks[i].solved ? 's' : loadedPacks[i].opened ? 'o' : 'c';

            foreach (var puzzle in loadedPacks[i])
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
