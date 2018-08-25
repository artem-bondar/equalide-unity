using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameField : MonoBehaviour
{

    public string puzzleString;
    public bool toolbarMode;


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

        int toolbarScenario = 0;
        if(toolbarMode)
            toolbarScenario = Screen.height / 10;

        puzzleString = puzzleString.Replace(' ', '\n');
        currentPuzzle = new Puzzle(puzzleString);

        cols = currentPuzzle.width;
        rows = currentPuzzle.height;

        verticalFieldMargin = Screen.width * 5 / 180;
        tileMargin = Screen.width / 180;

        fieldHeight = Screen.height - 2 * verticalFieldMargin - Screen.width / 5- toolbarScenario;        

        tileSize = Mathf.Min((fieldHeight - 2 * tileMargin * rows) / rows, (Screen.width - 2 * tileMargin * cols) / cols);

        int centringTop = (fieldHeight - rows * (tileSize + 2 * tileMargin)) / 2;
        int centringLeft = (Screen.width - cols * (tileSize + 2 * tileMargin)) / 2;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                int index = i * cols + j;

                int X = centringLeft + tileMargin + j * (tileSize + 2 * tileMargin);
                int Y = Screen.height - toolbarScenario - centringTop - verticalFieldMargin - tileMargin - i * (tileSize + 2 * tileMargin);

                tiles.Add(Instantiate(tile, new Vector3(X, Y, 0), Quaternion.identity) as GameObject);
                tiles[index].transform.SetParent(gameObject.transform, true);

                tiles[index].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tileSize);
                tiles[index].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tileSize);
                switch(currentPuzzle.get(i,j))
                {
                    case 'b': tiles[index].GetComponent<Image>().color = Color.gray; break;
                    case 'e': tiles[index].GetComponent<Image>().color = Color.white; break;
                    default: tiles[index].GetComponent<Image>().color = colors[( currentPuzzle.get(i, j) -'0')]; break;
                }
            }
        }
    }

    void TileClick(int index)
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}