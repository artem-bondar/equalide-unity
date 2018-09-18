using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private Palette palette;
    private PuzzleGrid puzzleGrid;
    private DataManager dataManager;

    public void Start()
    {
        palette = GameObject.FindObjectOfType<Palette>();
        puzzleGrid = GameObject.FindObjectOfType<PuzzleGrid>();
        dataManager = GameObject.FindObjectOfType<DataManager>();

        // puzzleGrid.RenderPuzzle(dataManager.GetCurrentPuzzle());
        puzzleGrid.RenderPuzzle(new Puzzle("eee",3,3,1,true,true));
    }
}
