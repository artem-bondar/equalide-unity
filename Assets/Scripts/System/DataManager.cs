using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private const string saveDirectoryPath = "Packs Data";

    private List<Pack> packs;

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

    private void Start()
    {
        List<PackData> packsData = LoadPacksData();

        packs = AssemblePacks(packsData, progressData);
    }

    public Pack GetPack(int packIndex) =>
        packIndex >= 0 && packIndex < packs.Count ? packs[packIndex] : null;

    private List<PackData> LoadPacksData()
    {
        var directoryPath = $"{Application.persistentDataPath}/{saveDirectoryPath}";

        if (Directory.Exists(directoryPath))
        {
            // Force sorting because the order of the returned file names
            // is not guaranteed in Directory.GetFiles
            string[] packPathes = Directory
                .GetFiles(directoryPath)
                .OrderBy(f => f).ToArray();

            if (packPathes.Length > 0)
            {
                var packsData = new List<PackData>();
                var bf = new BinaryFormatter();

                foreach (var packPath in packPathes)
                {
                    FileStream file = File.Open(packPath, FileMode.Open);
                    packsData.Add((PackData)bf.Deserialize(file));
                    file.Close();
                }

                return packsData;
            }
            else
            {
                Debug.LogError("Missing packs data!");

                CopyPacksData();
                return LoadPacksData();
            }
        }
        else
        {
            Debug.LogError("Missing packs data directory!");

            CopyPacksData();
            return LoadPacksData();
        }
    }

    private void CopyPacksData()
    {
        var fromPath = Application.platform != RuntimePlatform.Android ?
            $"{Application.streamingAssetsPath}/{saveDirectoryPath}" :
            $"jar:file://{Application.dataPath}!/assets/{saveDirectoryPath}/";

        var toPath = $"{Application.persistentDataPath}/{saveDirectoryPath}/";

        Directory.CreateDirectory(toPath);

        for (var i = 0; i < 9; i++)
        {
            var packFileName = $"pack-0{i + 1}";
            var file = new WWW(fromPath + packFileName);

            while (!file.isDone) ;

            File.WriteAllBytes(toPath + packFileName, file.bytes);
        }
    }

    private List<Pack> AssemblePacks(List<PackData> packsData, ProgressData progressData)
    {
        var packs = new List<Pack>();

        // if (!CheckProgressDataIntegrity(packsData, progressData))
        // {
        //     progressData = RepairProgressData(packsData, progressData);
        // }

        for (var i = 0; i < packsData.Count; i++)
        {
            var puzzles = new Puzzle[packsData[i].puzzles.Length];

            for (var j = 0; j < packsData[i].puzzles.Length; j++)
            {
                puzzles[j] = new Puzzle(
                    packsData[i].puzzles[j], packsData[i].puzzlesElementsCounts[j],
                    packsData[i].puzzlesWidths[j], packsData[i].puzzles[j].Length / packsData[i].puzzlesWidths[j],
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

    // // TODO
    // private ProgressData RepairProgressData(List<PackData> packsData, ProgressData progressData)
    // {
    //     return InitGameProgress(packsData);
    // }
}
