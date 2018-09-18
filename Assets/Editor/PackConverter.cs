using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEditor;

public class PackConverter : EditorWindow
{
    private const string sourceDirectoryPath = "Assets/Editor Default Resources/Packs Data";
    private const string packsExtension = "*.eqld";
    private const string saveDirectoryPath = "Packs Data";

    private List<PackData> packsForConvert;

    [MenuItem("Window/Pack Converter")]
    static void Init() => EditorWindow.GetWindow(typeof(PackConverter)).Show();

    private void OnGUI()
    {
        if (GUILayout.Button("Convert packs"))
        {
            packsForConvert = ReadPacks();
            ConvertPacks(packsForConvert);
        }
    }

    private List<PackData> ReadPacks()
    {
        var readPacks = new List<PackData>();

        if (Directory.Exists(sourceDirectoryPath))
        {
            // Force sorting because the order of the returned file names
            // is not guaranteed in Directory.GetFiles
            var packPathes = Directory
                .GetFiles(sourceDirectoryPath, packsExtension)
                .OrderBy(f => f).ToArray();

            if (packPathes.Length > 0)
            {
                foreach (var packPath in packPathes)
                {
                    readPacks.Add((PackData)File.ReadAllText(packPath));
                }
            }
            else
            {
                Debug.Log("Missing packs for convert!");
            }
        }
        else
        {
            Debug.Log("Missing packs source directory!");
        }

        return readPacks;
    }

    private void ConvertPacks(List<PackData> packsForConvert)
    {
        if (packsForConvert.Count > 0)
        {
			BinaryFormatter bf = new BinaryFormatter();

            var filePath = $"{Application.persistentDataPath}/{saveDirectoryPath}";
            Directory.CreateDirectory(filePath);

            for (var i = 1; i <= packsForConvert.Count; i++)
            {
                var fileName = $"pack-{i.ToString().PadLeft(2, '0')}";
                FileStream file = File.Open($"{filePath}/{fileName}", FileMode.Create);

                bf.Serialize(file, packsForConvert[i - 1]);
  				file.Close();
            }

            Debug.Log($"Successfully converted {packsForConvert.Count} packs!");
        }
        else
        {
            Debug.Log("Nothing to convert!");
        }
    }
}
