using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public GameObject gameField;
    public string puzzleString;
    public bool toolbarMode;

    public string sceneName;

    private Puzzle currentPuzzle;

    private int fieldHeight;

    private int verticalFieldMargin;
    private int tileMargin;
    private int tileSize;

    private int cols;
    private int rows;

    private Color[] colors;
    public GameObject tile;
    public Palette palette;

    public AudioClip drawSoundDown;
    public AudioClip eraseSoundDown;

    public AudioClip drawSoundHover;
    public AudioClip eraseSoundHover;

    private List<GameObject> tiles;

    private bool eraseMode;
    private bool down;

    public void Start()
    {
        eraseMode = false;
        down = false;
        tiles = new List<GameObject>();
        colors = new Color[4];
        ColorUtility.TryParseHtmlString("#4285F4", out colors[0]);
        ColorUtility.TryParseHtmlString("#FBBC05", out colors[1]);
        ColorUtility.TryParseHtmlString("#EA4335", out colors[2]);
        ColorUtility.TryParseHtmlString("#34A853", out colors[3]);
        palette = GameObject.FindObjectOfType<Palette>();

        palette.ColorCount = 2; // It must be taken from Puzzle

        int toolbarScenario = 0;
        if(toolbarMode)
            toolbarScenario = Screen.height / 10;

        puzzleString = puzzleString.Replace(' ', '\n');

        currentPuzzle = new Puzzle("b0011b", 2, 3, 2, false, false);
        currentPuzzle.Clear();

        cols = currentPuzzle.width;
        rows = currentPuzzle.height;

        verticalFieldMargin = Screen.width * 5 / 180;
        tileMargin = Screen.width / 360;

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
            gameObject.GetComponents<AudioSource>()[1].PlayOneShot(eraseSoundHover, 1f);
            return;
        }

        if (currentColor != (currentPuzzle[index / currentPuzzle.width, index % currentPuzzle.width] - '0') && !eraseMode)
        {
            currentPuzzle[index / currentPuzzle.width, index % currentPuzzle.width] = (char)('0' + currentColor);
            tiles[index].GetComponent<Image>().color = colors[currentColor];
            gameObject.GetComponents<AudioSource>()[1].PlayOneShot(drawSoundHover, 1f);
        }

        if (currentPuzzle.CheckForSolution()) {

            Debug.Log("Solved!");

            RemovePartitions();
        }
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
            gameObject.GetComponents<AudioSource>()[1].PlayOneShot(eraseSoundDown, 1f);
        }
        else
        {
            currentPuzzle[index / currentPuzzle.width, index % currentPuzzle.width] = (char)('0' + currentColor);
            tiles[index].GetComponent<Image>().color = colors[currentColor];
            eraseMode = false;
            gameObject.GetComponents<AudioSource>()[1].PlayOneShot(drawSoundDown, 1f);
        }

        if (currentPuzzle.CheckForSolution()) {

            Debug.Log("Solved!");

            RemovePartitions();
        }
    }

    void Update()
    {
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
                RemovePartitionsForSingleTile(i, j);
            }
        }
    }    

    void RemovePartitionsForSingleTile(int tileRow, int tileColumn)
    {
	int longSide = tileSize;
	int shortSide = 2 * tileMargin;

        int tileIndex = tileRow * cols + tileColumn;
        var tileColor = tiles[tileIndex].GetComponent<Image>().color;

        int X = (int)tiles[tileIndex].transform.position.x;
        int Y = (int)tiles[tileIndex].transform.position.y;
        		
        if (tileColumn != cols - 1) { // has right neighbor
            int rightTileIndex = tileRow * cols + (tileColumn + 1);
            var rightTileColor = tiles[rightTileIndex].GetComponent<Image>().color;

            if (tileColor == rightTileColor) {
                var verticalPartition = Instantiate(tile, new Vector3(X + tileSize, Y, 0), Quaternion.identity) as GameObject;
                verticalPartition.transform.SetParent(gameObject.transform, true);

                verticalPartition.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, shortSide);
                verticalPartition.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, longSide);

                verticalPartition.GetComponent<Image>().color = tileColor;

                tiles.Add(verticalPartition);
            }
        }

        if (tileRow != rows - 1) { // has down neighbor
            int downTileIndex = (tileRow + 1) * cols + tileColumn;
            var downTileColor = tiles[downTileIndex].GetComponent<Image>().color;

            if (tileColor == downTileColor) {
                var horizontalPartition = Instantiate(tile, new Vector3(X, Y - tileSize, 0), Quaternion.identity) as GameObject;
                horizontalPartition.transform.SetParent(gameObject.transform, true);

                horizontalPartition.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, longSide);
                horizontalPartition.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, shortSide);

                horizontalPartition.GetComponent<Image>().color = tileColor;

                tiles.Add(horizontalPartition);
            }
        }
    }
}
