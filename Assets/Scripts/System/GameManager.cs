using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private Palette palette;
    private PuzzleGrid puzzleGrid;
    private DataManager dataManager;
    private SelectPackManager selectPackManager;

    private Text topAppBarText;

    private void Start()
    {
        palette = GameObject.FindObjectOfType<Palette>();
        puzzleGrid = GameObject.FindObjectOfType<PuzzleGrid>();
        dataManager = GameObject.FindObjectOfType<DataManager>();
        selectPackManager = GameObject.FindObjectOfType<SelectPackManager>();

        topAppBarText = GameObject.Find("TopAppBarTitle").GetComponent<Text>();
        topAppBarText.text = "Equalide   " + 
            $"{dataManager.currentPackIndex + 1}-" +
            $"{dataManager.currentPuzzleIndex + 1}".PadLeft(2, '0');

        Puzzle puzzle = dataManager.currentPuzzle;
        puzzleGrid.RenderPuzzle(puzzle);
        palette.Create(puzzle.parts);

        selectPackManager.CreatePackList(dataManager.packsProgress);
    }

    public void OnMailIntent()
    {
        
    }
}
