using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private DataManager dataManager;
    private TransitionsController transitionsController;

    private Palette palette;
    private PuzzleGrid puzzleGrid;

    private PackList packList;
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

        transitionsController = GameObject.FindObjectOfType<TransitionsController>();

        packList = GameObject.FindObjectOfType<PackList>();
        packList.Create(dataManager.packsProgress);

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
        puzzleGrid.paintLock = false;
        palette.gameObject.SetActive(true);
    }

    public void OnSolvedLevel()
    {
        puzzleGrid.paintLock = true;
        palette.gameObject.SetActive(false);

        if (!dataManager.IsOnLastLevel())
        {
            fab.SetActive(true);
            fab.GetComponent<Animator>().Play("FadeIn");
        }

        levelSolvedState = true;
    }

    public void OnFabClick()
    {
        fab.GetComponent<Animator>().Play("FadeOut");
        fab.SetActive(false);
    }

    private void LoadCurrentPuzzle()
    {
        topAppBarTitle.text = "Equalide   " +
            $"{dataManager.currentPackIndex + 1}-" +
            $"{dataManager.currentPuzzleIndex + 1}".PadLeft(2, '0');

        puzzleGrid.Create(dataManager.currentPuzzle);
        palette.Create(dataManager.currentPuzzle.elementsCount);
        levelSolvedState = false;
    }

    private void DestroyCurrentPuzzle()
    {
        puzzleGrid.Destroy();
        palette.Destroy();
    }
}
