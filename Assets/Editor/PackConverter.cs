// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PackConverter : EditorWindow
{
	[MenuItem("Window/Pack Converter")]
	static void Init()
	{
		PackConverter window = (PackConverter)EditorWindow.GetWindow(typeof(PackConverter));
        window.Show();
	}

	private void OnGUI()
	{
		if (GUILayout.Button ("Convert packs"))
            {
                ConvertPacks();
            }
	}

	private void ConvertPacks()
	{

	}
}
