using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelGrid : MonoBehaviour
{
    public GameObject levelTile;
    private List<GameObject> levelTiles;

    [Tooltip("Level select screen top app bar title")]
    public Text topAppBarTitle;

    private void Start()
    {
        Create(4, "sssssooooooooocccccc");
    }

    public void Create(int packIndex, string puzzlesStates)
    {
        topAppBarTitle.text = $"Pack {packIndex}";

        var grid = gameObject.GetComponent<GridLayoutGroup>();
        var gridrt = grid.GetComponent<RectTransform>().rect;

        var tileSize = (0.2f * gridrt.width);
        var tileSizeByHeight = (0.14f * gridrt.height);
        
        float tileMargin, horizontalMargin, verticalMargin;

        if (tileSizeByHeight < tileSize)
        {
            // Tiles can't fit to screen by height
            tileSize = tileSizeByHeight;
            tileMargin = (0.01f * gridrt.height);
        }
        else
        {
            tileMargin = ((0.2f / 14) * gridrt.width);
        }

        horizontalMargin = (gridrt.width - 4 * tileSize - 8 * tileMargin) / 2;
        verticalMargin = (gridrt.height - 6 * tileSize - 12 * tileMargin) / 2;

        grid.cellSize = new Vector2(tileSize, tileSize);
        grid.spacing = new Vector2(horizontalMargin, verticalMargin);

        for (var i = 0; i < puzzlesStates.Length; i++)
        {
            var newTile = Instantiate(levelTile);
            newTile.transform.SetParent(gameObject.transform, false);

            newTile.GetComponent<Image>().color = puzzlesStates[i] == 's' ?
                Colors.solvedTileColor : Colors.unsolvedTileColor;
            newTile.GetComponentInChildren<Text>().text = puzzlesStates[i] != 'c' ?
                (i + 1).ToString() : string.Empty;

            var iCopy = i;
            newTile.GetComponent<Button>().onClick.AddListener(delegate { OnLevelTileClick(iCopy); });
        }
    }

    public void Destroy()
    {
        foreach (Transform levelTile in transform)
        {
            Destroy(levelTile.gameObject);
        }
    }

    private void OnLevelTileClick(int puzzleIndex)
    {
    }
}
