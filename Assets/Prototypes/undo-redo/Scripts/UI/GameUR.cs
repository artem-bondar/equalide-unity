using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameUR : MonoBehaviour
{
    public GameObject gameField;
    public string puzzleString;
    public bool toolbarMode;


    private PuzzleBT currentPuzzle;

    private int fieldHeight;

    private int verticalFieldMargin;
    private int tileMargin;
    private int tileSize;

    private int cols;
    private int rows;

    private Color[] colors;
    public GameObject tile;
    public PaletteBT palette;


    private List<GameObject> tiles;
    private List<GameObject> contour;

    private bool eraseMode;
    private bool down;

    private bool solved = false; // used to clear everything after a click if solved
    

    public GameObject undo;
    public GameObject redo;

    private List<char[]> history;
    public int historyPtr = 0;

    private bool changed = false;

    public string curr;
    public void Start()
    {

        

        eraseMode = false;
        down = false;
        tiles = new List<GameObject>();
        contour = new List<GameObject>();
        colors = new Color[4];
        ColorUtility.TryParseHtmlString("#4285F4", out colors[0]);
        ColorUtility.TryParseHtmlString("#FBBC05", out colors[1]);
        ColorUtility.TryParseHtmlString("#EA4335", out colors[2]);
        ColorUtility.TryParseHtmlString("#34A853", out colors[3]);
        palette = GameObject.FindObjectOfType<PaletteBT>();

        palette.ColorCount = 2; // It must be taken from Puzzle

        int toolbarScenario = 0;
        if(toolbarMode)
            toolbarScenario = Screen.height / 10;

        puzzleString = puzzleString.Replace(' ', '\n');

        currentPuzzle = new PuzzleBT("bb0bb00b0001b011b111bb1b", 2, 4,6, false, false);
        currentPuzzle.Clear();

        cols = currentPuzzle.width;
        rows = currentPuzzle.height;

        history = new List<char[]>();

        undo.GetComponent<Button>().interactable = false;
        redo.GetComponent<Button>().interactable = false;

        RecordHistory();

        tileMargin = (int) Mathf.Ceil( Screen.width / 720.0f);

        verticalFieldMargin = tileMargin; //Screen.width * 5 / 180; 

        fieldHeight = Screen.height - 2 * verticalFieldMargin- toolbarScenario - Screen.width / 5 - 8 * tileMargin;        

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
                tiles[index].transform.SetParent(gameField.transform, true);

                tiles[index].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tileSize);
                tiles[index].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tileSize);
                switch(currentPuzzle[i, j])
                {
                    case 'b': tiles[index].GetComponent<Image>().color = Color.clear; break;
                    case 'e': tiles[index].GetComponent<Image>().color = Color.white; break;
                }

                EventTrigger trigger = tiles[index].GetComponent<EventTrigger>();

                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerDown;
                entry.callback.AddListener(delegate { TileDown(index); });
                trigger.triggers.Add(entry);

                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener(delegate { TileHover(index); });
                trigger.triggers.Add(entry);


            }
        }
    }

    void RecordHistory()
    {
        
        char[] record = new char[cols * rows];
        for (int i = 0; i < cols*rows; i++)
        {
            record[i] = currentPuzzle[i / cols, i % cols];
        }
        if (historyPtr != history.Count)
            history.RemoveRange(historyPtr, history.Count - historyPtr);
        history.Add(record);
        historyPtr++;
    }

    void  GetHistory(int ptr)
    {
        for (int i = 0; i < cols * rows; i++)
        {
            currentPuzzle[i / cols, i % cols] = history[ptr][i];
            tiles[i].GetComponent<Image>().color = GetTileColor(history[ptr][i]);
        }       
    }
    Color GetTileColor(char c)
    {
        if (c == 'e')
            return Color.white;

        if (c == 'b')
            return Color.clear;

        return colors[c - '0'];
    }

    public void OnUndoClick()
    {
        ClearContour();
        historyPtr--;
        GetHistory(historyPtr - 1);
        IfSolved();
    }

    public void OnRedoClick()
    {
        ClearContour();
        GetHistory(historyPtr);
        historyPtr++;
        IfSolved();
    }

    void TileHover(int index)
    {
        if (!down)
            return;

        if (currentPuzzle[index / currentPuzzle.width, index % currentPuzzle.width] == 'b')
            return;

        if (palette.selectedIndex == -1)
            return;
       
        int currentColor = palette.selectedIndex;
        //Debug.Log(currentPuzzle[index / currentPuzzle.width, index % currentPuzzle.width]);

        if (currentColor == (currentPuzzle[index / currentPuzzle.width, index % currentPuzzle.width] - '0') && eraseMode)
        {
            currentPuzzle[index / currentPuzzle.width, index % currentPuzzle.width] = 'e';
            tiles[index].GetComponent<Image>().color = Color.white;
            // gameObject.GetComponents<AudioSource>()[1].PlayOneShot(eraseSoundHover, 1f);
            changed = true;
            return;
        }

        if (currentColor != (currentPuzzle[index / currentPuzzle.width, index % currentPuzzle.width] - '0') && !eraseMode)
        {
            currentPuzzle[index / currentPuzzle.width, index % currentPuzzle.width] = (char)('0' + currentColor);
            tiles[index].GetComponent<Image>().color = colors[currentColor];
            //gameObject.GetComponents<AudioSource>()[1].PlayOneShot(drawSoundHover, 1f);
            changed = true;
        }

        IfSolved();
    }

    void TileDown(int index)
    {
        if (currentPuzzle[index / currentPuzzle.width, index % currentPuzzle.width] == 'b')
            return;

        if (palette.selectedIndex == -1)
            return;

        if (solved)
        {
            ClearColors();
            solved = false;
            return;
        }

        down = true;
        int currentColor = palette.selectedIndex;
        
        if (currentColor == (currentPuzzle[index / currentPuzzle.width, index % currentPuzzle.width] - '0'))
        {
            currentPuzzle[index / currentPuzzle.width, index % currentPuzzle.width] = 'e';
            tiles[index].GetComponent<Image>().color = Color.white;
            eraseMode = true;
            changed = true;
            //gameObject.GetComponents<AudioSource>()[1].PlayOneShot(eraseSoundDown, 1f);
        }
        else
        {
            currentPuzzle[index / currentPuzzle.width, index % currentPuzzle.width] = (char)('0' + currentColor);
            tiles[index].GetComponent<Image>().color = colors[currentColor];
            eraseMode = false;
            changed = true;
            //gameObject.GetComponents<AudioSource>()[1].PlayOneShot(drawSoundDown, 1f);
        }

        IfSolved();
    }
    void IfSolved()
    {
        if (currentPuzzle.CheckForSolution())
        {
            Debug.Log("Solved!");
            RemovePartitions();
            StartCoroutine(SetSolved());
        }
        else
            solved = false;
    }

    void ClearColors()
    {  
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (currentPuzzle[i, j] == 'b')
                    continue;

                int index = i * cols + j;
                tiles[index].GetComponent<Image>().color = Color.white;
                currentPuzzle[i, j] = 'e';
            }
        }
        ClearContour();
        RecordHistory();

    }

    void ClearContour()
    {
        foreach (var obj in contour)
        {
            Destroy(obj);
        }
        contour.Clear();
    }

    void Update()
    {

        if (Input.GetMouseButtonUp(0))
        {

            curr = currentPuzzle.Partition;
            if (changed)
            {
                RecordHistory();
                
            }
            down = false;
            changed = false;

            if (historyPtr == 1)
                undo.GetComponent<Button>().interactable = false;
            else
                undo.GetComponent<Button>().interactable = true;
            if (historyPtr == history.Count)
                redo.GetComponent<Button>().interactable = false;
            else
                redo.GetComponent<Button>().interactable = true;
        }
    }

    void RemovePartitions() // removes black partitions between tiles with same color (when puzzle is solved)
    {
        for (int i = 0; i < rows; ++i) {
            for (int j = 0; j < cols; ++j) {
                if(currentPuzzle[i,j] != 'b')
                    RemovePartitionsForSingleTile(i, j);
            }
        }
    }    

    IEnumerator SetSolved()
    {
        yield return new WaitForSeconds(0.1f); //may change later
        solved = true;
    }

    void RemovePartitionsForSingleTile(int i, int j)
    {
	    int longSide = tileSize;
	    int shortSide = 2 * tileMargin;

        int tileIndex = i * cols + j;
        var color = tiles[tileIndex].GetComponent<Image>().color;
        char colorIndex = currentPuzzle[i, j];

        float X = tiles[tileIndex].transform.position.x;
        float Y = tiles[tileIndex].transform.position.y;

        bool rightMatch = false;
        bool downMatch = false;
        		
        if (j != cols - 1) { // has right neighbor
            var rightTileColor = currentPuzzle[i, j + 1];

            if (colorIndex == rightTileColor) {
                var verticalPartition = Instantiate(tile, new Vector3(X + tileSize, Y, 0), Quaternion.identity) as GameObject;
                verticalPartition.transform.SetParent(gameField.transform, true);

                verticalPartition.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, shortSide);
                verticalPartition.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, longSide);

                verticalPartition.GetComponent<Image>().color = color;

                contour.Add(verticalPartition);
                rightMatch = true;
            }
        }

        if (i != rows - 1) { // has down neighbor
            var downTileColor = currentPuzzle[i + 1, j];

            if (colorIndex == downTileColor) {
                var horizontalPartition = Instantiate(tile, new Vector3(X, Y - tileSize, 0), Quaternion.identity) as GameObject;
                horizontalPartition.transform.SetParent(gameField.transform, true);

                horizontalPartition.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, longSide);
                horizontalPartition.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, shortSide);

                horizontalPartition.GetComponent<Image>().color = color;

                contour.Add(horizontalPartition);
                downMatch = true;
            }
        }

        if(rightMatch && downMatch) //if there is a 2x2 square of same-colored tiles - we need to color the center of this square
        {
            var diagonalTileColor = currentPuzzle[i + 1, j + 1];
            if(colorIndex == diagonalTileColor)
            {
                var centerPartition = Instantiate(tile, new Vector3(X + tileSize, Y - tileSize, 0), Quaternion.identity) as GameObject;
                centerPartition.transform.SetParent(gameField.transform, true);

                centerPartition.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, shortSide);
                centerPartition.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, shortSide);

                centerPartition.GetComponent<Image>().color = color;

                contour.Add(centerPartition);
            }
        }
    }
}
