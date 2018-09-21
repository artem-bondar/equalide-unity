using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private PackList packList;
    private DataManager dataManager;

    private Palette palette;
    private PuzzleGrid puzzleGrid;

    [Tooltip("Game screen top app bar title")]
    public Text topAppBarTitle;

    private void Start()
    {
        packList = GameObject.FindObjectOfType<PackList>();
        dataManager = GameObject.FindObjectOfType<DataManager>();

        palette = GameObject.FindObjectOfType<Palette>();
        puzzleGrid = GameObject.FindObjectOfType<PuzzleGrid>();

        topAppBarTitle.text = "Equalide   " +
            $"{dataManager.currentPackIndex + 1}-" +
            $"{dataManager.currentPuzzleIndex + 1}".PadLeft(2, '0');

        Puzzle puzzle = dataManager.currentPuzzle;
        puzzleGrid.RenderPuzzle(puzzle);
        palette.Create(puzzle.parts);

        packList.Create(dataManager.packsProgress);
    }

    public void OnMailIntent()
    {

    }
}
