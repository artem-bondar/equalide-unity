using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelection : MonoBehaviour
{
    public string levelStates;


    public Color colorSolvedTile;
    public Color colorUnSolvedTile;
    public Color colorCheckedSolvedTile;
    public Color colorCheckedUnSolvedTile;

    public GUIStyle buttonStyle;

    private Texture2D solved;
    private Texture2D unSolved;
    private Texture2D solvedActive;
    private Texture2D unSolvedActive;

    void Awake()
    {
        solved = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        solved.SetPixel(0, 0, colorSolvedTile);
        solved.Apply();

        unSolved = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        unSolved.SetPixel(0, 0, colorUnSolvedTile);
        unSolved.Apply();

        solvedActive = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        solvedActive.SetPixel(0, 0, colorCheckedSolvedTile);
        solvedActive.Apply();

        unSolvedActive = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        unSolvedActive.SetPixel(0, 0, colorCheckedUnSolvedTile);
        unSolvedActive.Apply();
        GUI.backgroundColor = Color.white;
    }

    void OnGUI()
    {
        int tileSize = (int)(0.2 * Screen.width);
        int tileSizeByHeight = (int)(0.14 * Screen.height);
        int tileMargin;


        // Debug.Log(buttons.ToString());


        if (tileSizeByHeight < tileSize)
        {
            // Tiles can't fit to screen by height
            tileSize = tileSizeByHeight;
            tileMargin = (int)(0.01 * Screen.height);
        }
        else
            tileMargin = (int)((0.2 / 14) * Screen.width);
        buttonStyle.fontSize = (int)tileSize / 3;


        int horizontalMargin = (Screen.width - 4 * tileSize - 8 * tileMargin) / 2;
        int verticalMargin = (Screen.height - 6 * tileSize - 12 * tileMargin) / 2;
        for (int i = 0; i <= 5; i++)
        {
            for (int j = 0; j <= 3; j++)
            {
                int index = i * 4 + j;
                string text = "";

                if (index + 1 > levelStates.Length || levelStates[index] == 'c') // closed levels
                {
                    buttonStyle.normal.background = unSolved;
                    buttonStyle.active.background = unSolvedActive;
                }
                else if (levelStates[index] == 's') // solved levels
                {
                    text = (index + 1).ToString();
                    buttonStyle.normal.background = solved;
                    buttonStyle.active.background = solvedActive;
                }
                else // open levels
                {
                    text = (index + 1).ToString();
                    buttonStyle.normal.background = unSolved;
                    buttonStyle.active.background = unSolvedActive;
                }

                int X = horizontalMargin + tileMargin + (tileMargin * 2 + tileSize) * j;
                int Y = verticalMargin + tileMargin + (tileMargin * 2 + tileSize) * i;

                if (GUI.Button(new Rect(X, Y, tileSize, tileSize), text, buttonStyle))
                {
                    Application.LoadLevel(index + 1);
                }
            }
        }
    }
}
