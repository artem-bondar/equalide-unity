﻿using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Tooltip("Game screen top app bar title")]
    public Text topAppBarTitle;

    [Tooltip("Floating action button")]
    public GameObject fab;

    private ProgressManager progressManager;
    private TransitionsController transitionsController;

    private Palette palette;
    private PuzzleGrid puzzleGrid;

    private LevelGrid levelGrid;

    private bool levelSolvedState;

    private void Start()
    {
        progressManager = GameObject.FindObjectOfType<ProgressManager>();

        palette = GameObject.FindObjectOfType<Palette>();
        puzzleGrid = GameObject.FindObjectOfType<PuzzleGrid>();

        LoadCurrentPuzzle();
        if (progressManager.currentPuzzle.CheckForSolution())
        {
            OnSolvedLevel();
        }

        transitionsController = GameObject.FindObjectOfType<TransitionsController>();

        levelGrid = GameObject.FindObjectOfType<LevelGrid>();
    }

    public void OnPackSelect(int packIndex)
    {
        if (progressManager.PackState(packIndex) == ProgressState.Opened)
        {
            levelGrid.Destroy();
            levelGrid.Create(packIndex, progressManager.PackProgress(packIndex));
            transitionsController.SelectPackToSelectLevelScreenTransition();
        }
    }

    public void OnLevelSelect(int packIndex, int puzzleIndex)
    {
        if (progressManager.PuzzleState(packIndex, puzzleIndex) == ProgressState.Opened)
        {
            progressManager.SetCurrentLevel(packIndex, puzzleIndex);
            progressManager.currentPuzzle.Refresh();
            progressManager.SaveGame();

            DestroyCurrentPuzzle();
            LoadCurrentPuzzle();

            transitionsController.SelectLevelToGameScreenTransition();
        }
    }

    public void OnMailIntent()
    {
        string subject = System.Uri.EscapeUriString("Equalide feedback");
        string body = System.Uri.EscapeUriString(
            "Hi!\nI\'m on level " +
            $"{progressManager.currentPackIndex + 1}-" +
            $"{progressManager.currentPuzzleIndex + 1}".PadLeft(2, '0') + ".\n" +
            "Here are my thoughts about the game:\n\n");
        Application.OpenURL($"mailto:equalide@gmail.com?subject={subject}&body={body}");
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

        if (!progressManager.IsOnLastLevel())
        {
            fab.SetActive(true);
            fab.GetComponent<Animator>().Play("FadeIn");

            progressManager.OpenNextLevel();
            progressManager.SaveGame();
        }
    }

    public void OnFabClick()
    {
        fab.GetComponent<Animator>().Play("FadeOut");
        fab.SetActive(false);

        if (!progressManager.IsOnLastLevel())
        {
            progressManager.SelectNextLevel();
            progressManager.currentPuzzle.Refresh();
            progressManager.SaveGame();

            DestroyCurrentPuzzle();
            LoadCurrentPuzzle();
        }
    }

    private void LoadCurrentPuzzle()
    {
        topAppBarTitle.text = "Equalide   " +
            $"{progressManager.currentPackIndex + 1}-" +
            $"{progressManager.currentPuzzleIndex + 1}".PadLeft(2, '0');

        puzzleGrid.Create(progressManager.currentPuzzle);

        palette.Create(progressManager.currentPuzzle.elementsCount);
        palette.gameObject.SetActive(true);

        levelSolvedState = false;
    }

    private void DestroyCurrentPuzzle()
    {
        puzzleGrid.Destroy();
        palette.Destroy();
    }
}
