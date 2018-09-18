using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PuzzleGrid : MonoBehaviour
{
    public GameObject primitive;
    private const int primitiveMargin = 3; // 1 dp
    private readonly List<Image> primitives = new List<Image>();

    private Puzzle puzzle;

    // public AudioClip drawSoundDown;
    // public AudioClip eraseSoundDown;

    // public AudioClip drawSoundHover;
    // public AudioClip eraseSoundHover;

    private bool eraseMode;
    private bool duringSwipe;

    public void RenderPuzzle(Puzzle puzzle)
    {
        Debug.Log(puzzle.partition);
        this.puzzle = puzzle;

        var grid = gameObject.GetComponent<GridLayoutGroup>();
        var gridRectTransorm = grid.GetComponent<RectTransform>().rect;

        var primitiveSize = Mathf.Min(
            (gridRectTransorm.width - (puzzle.width - 1) * primitiveMargin) / puzzle.width,
            (gridRectTransorm.height - (puzzle.height - 1) * primitiveMargin) / puzzle.height);

        grid.cellSize = new Vector2(primitiveSize, primitiveSize);

        for (var i = 0; i < puzzle.height; i++)
        {
            for (var j = 0; j < puzzle.width; j++)
            {
                var newPrimitive = Instantiate(primitive).GetComponent<Image>();
                newPrimitive.transform.SetParent(grid.transform);

                Debug.Log(puzzle[i, j]);
                if (puzzle[i, j] != 'e')
                {
                    newPrimitive.GetComponent<Image>().color = 
                        (puzzle[i, j] == 'b') ? Colors.backgroundColor :
                            Colors.cellColors[puzzle[i, j] - '0'];
                }

                var trigger = newPrimitive.GetComponent<EventTrigger>();
                var iCopy = i;
                var jCopy = j;

                var entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerDown;
                entry.callback.AddListener(delegate { TileDown(iCopy, jCopy); });
                trigger.triggers.Add(entry);

                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener(delegate { TileHover(iCopy, jCopy); });
                trigger.triggers.Add(entry);

                primitives.Add(newPrimitive);
            }
        }
    }

    void TileHover(int i, int j)
    {
        if (!duringSwipe || puzzle[i, j] == 'b')
        {
            return;
        }

        int currentColor = 0; // !!!

        if (currentColor == (puzzle[i, j] - '0') && eraseMode)
        {
            puzzle[i, j] = 'e';
            primitives[i * puzzle.width + j].GetComponent<Image>().color = Color.white;
            // gameObject.GetComponents<AudioSource>()[1].PlayOneShot(eraseSoundHover, 1f);
            return;
        }

        if (currentColor != (puzzle[i, j] - '0') && !eraseMode)
        {
            puzzle[i, j] = (char)('0' + currentColor);
            primitives[i * puzzle.width + j].GetComponent<Image>().color = Colors.cellColors[currentColor];
            // gameObject.GetComponents<AudioSource>()[1].PlayOneShot(drawSoundHover, 1f);
        }

        if (puzzle.CheckForSolution())
        {
            // RemovePartitions();
        }
    }

    void TileDown(int i, int j)
    {
        if (puzzle[i, j] == 'b')
        {
            return;
        }

        duringSwipe = true;
        int currentColor = 0; // !!!

        if (currentColor == (puzzle[i, j] - '0'))
        {
            puzzle[i, j] = 'e';
            primitives[i * puzzle.width + j].GetComponent<Image>().color = Color.white;
            eraseMode = true;
            // gameObject.GetComponents<AudioSource>()[1].PlayOneShot(eraseSoundDown, 1f);
        }
        else
        {
            puzzle[i, j] = (char)('0' + currentColor);
            primitives[i * puzzle.width + j].GetComponent<Image>().color = Colors.cellColors[currentColor];
            eraseMode = false;
            // gameObject.GetComponents<AudioSource>()[1].PlayOneShot(drawSoundDown, 1f);
        }

        if (puzzle.CheckForSolution())
        {
            // RemovePartitions();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            duringSwipe = false;
        }
    }

    /* void RemovePartitions() // removes black partitions between tiles with same color (when puzzle is solved)
    {
        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < cols; ++j)
            {
                RemovePartitionsForSingleTile(i, j);
            }
        }
    }

    void RemovePartitionsForSingleTile(int tileRow, int tileColumn)
    {
        int longSide = tileSize;
        int shortSide = 2 * primitiveMargin;

        int tileIndex = tileRow * cols + tileColumn;
        var tileColor = primitives[tileIndex].GetComponent<Image>().color;

        int X = (int)primitives[tileIndex].transform.position.x;
        int Y = (int)primitives[tileIndex].transform.position.y;

        if (tileColumn != cols - 1)
        { // has right neighbor
            int rightTileIndex = tileRow * cols + (tileColumn + 1);
            var rightTileColor = primitives[rightTileIndex].GetComponent<Image>().color;

            if (tileColor == rightTileColor)
            {
                var verticalPartition = Instantiate(primitive, new Vector3(X + tileSize, Y, 0), Quaternion.identity) as GameObject;
                verticalPartition.transform.SetParent(gameObject.transform, true);

                verticalPartition.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, shortSide);
                verticalPartition.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, longSide);

                verticalPartition.GetComponent<Image>().color = tileColor;

                primitives.Add(verticalPartition);
            }
        }

        if (tileRow != rows - 1)
        { // has down neighbor
            int downTileIndex = (tileRow + 1) * cols + tileColumn;
            var downTileColor = primitives[downTileIndex].GetComponent<Image>().color;

            if (tileColor == downTileColor)
            {
                var horizontalPartition = Instantiate(primitive, new Vector3(X, Y - tileSize, 0), Quaternion.identity) as GameObject;
                horizontalPartition.transform.SetParent(gameObject.transform, true);

                horizontalPartition.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, longSide);
                horizontalPartition.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, shortSide);

                horizontalPartition.GetComponent<Image>().color = tileColor;

                primitives.Add(horizontalPartition);
            }
        }
    } */
}
