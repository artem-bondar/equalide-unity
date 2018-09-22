﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private DataManager dataManager;
    private TransitionsController transitionsController;

    private Palette palette;
    private PuzzleGrid puzzleGrid;

    private LevelGrid levelGrid;

    [Tooltip("Game screen top app bar title")]
    public Text topAppBarTitle;

    [Tooltip("Floating action button")]
    public GameObject fab;

    private bool levelSolvedState;

    private void Start()
    {
        dataManager = GameObject.FindObjectOfType<DataManager>();

        palette = GameObject.FindObjectOfType<Palette>();
        puzzleGrid = GameObject.FindObjectOfType<PuzzleGrid>();

        LoadCurrentPuzzle();
        if (dataManager.currentPuzzle.CheckForSolution())
        {
            OnSolvedLevel();
        }

        transitionsController = GameObject.FindObjectOfType<TransitionsController>();

        levelGrid = GameObject.FindObjectOfType<LevelGrid>();
    }

    public void OnPackSelect(int packIndex)
    {
        if (dataManager.GetPack(packIndex).opened)
        {
            levelGrid.Destroy();
            levelGrid.Create(packIndex, dataManager.GetPack(packIndex).puzzlesProgress);
            transitionsController.SelectPackToSelectLevelScreenTransition();
        }
    }

    public void OnLevelSelect(int packIndex, int puzzleIndex)
    {
        if (dataManager.GetPack(packIndex)[puzzleIndex].opened)
        {
            dataManager.SetCurrentLevel(packIndex, puzzleIndex);
            dataManager.currentPuzzle.Refresh();
            dataManager.SaveGameProgress(dataManager.gameProgress);

            DestroyCurrentPuzzle();
            LoadCurrentPuzzle();

            transitionsController.SelectLevelToGameScreenTransition();
        }
    }

    public void OnMailIntent()
    {

    }

    public void OnRefreshButton()
    {
        if (levelSolvedState)
        {
            puzzleGrid.Refresh();
            puzzleGrid.paintLock = false;

            palette.gameObject.SetActive(true);

            fab.GetComponent<Animator>().Play("FadeOut");
            fab.SetActive(false);
        }
    }

    public void OnSolvedLevel()
    {
        puzzleGrid.paintLock = true;
        palette.gameObject.SetActive(false);
        levelSolvedState = true;

        if (!dataManager.IsOnLastLevel())
        {
            fab.SetActive(true);
            fab.GetComponent<Animator>().Play("FadeIn");

            dataManager.OpenNextLevel();
            dataManager.SaveGameProgress(dataManager.gameProgress);
        }
    }

    public void OnFabClick()
    {
        fab.GetComponent<Animator>().Play("FadeOut");
        fab.SetActive(false);

        if (!dataManager.IsOnLastLevel())
        {
            dataManager.SelectNextLevel();
            dataManager.currentPuzzle.Refresh();
            dataManager.SaveGameProgress(dataManager.gameProgress);

            DestroyCurrentPuzzle();
            LoadCurrentPuzzle();
        }
    }

    private void LoadCurrentPuzzle()
    {
        topAppBarTitle.text = "Equalide   " +
            $"{dataManager.currentPackIndex + 1}-" +
            $"{dataManager.currentPuzzleIndex + 1}".PadLeft(2, '0');

        puzzleGrid.Create(dataManager.currentPuzzle);

        palette.Create(dataManager.currentPuzzle.elementsCount);
        palette.gameObject.SetActive(true);

        levelSolvedState = false;
    }

    private void DestroyCurrentPuzzle()
    {
        puzzleGrid.Destroy();
        palette.Destroy();
    }
}
