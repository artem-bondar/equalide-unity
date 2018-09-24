using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private const string packsDataDirectoryPath = "Packs Data";

    private List<Pack> packs;

    private void Awake() => packs = LoadPacksData().ConvertAll(x => (Pack)x);

    public Pack Pack(int packIndex) =>
        packIndex >= 0 && packIndex < packs.Count ? packs[packIndex] : null;

    // Returns progress data for new game based on loaded packs
    public ProgressData GetInitialProgressData()
    {
        var packsProgress = 'o' + new string('c', packs.Count - 1);
        var puzzlesProgress = new string[packs.Count];

        puzzlesProgress[0] = 'o' + new string('c', packs[0].puzzles.Length - 1);
        
        for (var i = 1; i < packs.Count; i++)
        {
            puzzlesProgress[i] += '\n' + new string('c', packs[i].puzzles.Length);
        }

        return new ProgressData(0, 0, string.Empty, packsProgress, puzzlesProgress);
    }

    // Checks if progress data is valid corresponding to packs data 
    public bool CheckProgressDataIntegrity(ProgressData progressData)
    {
        if (packs.Count != progressData.packsProgress.Length)
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

        for (var i = 0; i < packs.Count; i++)
        {
            if (packs[i].puzzles.Length != progressData.puzzlesProgress[i].Length)
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

    private List<PackData> LoadPacksData()
    {
        var directoryPath = $"{Application.persistentDataPath}/{packsDataDirectoryPath}";

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
            $"{Application.streamingAssetsPath}/{packsDataDirectoryPath}" :
            $"jar:file://{Application.dataPath}!/assets/{packsDataDirectoryPath}/";

        var toPath = $"{Application.persistentDataPath}/{packsDataDirectoryPath}/";

        Directory.CreateDirectory(toPath);

        for (var i = 0; i < 9; i++)
        {
            var packFileName = $"pack-0{i + 1}";
            var file = new WWW(fromPath + packFileName);

            while (!file.isDone) ;

            File.WriteAllBytes(toPath + packFileName, file.bytes);
        }
    }
}
