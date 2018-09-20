using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private Palette palette;
    private PuzzleGrid puzzleGrid;
    private DataManager dataManager;
    private SelectPackManager selectPackManager;

    private void Start()
    {
        palette = GameObject.FindObjectOfType<Palette>();
        puzzleGrid = GameObject.FindObjectOfType<PuzzleGrid>();
        dataManager = GameObject.FindObjectOfType<DataManager>();
        selectPackManager = GameObject.FindObjectOfType<SelectPackManager>();

        Puzzle puzzle = dataManager.currentPuzzle;
        puzzleGrid.RenderPuzzle(puzzle);
        palette.Create(puzzle.parts);

        selectPackManager.CreatePackList(dataManager.packsProgress);
    }

    public void OnMailIntent()
    {
        
    }
}
