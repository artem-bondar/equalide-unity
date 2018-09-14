using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEditor;

public class PackConverter : EditorWindow
{
    private readonly string packsDirectoryPath = "Assets/Editor Default Resources/Packs Data";
    private readonly string packsSaveDirectoryPath = $"Packs Data";
    private readonly int packsAmount = 9;

    private List<Pack> packsForConvert;

    [MenuItem("Window/Pack Converter")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(PackConverter)).Show();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Convert packs"))
        {
            ReadPacks();
            ConvertPacks();
        }
    }

    private void ReadPacks()
    {
        for (var i = 1; i <= packsAmount; i++)
        {
            var fileName = $"pack-{i.ToString().PadLeft(2, '0')}.eqld";
            var filePath = $"{packsDirectoryPath}/{fileName}";

            if (File.Exists(filePath))
            {
                packsForConvert.Add(new Pack(File.ReadAllText(filePath)));
                Debug.Log($"Successfully read: {fileName}");
            }
            else
            {
                Debug.Log($"File doesn't exist: {filePath}");
            }
        }
    }

    private void ConvertPacks()
    {
        if (packsForConvert.Count > 0)
        {
			BinaryFormatter bf = new BinaryFormatter();

            for (var i = 1; i <= packsForConvert.Count; i++)
            {
                var fileName = $"pack-{i.ToString().PadLeft(2, '0')}";
                var filePath = $"{Application.persistentDataPath}/{fileName}";

                FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Create);
                bf.Serialize(file, packsForConvert[i]);
  				file.Close();
            }
        }
        else
        {
            Debug.Log("Nothing to convert!");
        }
    }
}
