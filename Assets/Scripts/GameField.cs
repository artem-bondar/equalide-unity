using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameField : MonoBehaviour
{

    public string puzzleString;
    
    private Puzzle currentPuzzle;

    private int fieldHeight;

    private int verticalFieldMargin;
    private int tileMargin;
    private int tileSize;

    private int cols;
    private int rows;

    private Color[] colors;

    public GameObject tile;
    public GameObject palette;

    private List<GameObject> tiles;

    // Use this for initialization
    void Start()
    {
        tiles = new List<GameObject>();
        colors = new Color[4];
        ColorUtility.TryParseHtmlString("#4285F4", out colors[0]);
        ColorUtility.TryParseHtmlString("#FBBC05", out colors[1]);
        ColorUtility.TryParseHtmlString("#EA4335", out colors[2]);
        ColorUtility.TryParseHtmlString("#34A853", out colors[3]);

        puzzleString = puzzleString.Replace(' ', '\n');
        currentPuzzle = new Puzzle(puzzleString);

        cols = currentPuzzle.width;
        rows = currentPuzzle.height;

        verticalFieldMargin = Screen.width * 5 / 180;
        tileMargin = Screen.width / 180;

        fieldHeight = Screen.height - 2 * verticalFieldMargin - Screen.width / 5;        

        tileSize = Mathf.Min((Screen.height - 2 * tileMargin * rows) / rows, (Screen.width - 2 * tileMargin * cols) / cols);

        int centringMargin = (fieldHeight - rows * (tileSize + 2 * tileMargin)) / 2;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                int index = i * cols + j;

                int X = tileMargin + j * (tileSize + 2 * tileMargin);
                int Y = Screen.height - centringMargin - verticalFieldMargin - tileMargin - i * (tileSize + 2 * tileMargin);

                tiles.Add(Instantiate(tile, new Vector3(X, Y, 0), Quaternion.identity) as GameObject);
                tiles[index].transform.SetParent(gameObject.transform, true);

                tiles[index].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tileSize);
                tiles[index].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tileSize);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}