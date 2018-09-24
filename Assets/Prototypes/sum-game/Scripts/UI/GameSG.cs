using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameSG : MonoBehaviour
{
    public GameObject gameField;
    public string puzzleString;
    public bool toolbarMode;

    public string sceneName;

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

    public AudioClip drawSoundDown;
    public AudioClip eraseSoundDown;

    public AudioClip drawSoundHover;
    public AudioClip eraseSoundHover;

    private List<GameObject> tiles;
    private List<GameObject> contour;

    private bool eraseMode;
    private bool down;

    private bool solved = false; // used to clear everything after a click if solved

    public Font font;
    public int[,] TextMas ={{10,15,5,12,13,7,17,18,4,2,16,14,19,8},{0,0,0,0,0,0,0,0,0,0,0,0,0,0}};
    private int TextCounter = 0; 
    public int fontsize;
    private GameObject[] TextGameMas;
    public int CounterFirst;
    public int CounterLast;
    int BlueTextSum=0;
    int YellowTextSum=0;

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

        
        tileMargin = (int) Mathf.Ceil( Screen.width / 720.0f);

        verticalFieldMargin = tileMargin; //Screen.width * 5 / 180; 

        fieldHeight = Screen.height - 2 * verticalFieldMargin - Screen.width / 5- toolbarScenario;        

        tileSize = Mathf.Min((fieldHeight - 2 * tileMargin * rows) / rows, (Screen.width - 2 * tileMargin * cols) / cols);

        int centringTop = (fieldHeight - rows * (tileSize + 2 * tileMargin)) / 2;
        int centringLeft = (Screen.width - cols * (tileSize + 2 * tileMargin)) / 2;
        TextGameMas = new GameObject[(rows)*cols+(cols)-1];
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

                if(tiles[index].GetComponent<Image>().color == Color.white)
                {
                    TextCreator(index);
                }
            }
        }
        
    }

    void TextCreator (int index)
    {   
        if(TextCounter >13) return;    
        GameObject newText = new GameObject("ButtonText"+index, typeof(Text));
        newText.transform.SetParent(tiles[index].transform);
        newText.GetComponent<Text>().text ="" + TextMas[0,TextCounter];
        newText.GetComponent<Text>().font = font;
        newText.GetComponent<Text>().fontSize=40;
        fontsize=newText.GetComponent<Text>().fontSize;
        RectTransform rt = newText.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.anchoredPosition = new Vector2(0, 0);
        rt.sizeDelta = new Vector2(0, 0);
        newText.GetComponent<Text>().color = Color.black;
        newText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        TextGameMas[index] = newText;
        TextMas[1,TextCounter] = index;
        TextCounter++;
        
    }

    int MiddleTile(int ColorIndex)
    {   
        CounterFirst = 0;
        while(tiles[CounterFirst].GetComponent<Image>().color != colors[ColorIndex])
        {
            CounterFirst++;
        }
        CounterLast = CounterFirst;
        while(tiles[CounterLast].GetComponent<Image>().color != Color.white)
        {
            CounterLast++;
        }
        if(tiles[CounterLast].GetComponent<Image>().color != colors[ColorIndex])
        {
            while(tiles[CounterLast].GetComponent<Image>().color != colors[ColorIndex])
            {
                CounterLast--;
            }
        }
        if(tiles[(CounterFirst+CounterLast)/2].GetComponent<Image>().color != colors[ColorIndex])
        {   
            if(tiles[(CounterFirst+CounterLast)/2 - 1].GetComponent<Image>().color != colors[ColorIndex])
            return (CounterFirst+CounterLast)/2 + 1;
            else return (CounterFirst+CounterLast)/2 - 1;
        }
        else return (CounterFirst+CounterLast)/2;
    }

    void TextOnOff(int ColorIndex)
    {   
        int k = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                int index = i * cols + j;
                if(tiles[index].GetComponent<Image>().color == colors[ColorIndex])
                {   
                    if(index<=CounterLast)
                    {
                        TextGameMas[index].gameObject.SetActive(false);
                    }
                }
                if(tiles[index].GetComponent<Image>().color == Color.white)
                {
                    TextGameMas[index].gameObject.SetActive(true);
                    while(TextMas[1,k]!= index)
                    {
                        k++;
                    }  
                    TextGameMas[index].GetComponent<Text>().text ="" + TextMas[0,k];
                }
            }
        }
    }

    void TilesSum (int CurrentColor, int index)
    {  
        int MiddleIndex;
        int j = 0;  
        
        MiddleIndex = MiddleTile(CurrentColor);
        TextOnOff(CurrentColor);
        TextGameMas[MiddleIndex].gameObject.SetActive(true);
        if(index-1>0)
        if(tiles[index].GetComponent<Image>().color == tiles[index-1].GetComponent<Image>().color)
        {   
            while(TextMas[1,j]!= index)
                    {
                        j++;
                    }   
            if(CurrentColor==0)
            {
                BlueTextSum = BlueTextSum + TextMas[0,j];
                TextGameMas[index].GetComponent<Text>().text ="" + BlueTextSum;
            }
            else 
            {
                YellowTextSum = YellowTextSum + TextMas[0,j];
                TextGameMas[index].GetComponent<Text>().text ="" + YellowTextSum;
            }
        }
        if(index+1<((rows)*cols+(cols)-1))
        if(tiles[index].GetComponent<Image>().color == tiles[index+1].GetComponent<Image>().color)
        {
           while(TextMas[1,j]!= index)
                    {
                        j++;
                    }   
            if(CurrentColor==0)
            {
                BlueTextSum = BlueTextSum + TextMas[0,j];
                TextGameMas[index].GetComponent<Text>().text ="" + BlueTextSum;
            }
            else 
            {
                YellowTextSum = YellowTextSum + TextMas[0,j];
                TextGameMas[index].GetComponent<Text>().text ="" + YellowTextSum;
            }
        }
        if(index-4>0)
        if(tiles[index].GetComponent<Image>().color == tiles[index-4].GetComponent<Image>().color)
        {
            while(TextMas[1,j]!= index)
                    {
                        j++;
                    }   
            if(CurrentColor==0)
            {
                BlueTextSum = BlueTextSum + TextMas[0,j];
                TextGameMas[index].GetComponent<Text>().text ="" + BlueTextSum;
            }
            else 
            {
                YellowTextSum = YellowTextSum + TextMas[0,j];
                TextGameMas[index].GetComponent<Text>().text ="" + YellowTextSum;
            }
        }
        if(index+4<((rows)*cols+(cols)-1))
        if(tiles[index].GetComponent<Image>().color == tiles[index+4].GetComponent<Image>().color)
        {
            while(TextMas[1,j]!= index)
                    {
                        j++;
                    }   
            if(CurrentColor==0)
            {
                BlueTextSum = BlueTextSum + TextMas[0,j];
                TextGameMas[index].GetComponent<Text>().text ="" + BlueTextSum;
            }
            else 
            {
                YellowTextSum = YellowTextSum + TextMas[0,j];
                TextGameMas[index].GetComponent<Text>().text ="" + YellowTextSum;
            }
        }
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
            return;
        }

        if (currentColor != (currentPuzzle[index / currentPuzzle.width, index % currentPuzzle.width] - '0') && !eraseMode)
        {
            currentPuzzle[index / currentPuzzle.width, index % currentPuzzle.width] = (char)('0' + currentColor);
            tiles[index].GetComponent<Image>().color = colors[currentColor];
            //gameObject.GetComponents<AudioSource>()[1].PlayOneShot(drawSoundHover, 1f);
        }

        if (currentPuzzle.CheckForSolution()) {

            Debug.Log("Solved!");
            RemovePartitions();
        }
        TilesSum(currentColor,index);
    }

    void TileDown(int index)
    {
        if (currentPuzzle[index / currentPuzzle.width, index % currentPuzzle.width] == 'b')
            return;

        if (palette.selectedIndex == -1)
            return;

        down = true;
        int currentColor = palette.selectedIndex;
    
        if (currentColor == (currentPuzzle[index / currentPuzzle.width, index % currentPuzzle.width] - '0'))
        {
            currentPuzzle[index / currentPuzzle.width, index % currentPuzzle.width] = 'e';
            tiles[index].GetComponent<Image>().color = Color.white;
            eraseMode = true;
            //gameObject.GetComponents<AudioSource>()[1].PlayOneShot(eraseSoundDown, 1f);
        }
        else
        {
            currentPuzzle[index / currentPuzzle.width, index % currentPuzzle.width] = (char)('0' + currentColor);
            tiles[index].GetComponent<Image>().color = colors[currentColor];
            eraseMode = false;
            //gameObject.GetComponents<AudioSource>()[1].PlayOneShot(drawSoundDown, 1f);
        }

        if (currentPuzzle.CheckForSolution()) {

            Debug.Log("Solved!");
            RemovePartitions();
        }
        TilesSum(currentColor,index);
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

        foreach (var obj in contour)
        {
            Destroy(obj);
        }
        contour.Clear();
    }

    void Update()
    {
        if(solved && Input.GetMouseButtonDown(0))
        {
            ClearColors();
            solved = false;
        }

        if (Input.GetMouseButtonUp(0))
        {
            down = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
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
        StartCoroutine(SetSolved());
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
