using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Tooltip("Game screen top app bar title")]
    public Text topAppBarTitle;

    [Tooltip("Floating action button")]
    public GameObject fab;

    private DataManager dataManager;
    private TransitionsController transitionsController;

    private Palette palette;
    private PuzzleGrid puzzleGrid;

    private LevelGrid levelGrid;

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
        if (dataManager.Pack(packIndex).opened)
        {
            levelGrid.Destroy();
            levelGrid.Create(packIndex, dataManager.Pack(packIndex).puzzlesProgress);
            transitionsController.SelectPackToSelectLevelScreenTransition();
        }
    }

    public void OnLevelSelect(int packIndex, int puzzleIndex)
    {
        if (dataManager.Pack(packIndex)[puzzleIndex].opened)
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
        string subject = System.Uri.EscapeUriString("Equalide feedback");
        string body = System.Uri.EscapeUriString(
            "Hi!\nI\'m on level " +
            $"{dataManager.currentPackIndex + 1}-" +
            $"{dataManager.currentPuzzleIndex + 1}".PadLeft(2, '0') + ".\n" +
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
