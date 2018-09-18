﻿using System.Collections;
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

        Puzzle puzzle = dataManager.GetCurrentPuzzle();
        puzzleGrid.RenderPuzzle(puzzle);
        palette.Create(puzzle.parts);
    }
}
