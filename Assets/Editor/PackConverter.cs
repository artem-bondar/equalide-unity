﻿using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
using UnityEditor;

using Data;

namespace Editor
{
    public class PackConverter : EditorWindow
    {
        private const string sourceDirectoryPath = "Assets/Editor Default Resources/Packs Data";
        private const string packsDirectoryName = "Packs Data";
        private const string packsExtension = "*.eqld";

        [MenuItem("Window/Pack Converter")]
        static void Init() => EditorWindow.GetWindow(typeof(PackConverter)).Show();

        private void OnGUI()
        {
            if (GUILayout.Button("Convert packs"))
            {
                ConvertPacks(ReadPacks());
            }
        }

        private List<PackData> ReadPacks()
        {
            var readPacks = new List<PackData>();

            if (Directory.Exists(sourceDirectoryPath))
            {
                // Force sorting because the order of the returned file names
                // is not guaranteed in Directory.GetFiles() method
                string[] packPathes = Directory
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
                    Debug.LogWarning("Missing packs for convert!");
                }
            }
            else
            {
                Debug.LogError("Missing packs source directory!");
            }

            return readPacks;
        }

        private void ConvertPacks(List<PackData> packsForConvert)
        {
            if (packsForConvert.Count > 0)
            {
                var bf = new BinaryFormatter();

                var filePath = $"{Application.persistentDataPath}/{packsDirectoryName}";
                Directory.CreateDirectory(filePath);

                for (var i = 0; i < packsForConvert.Count; i++)
                {
                    var fileName = $"pack-{(i + 1).ToString().PadLeft(2, '0')}";
                    FileStream file = File.Open($"{filePath}/{fileName}", FileMode.Create);

                    bf.Serialize(file, packsForConvert[i]);
                    file.Close();
                }

                Debug.Log($"Successfully converted {packsForConvert.Count} packs!");
            }
            else
            {
                Debug.LogWarning("Nothing to convert!");
            }
        }
    }
}
