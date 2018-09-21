using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private DataManager dataManager;

    private Palette palette;
    private PuzzleGrid puzzleGrid;
    
    private PackList packList;
    private Text topAppBarText;

    private void Start()
    {
        dataManager = GameObject.FindObjectOfType<DataManager>();

        palette = GameObject.FindObjectOfType<Palette>();
        puzzleGrid = GameObject.FindObjectOfType<PuzzleGrid>();

        packList = GameObject.FindObjectOfType<PackList>();
        topAppBarText = GameObject.Find("TopAppBarTitle").GetComponent<Text>();
        topAppBarText.text = "Equalide   " +
            $"{dataManager.currentPackIndex + 1}-" +
            $"{dataManager.currentPuzzleIndex + 1}".PadLeft(2, '0');

        Puzzle puzzle = dataManager.currentPuzzle;
        puzzleGrid.RenderPuzzle(puzzle);
        palette.Create(puzzle.parts);

        packList.CreatePackList(dataManager.packsProgress);
    }

    public void OnMailIntent()
    {

    }
}
