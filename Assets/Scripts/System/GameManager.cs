using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private Palette palette;
    private PuzzleGrid puzzleGrid;
    private DataManager dataManager;

    private void Start()
    {
        palette = GameObject.FindObjectOfType<Palette>();
        puzzleGrid = GameObject.FindObjectOfType<PuzzleGrid>();
        dataManager = GameObject.FindObjectOfType<DataManager>();

        Puzzle puzzle = dataManager.currentPuzzle;
        puzzleGrid.RenderPuzzle(puzzle);
        palette.Create(puzzle.parts);
        SceneManager.LoadScene("SelectPack", LoadSceneMode.Additive);
    }

    public void ShowPackSelectScreen()
    {
    }

    public void OnMailIntent()
    {
        
    }
}
