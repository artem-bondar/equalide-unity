﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class PackConverter : EditorWindow
{
	private readonly string packsDirectoryPath = "Assets/Editor Default Resources/Packs Data";
	private readonly string packsSaveDirectoryPath = $"{Application.persistentDataPath}/Packs Data";
	private readonly int packsAmount = 9;

	private List<string> packsForConvert;

	[MenuItem("Window/Pack Converter")]
	static void Init()
	{
		EditorWindow.GetWindow(typeof(PackConverter)).Show();
	}

	private void OnGUI()
	{
		if (GUILayout.Button ("Convert packs"))
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
				packsForConvert.Add(File.ReadAllText(filePath));
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
			for (var i = 1; i <= packsForConvert.Count; i++)
			{
				var fileName = $"pack-{i.ToString().PadLeft(2, '0')}";
				var filePath = $"{Application.persistentDataPath}/{fileName}";
			}
		}
		else
		{
			Debug.Log("Nothing to convert!");
		}
	}
}
